module RegisterVisitWithEnv

open Domain
open Services

let getPatientById (env: #IPatientRepositoryP) patientId =
    match env.PatientRepository.GetPatientById patientId with
    | None -> Error PatientNotFound
    | Some patient -> Ok(patient)

let addOrUpdatePatient env (patient: PatientRegistrationInfoDto) =
    match patient with
    | Anonymous email -> Ok(None, email)
    | LoggedIn patientId ->
        getPatientById env patientId |> Result.bind (fun patient -> Ok((Some patientId), patient.Email))
    | AnonymousWithCreatingNewAccount email ->
        let patientId = env.PatientRepository.AddPatient { Id = ""; Name = email; Email = email }
        // let emailContent = $"New patient account has been created from '{email}'"
        // (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
        Ok(Some patientId, email)
    | LoggedInWithUpdatingEmail (patientId, email) ->
        getPatientById env patientId
        |> Result.bind (fun patient ->
            env.PatientRepository.UpdatePatient { patient with Email = email }
            (env: ILoggerP).Logger.Info $"Patient email has been changed from '{patient.Email}' to '{email}' "
            Ok((Some patientId), email))

let createPayment (env: #IExternalPaymentServiceP) visitId price email =
    let externalPaymentId = env.ExternalPaymentService.CreatePayment(visitId, price, email)
    let payment =
        { Id = ""
          UserEmail = email
          Price = price
          VisitId = visitId
          ExternalPaymentId = externalPaymentId }
    (env :> IPaymentRepositoryP).PaymentRepository.AddPayment payment |> ignore
    externalPaymentId

// todo: result {}
let registerVisit (env: #IScheduleRepositoryP) (registration: RegisterVisitRequestDto) =
    match env.ScheduleRepository.GetSlotById registration.SlotId with
    | None -> Error SlotNotFound
    | Some slot ->
        if not (Array.contains registration.ServiceId slot.ServiceIds) then
            Error SlotServiceNotFound
        else
            addOrUpdatePatient env registration.Patient
            |> Result.bind (fun (patientIdO, email) ->
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
                    Ok { VisitId = visitId; ExternalPaymentId = None }
                else
                    let externalPaymentId = createPayment env visitId slot.Price email
                    let emailContent = $"New visit created, please pay in 15 minutes here '{externalPaymentId}' "
                    (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
                    Ok { VisitId = visitId; ExternalPaymentId = Some externalPaymentId })



let tests () =

    let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

    let mutable appEnv = createEnvWithSlots ()

    let mutable registration = { SlotId = "1"; Patient = LoggedIn "1"; ServiceId = "1" }

    // testing whole scenarios
    registerVisit (createEnvWithSlots ()) { registration with SlotId = "3" } === Error SlotNotFound

    registerVisit (createEnvWithSlots ()) { registration with ServiceId = "3" } === Error SlotServiceNotFound

    registerVisit (createEnvWithSlots ()) { registration with Patient = LoggedIn "2" } === Error PatientNotFound

    registerVisit (createEnvWithSlots ()) registration === Ok { VisitId = "1"; ExternalPaymentId = None }

    registerVisit (createEnvWithSlots ()) { registration with SlotId = "2" }
    === Ok { VisitId = "1"; ExternalPaymentId = Some "payment_1" }

    appEnv <- createEnvWithSlots ()

    registerVisit appEnv { registration with Patient = LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com") }
    === Ok { VisitId = "1"; ExternalPaymentId = None }

    (appEnv.getService<IEmailService> () :?> InMemoryEmailService).Store[0].Email === "najder.marcin@gmail.com"

    appEnv.getService<IPatientRepository>().GetPatientById("1").Value.Email === "najder.marcin@gmail.com"


    // testing only subfunctions
    addOrUpdatePatient (createEnvWithSlots ()) (Anonymous "marcin.najder@gmail.com")
    === Ok(None, "marcin.najder@gmail.com")

    addOrUpdatePatient (createEnvWithSlots ()) (AnonymousWithCreatingNewAccount "marcin.najder@gmail.com")
    === Ok(Some "2", "marcin.najder@gmail.com")

    addOrUpdatePatient (createEnvWithSlots ()) (LoggedIn "1") === Ok(Some "1", "marcin.najder@gmail.com")

    addOrUpdatePatient (createEnvWithSlots ()) (LoggedIn "2") === Error PatientNotFound

    appEnv <- createEnvWithSlots ()

    addOrUpdatePatient appEnv (LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com"))
    === Ok(Some "1", "najder.marcin@gmail.com")
