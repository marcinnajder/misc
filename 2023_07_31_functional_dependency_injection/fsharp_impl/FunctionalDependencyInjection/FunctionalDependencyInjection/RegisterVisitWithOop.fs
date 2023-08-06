module RegisterVisitWithOop

open Domain
open Services

type RegistrationService
    (
        patientRepository: IPatientRepository,
        logger: ILogger,
        externalPaymentService: IExternalPaymentService,
        paymentRepository: IPaymentRepository,
        scheduleRepository: IScheduleRepository,
        emailService: IEmailService
    ) =

    let getPatientById patientId =
        match patientRepository.GetPatientById patientId with
        | None -> Error PatientNotFound
        | Some patient -> Ok(patient)


    let addOrUpdatePatient (patient: PatientRegistrationInfoDto) =
        match patient with
        | Anonymous email -> Ok(None, email)
        | LoggedIn patientId ->
            getPatientById patientId |> Result.bind (fun patient -> Ok((Some patientId), patient.Email))
        | AnonymousWithCreatingNewAccount email ->
            let patientId = patientRepository.AddPatient { Id = ""; Name = email; Email = email }
            // let emailContent = $"New patient account has been created from '{email}'"
            // (env :> IEmailServiceP).EmailService.SendEmail(emailContent, email)
            Ok(Some patientId, email)
        | LoggedInWithUpdatingEmail (patientId, email) ->
            getPatientById patientId
            |> Result.bind (fun patient ->
                patientRepository.UpdatePatient { patient with Email = email }
                logger.Info $"Patient email has been changed from '{patient.Email}' to '{email}' "
                Ok((Some patientId), email))

    let createPayment visitId price email =
        let externalPaymentId = externalPaymentService.CreatePayment(visitId, price, email)

        let payment =
            { Id = ""
              UserEmail = email
              Price = price
              VisitId = visitId
              ExternalPaymentId = externalPaymentId }

        paymentRepository.AddPayment payment |> ignore
        externalPaymentId


    let registerVisit (registration: RegisterVisitRequestDto) =
        match scheduleRepository.GetSlotById registration.SlotId with
        | None -> Error SlotNotFound
        | Some slot ->
            if not (Array.contains registration.ServiceId slot.ServiceIds) then
                Error SlotServiceNotFound
            else
                addOrUpdatePatient registration.Patient
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
                        let externalPaymentId = createPayment visitId slot.Price email

                        let emailContent = $"New visit created, please pay in 15 minutes here '{externalPaymentId}' "

                        emailService.SendEmail(emailContent, email)

                        Ok { VisitId = visitId; ExternalPaymentId = Some externalPaymentId })
