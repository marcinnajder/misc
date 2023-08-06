import { Option } from "powerfp";

export type Id = string;

export type PatientRegistrationInfoDto =
    | { type: "Anonymous"; email: string; }
    | { type: "AnonymousWithCreatingNewAccount"; email: string; }
    | { type: "LoggedIn"; patientId: Id; }
    | { type: "LoggedInWithUpdatingEmail"; patientId: Id; email: string; }

export type RegisterVisitRequestDto = { slotId: Id; patient: PatientRegistrationInfoDto; serviceId: Id; }

export type RegisterVisitResponseDto = { visitId: Id; externalPaymentId: Option<Id>; }

export type Slot = { id: string; from: Date; to: Date; serviceIds: Id[]; unitId: Id; price: number; }

export type Visit = { id: Id; slotId: Id; from: Date; to: Date; serviceId: Id; unitId: Id; patientId: Option<Id>; patientEmail: string; }

export type Patient = { id: Id; name: string; email: string; }

export type Payment = { id: Id; userEmail: string; price: number; visitId: Id; externalPaymentId: string; }

export type RegistrationError = "SlotNotFound" | "SlotServiceNotFound" | "PatientNotFound"
