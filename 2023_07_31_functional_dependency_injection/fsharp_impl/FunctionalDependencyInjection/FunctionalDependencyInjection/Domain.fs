module Domain

open System

type Id = string

type PatientRegistrationInfoDto =
    | Anonymous of email: string
    | AnonymousWithCreatingNewAccount of email: string
    | LoggedIn of patientId: Id
    | LoggedInWithUpdatingEmail of patientId: Id * email: string

type RegisterVisitRequestDto = { SlotId: Id; Patient: PatientRegistrationInfoDto; ServiceId: Id }

type RegisterVisitResponseDto = { VisitId: Id; ExternalPaymentId: option<Id> } //Option<Id>

type Slot = { Id: string; From: DateTime; To: DateTime; ServiceIds: Id []; UnitId: Id; Price: decimal }

type Visit =
    { Id: Id
      SlotId: Id
      From: DateTime
      To: DateTime
      ServiceId: Id
      UnitId: Id
      PatientId: Id option
      PatientEmail: string }

type Patient = { Id: Id; Name: string; Email: string }

type Payment = { Id: Id; UserEmail: string; Price: decimal; VisitId: Id; ExternalPaymentId: string }

type RegistrationError =
    | SlotNotFound
    | SlotServiceNotFound
    | PatientNotFound
