module RegisterVisitWithDeps

open Domain
open Services


let getPatientById (patientRepository: IPatientRepository) patientId =
    match patientRepository.GetPatientById patientId with
    | None -> Error PatientNotFound
    | Some patient -> Ok(patient)


let addOrUpdatePatient
    (patientRepository: IPatientRepository)
    (logger: ILogger)
    getPatientById_
    (patient: PatientRegistrationInfoDto)
    =
    match patient with
    | Anonymous email -> Ok(None, email)
    | LoggedIn patientId ->
        getPatientById_ patientId |> Result.bind (fun patient -> Ok((Some patientId), patient.Email))
    | AnonymousWithCreatingNewAccount email ->
        let patientId = patientRepository.AddPatient { Id = ""; Name = email; Email = email }
        // let emailContent = $"New patient account has been created from '{email}'"
        // (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
        Ok(Some patientId, email)
    | LoggedInWithUpdatingEmail (patientId, email) ->
        getPatientById_ patientId
        |> Result.bind (fun patient ->
            patientRepository.UpdatePatient { patient with Email = email }
            logger.Info $"Patient email has been changed from '{patient.Email}' to '{email}' "
            Ok((Some patientId), email))

let createPayment
    (externalPaymentService: IExternalPaymentService)
    (paymentRepository: IPaymentRepository)
    visitId
    price
    email
    =
    let externalPaymentId = externalPaymentService.CreatePayment(visitId, price, email)

    let payment =
        { Id = ""
          UserEmail = email
          Price = price
          VisitId = visitId
          ExternalPaymentId = externalPaymentId }

    paymentRepository.AddPayment payment |> ignore
    externalPaymentId


let registerVisit
    (scheduleRepository: IScheduleRepository)
    (emailService: IEmailService)
    addOrUpdatePatient_
    createPayment_
    (registration: RegisterVisitRequestDto)
    =
    match scheduleRepository.GetSlotById registration.SlotId with
    | None -> Error SlotNotFound
    | Some slot ->
        if not (Array.contains registration.ServiceId slot.ServiceIds) then
            Error SlotServiceNotFound
        else
            addOrUpdatePatient_ registration.Patient
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

                let visitId = scheduleRepository.AddVisit visit

                if slot.Price = 0m then
                    let emailContent = $"New visit created ... "
                    emailService.SendEmail(emailContent, email)

                    Ok { VisitId = visitId; ExternalPaymentId = None }
                else
                    let externalPaymentId = createPayment_ visitId slot.Price email

                    let emailContent = $"New visit created, please pay in 15 minutes here '{externalPaymentId}' "

                    emailService.SendEmail(emailContent, email)

                    Ok { VisitId = visitId; ExternalPaymentId = Some externalPaymentId })



let tests () =

    let (===) actual expected = if actual = expected then () else failwithf "assertion failed: %A <> %A" actual expected

    // let mutable appEnv = createEnvWithSlots ()
    let mutable registration = { SlotId = "1"; Patient = LoggedIn "1"; ServiceId = "1" }

    // let mutable appEnv = createEnvWithSlots ()

    let createAddOrUpdatePatient (appEnv: Env) =
        let getPatientById__ = getPatientById (appEnv.getService<IPatientRepository> ())

        let addOrUpdatePatient__ =
            addOrUpdatePatient
                (appEnv.getService<IPatientRepository> ())
                (appEnv.getService<ILogger> ())
                getPatientById__

        addOrUpdatePatient__

    let createRegisterVisit (appEnv: Env) =
        let addOrUpdatePatient__ = createAddOrUpdatePatient appEnv

        let createPayment__ =
            createPayment (appEnv.getService<IExternalPaymentService> ()) (appEnv.getService<IPaymentRepository> ())

        let registerVisit__ =
            registerVisit
                (appEnv.getService<IScheduleRepository> ())
                (appEnv.getService<IEmailService> ())
                addOrUpdatePatient__
                createPayment__

        registerVisit__


    // testing whole scenarios
    (createRegisterVisit (createEnvWithSlots ())) { registration with SlotId = "3" } === Error SlotNotFound

    (createRegisterVisit (createEnvWithSlots ())) { registration with ServiceId = "3" } === Error SlotServiceNotFound

    (createRegisterVisit (createEnvWithSlots ())) { registration with Patient = LoggedIn "2" } === Error PatientNotFound

    (createRegisterVisit (createEnvWithSlots ())) registration === Ok { VisitId = "1"; ExternalPaymentId = None }

    (createRegisterVisit (createEnvWithSlots ())) { registration with SlotId = "2" }
    === Ok { VisitId = "1"; ExternalPaymentId = Some "payment_1" }

    // appEnv <- createEnvWithSlots ()
    // registerVisit__ appEnv { registration with Patient = LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com") }
    // === Ok { VisitId = "1"; ExternalPaymentId = None }
    // (appEnv.getService<IEmailService> () :?> InMemoryEmailService).Store[0].Email === "najder.marcin@gmail.com"
    // appEnv.getService<IPatientRepository>().GetPatientById("1").Value.Email === "najder.marcin@gmail.com"


    // testing only subfunctions
    (createAddOrUpdatePatient (createEnvWithSlots ())) (Anonymous "marcin.najder@gmail.com")
    === Ok(None, "marcin.najder@gmail.com")

    (createAddOrUpdatePatient (createEnvWithSlots ())) (AnonymousWithCreatingNewAccount "marcin.najder@gmail.com")
    === Ok(Some "2", "marcin.najder@gmail.com")

    (createAddOrUpdatePatient (createEnvWithSlots ())) (LoggedIn "1") === Ok(Some "1", "marcin.najder@gmail.com")

    (createAddOrUpdatePatient (createEnvWithSlots ())) (LoggedIn "2") === Error PatientNotFound

    (createAddOrUpdatePatient (createEnvWithSlots ())) (LoggedInWithUpdatingEmail("1", "najder.marcin@gmail.com"))
    === Ok(Some "1", "najder.marcin@gmail.com")


    // passing dependencies as functions
    addOrUpdatePatient
        Unchecked.defaultof<_>
        Unchecked.defaultof<_>
        Unchecked.defaultof<_>
        (Anonymous "marcin.najder@gmail.com")
    === Ok(None, "marcin.najder@gmail.com")

    addOrUpdatePatient
        Unchecked.defaultof<_>
        Unchecked.defaultof<_>
        (fun id -> Ok({ Id = id; Name = ""; Email = "" }))
        (LoggedIn "1")
    === Ok(Some "1", "")

    addOrUpdatePatient Unchecked.defaultof<_> Unchecked.defaultof<_> (fun id -> Error PatientNotFound) (LoggedIn "1")
    === Error PatientNotFound
