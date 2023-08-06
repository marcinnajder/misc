module RegisterVisitWithEnvCE

open Domain
open Services
open TrickWithEnv



let getPatientById patientId =
    di {
        //let! env = (fun (abc: #IPatientRepositoryP) -> abc)
        let! env = id<#IPatientRepositoryP>
        return
            match env.PatientRepository.GetPatientById patientId with
            | None -> Error PatientNotFound
            | Some patient -> Ok(patient)
    }

let getPatientById' patientId (env: #IPatientRepositoryP) =
    match env.PatientRepository.GetPatientById patientId with
    | None -> Error PatientNotFound
    | Some patient -> Ok(patient)



let addOrUpdatePatient (patient: PatientRegistrationInfoDto) =
    di {
        let! env = id<#IPatientRepositoryP>
        match patient with
        | Anonymous email -> return Ok(None, email)
        | LoggedIn patientId ->
            let! patientR = getPatientById' patientId
            return patientR |> Result.bind (fun patient -> Ok((Some patientId), patient.Email))
        | AnonymousWithCreatingNewAccount email ->
            let patientId = env.PatientRepository.AddPatient { Id = ""; Name = email; Email = email }
            // let emailContent = $"New patient account has been created from '{email}'"
            // (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
            return Ok(Some patientId, email)
        | LoggedInWithUpdatingEmail (patientId, email) ->
            let! patientR = getPatientById patientId
            return
                patientR
                |> Result.bind (fun patient ->
                    env.PatientRepository.UpdatePatient { patient with Email = email }
                    (env :> ILoggerP).Logger.Info $"Patient email has been changed from '{patient.Email}' to '{email}' "
                    Ok((Some patientId), email))

    }

let createPayment visitId price email =
    di {
        let! env = id<#IExternalPaymentServiceP>

        let externalPaymentId = env.ExternalPaymentService.CreatePayment(visitId, price, email)

        let payment =
            { Id = ""
              UserEmail = email
              Price = price
              VisitId = visitId
              ExternalPaymentId = externalPaymentId }
        // let! env2 = id<#IPaymentRepositoryP>
        // env2.PaymentRepository.AddPayment payment |> ignore
        (env :> IPaymentRepositoryP).PaymentRepository.AddPayment payment |> ignore

        return externalPaymentId
    }

// todo: result {}
let registerVisit (registration: RegisterVisitRequestDto) =
    di {
        // (env: #IScheduleRepositoryP)
        let! env = id<#IScheduleRepositoryP>
        match env.ScheduleRepository.GetSlotById registration.SlotId with
        | None -> return Error SlotNotFound
        | Some slot ->
            if not (Array.contains registration.ServiceId slot.ServiceIds) then
                return Error SlotServiceNotFound
            else
                let! patientIdR = addOrUpdatePatient registration.Patient
                match patientIdR with
                | Error err -> return Error err
                | Ok (patientIdO, email) ->
                    let visit =
                        { Id = ""
                          SlotId = slot.Id
                          From = slot.From
                          To = slot.To
                          ServiceId = registration.ServiceId
                          UnitId = slot.UnitId
                          PatientId = patientIdO
                          PatientEmail = email }

                    let visitId = env.ScheduleRepository.AddVisit visit

                    if slot.Price = 0m then
                        let emailContent = $"New visit created ... "
                        (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
                        return Ok { VisitId = visitId; ExternalPaymentId = None }
                    else
                        let! externalPaymentId = createPayment visitId slot.Price email
                        let emailContent = $"New visit created, please pay in 15 minutes here '{externalPaymentId}' "
                        (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
                        return Ok { VisitId = visitId; ExternalPaymentId = Some externalPaymentId }
    }

// why not to use SE di { ... }
// - hard to combine with other CEs
// - special usage of "return .." keyword
// - cannot easily use map/bind functions from Option, Result types


let tests () =

    let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

    let mutable appEnv = createEnvWithSlots ()

    let mutable registration = { SlotId = "1"; Patient = LoggedIn "1"; ServiceId = "1" }

    // testing whole scenarios
    (createEnvWithSlots ()) |> registerVisit { registration with SlotId = "3" } === Error SlotNotFound

    (createEnvWithSlots ()) |> registerVisit { registration with ServiceId = "3" } === Error SlotServiceNotFound

    (createEnvWithSlots ()) |> registerVisit { registration with Patient = LoggedIn "2" } === Error PatientNotFound

    (createEnvWithSlots ()) |> registerVisit registration === Ok { VisitId = "1"; ExternalPaymentId = None }

    (createEnvWithSlots ()) |> registerVisit { registration with SlotId = "2" }
    === Ok { VisitId = "1"; ExternalPaymentId = Some "payment_1" }

    appEnv <- createEnvWithSlots ()

    appEnv |> registerVisit { registration with Patient = LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com") }
    === Ok { VisitId = "1"; ExternalPaymentId = None }

    (appEnv.getService<IEmailService> () :?> InMemoryEmailService).Store[0].Email === "najder.marcin@gmail.com"

    appEnv.getService<IPatientRepository>().GetPatientById("1").Value.Email === "najder.marcin@gmail.com"

    // testing only subfunctions

    (createEnvWithSlots ()) |> addOrUpdatePatient (Anonymous "marcin.najder@gmail.com")
    === Ok(None, "marcin.najder@gmail.com")

    (createEnvWithSlots ()) |> addOrUpdatePatient (AnonymousWithCreatingNewAccount "marcin.najder@gmail.com")
    === Ok(Some "2", "marcin.najder@gmail.com")

    (createEnvWithSlots ()) |> addOrUpdatePatient (LoggedIn "1") === Ok(Some "1", "marcin.najder@gmail.com")

    (createEnvWithSlots ()) |> addOrUpdatePatient (LoggedIn "2") === Error PatientNotFound

    appEnv <- createEnvWithSlots ()

    appEnv |> addOrUpdatePatient (LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com"))
    === Ok(Some "1", "najder.marcin@gmail.com")
