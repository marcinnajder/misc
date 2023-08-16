import { Option, some, none, pipe, error, ok, Result, matchValue } from "powerfp";
import { Id, Patient, Payment, PatientRegistrationInfoDto, RegisterVisitRequestDto, RegisterVisitResponseDto, RegistrationError, Visit, Slot } from "./domain";
import { IEmailServiceP, IExternalPaymentServiceP, ILoggerP, IP, IPatientRepositoryP, IPaymentRepositoryP, IScheduleRepositoryP, createEnvWithSlots } from "./interfaces";

type SkipFirst<T extends (...args: any) => any> = T extends (arg1: any, ...args: infer P) => infer R ? (...args: P) => R : any;

const appState: IPatientRepositoryP & ILoggerP & IExternalPaymentServiceP & IPaymentRepositoryP & IScheduleRepositoryP & IEmailServiceP = {} as any;


function select<T, P extends keyof T>(obj: T, ...props: P[]): Pick<T, P> {
    var result = {} as any;
    for (const p of props) {
        result[p] = obj[p];
    }
    return result;
}
// var patient = { n: 1, s: "", b: true };
// var ns = select(patient);




function getPatientById(patientId: Id, deps = select(appState, "patientRepository")): Result<Patient, RegistrationError> {
    const patient = deps.patientRepository.getPatientById(patientId);
    switch (patient.type) {
        case "some": return ok(patient.value);
        case "none": return error("PatientNotFound");
    }
}

function addOrUpdatePatient(patient: PatientRegistrationInfoDto, deps = { ...select(appState, "logger", "patientRepository"), getPatientById })
    : Result<[Option<Id>, string], RegistrationError> {

    switch (patient.type) {
        case "Anonymous": return ok([none, patient.email]);
        case "LoggedIn": return deps.getPatientById(patient.patientId).bind(p => ok([some(p.id), p.email]));
        case "AnonymousWithCreatingNewAccount":
            const patientId = deps.patientRepository.addPatient({ id: "", name: patient.email, email: patient.email });
            return ok([some(patientId), patient.email]);
        case "LoggedInWithUpdatingEmail":
            return deps.getPatientById(patient.patientId)
                .bind(p => {
                    deps.patientRepository.updatePatient({ ...p, email: patient.email });
                    deps.logger.info(`Patient email has been changed from '${p.email}' to '${patient.email}' `);
                    return ok([some(p.id), p.email])
                });
    }
}




function createPayment(visitId: string, price: number, email: string, deps = select(appState, "externalPaymentService", "paymentRepository")): string {
    const externalPaymentId = deps.externalPaymentService.createPayment(visitId, price, email);
    let payment = { id: "", userEmail: email, price, visitId, externalPaymentId };
    deps.paymentRepository.addPayment(payment)
    return externalPaymentId;
}


export function registerVisit(registration: RegisterVisitRequestDto,
    deps = { ...select(appState, "scheduleRepository", "emailService"), addOrUpdatePatient, createPayment })
    : Result<RegisterVisitResponseDto, RegistrationError> {

    // deps = setDefault(appState, deps);

    const slotO = deps.scheduleRepository.getSlotById(registration.slotId);
    switch (slotO.type) {
        case "none": return error("SlotNotFound");
        case "some":
            if (slotO.value.serviceIds.indexOf(registration.serviceId) === -1) {
                return error("SlotServiceNotFound");
            } else {
                const slot = slotO.value;
                return deps.addOrUpdatePatient(registration.patient)
                    .bind(([patientIdO, email]) => {
                        const visit: Visit = { id: "", slotId: slot.id, from: slot.from, to: slot.to, serviceId: registration.serviceId, unitId: slot.unitId, patientId: patientIdO, patientEmail: email }
                        const visitId = deps.scheduleRepository.addVisit(visit);
                        if (slot.price === 0) {
                            const emailContent = "New visit created ... ";
                            deps.emailService.sendEmail(emailContent, email)
                            return ok({ visitId, externalPaymentId: none }) as Result<RegisterVisitResponseDto, RegistrationError>;
                        } else {
                            const externalPaymentId = deps.createPayment(visitId, slot.price, email);
                            const emailContent = `New visit created, please pay in 15 minutes here $'{externalPaymentId}' `;
                            deps.emailService.sendEmail(emailContent, email);
                            return ok({ visitId, externalPaymentId: some(externalPaymentId) });
                        }
                    });
            }

    }
}
