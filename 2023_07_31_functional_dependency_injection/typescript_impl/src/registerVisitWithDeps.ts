import { Option, some, none, pipe, error, ok, Result, matchValue } from "powerfp";
import { Id, Patient, Payment, PatientRegistrationInfoDto, RegisterVisitRequestDto, RegisterVisitResponseDto, RegistrationError, Visit, Slot } from "./domain";
import { IEmailServiceP, IExternalPaymentServiceP, ILoggerP, IP, IPatientRepositoryP, IPaymentRepositoryP, IScheduleRepositoryP, createEnvWithSlots } from "./interfaces";
import * as assert from "assert";

type SkipFirst<T extends (...args: any) => any> = T extends (arg1: any, ...args: infer P) => infer R ? (...args: P) => R : any;

function getPatientById(env: IPatientRepositoryP, patientId: Id): Result<Patient, RegistrationError> {
    const patient = env.patientRepository.getPatientById(patientId);
    switch (patient.type) {
        case "some": return ok(patient.value);
        case "none": return error("PatientNotFound");
    }
}

function addOrUpdatePatient(
    env: { getPatientById: SkipFirst<typeof getPatientById> } & IPatientRepositoryP & ILoggerP,
    patient: PatientRegistrationInfoDto)
    : Result<[Option<Id>, string], RegistrationError> {

    switch (patient.type) {
        case "Anonymous": return ok([none, patient.email]);
        case "LoggedIn": return env.getPatientById(patient.patientId).bind(p => ok([some(p.id), p.email]));
        case "AnonymousWithCreatingNewAccount":
            const patientId = env.patientRepository.addPatient({ id: "", name: patient.email, email: patient.email });
            return ok([some(patientId), patient.email]);
        case "LoggedInWithUpdatingEmail":
            return env.getPatientById(patient.patientId)
                .bind(p => {
                    env.patientRepository.updatePatient({ ...p, email: patient.email });
                    env.logger.info(`Patient email has been changed from '${p.email}' to '${patient.email}' `);
                    return ok([some(p.id), p.email])
                });
    }
}




function createPayment(env: IExternalPaymentServiceP & IPaymentRepositoryP, visitId: string, price: number, email: string): string {
    const externalPaymentId = env.externalPaymentService.createPayment(visitId, price, email);
    let payment = { id: "", userEmail: email, price, visitId, externalPaymentId };
    env.paymentRepository.addPayment(payment)
    return externalPaymentId;
}


function registerVisit(
    env: { addOrUpdatePatient: SkipFirst<typeof addOrUpdatePatient>, createPayment: SkipFirst<typeof createPayment> } & IScheduleRepositoryP & IEmailServiceP,
    registration: RegisterVisitRequestDto): Result<RegisterVisitResponseDto, RegistrationError> {
    const slotO = env.scheduleRepository.getSlotById(registration.slotId);
    switch (slotO.type) {
        case "none": return error("SlotNotFound");
        case "some":
            if (slotO.value.serviceIds.indexOf(registration.serviceId) === -1) {
                return error("SlotServiceNotFound");
            } else {
                const slot = slotO.value;
                return env.addOrUpdatePatient(registration.patient)
                    .bind(([patientIdO, email]) => {
                        const visit: Visit = { id: "", slotId: slot.id, from: slot.from, to: slot.to, serviceId: registration.serviceId, unitId: slot.unitId, patientId: patientIdO, patientEmail: email }
                        const visitId = env.scheduleRepository.addVisit(visit);
                        if (slot.price === 0) {
                            const emailContent = "New visit created ... ";
                            env.emailService.sendEmail(emailContent, email)
                            return ok({ visitId, externalPaymentId: none }) as Result<RegisterVisitResponseDto, RegistrationError>;
                        } else {
                            const externalPaymentId = env.createPayment(visitId, slot.price, email);
                            const emailContent = `New visit created, please pay in 15 minutes here $'{externalPaymentId}' `;
                            env.emailService.sendEmail(emailContent, email);
                            return ok({ visitId, externalPaymentId: some(externalPaymentId) });
                        }
                    });
            }

    }
}


var appEnv = createEnvWithSlots();
let getPatientById_: SkipFirst<typeof getPatientById> = (...args) => getPatientById(appEnv, ...args);
let addOrUpdatePatient_: SkipFirst<typeof addOrUpdatePatient> = (...args) => addOrUpdatePatient({ ...appEnv, getPatientById: getPatientById_ }, ...args);
let createPayment_: SkipFirst<typeof createPayment> = (...args) => createPayment(appEnv, ...args);
let registerVisit_: SkipFirst<typeof registerVisit> = (...args) => registerVisit({ ...appEnv, createPayment: createPayment_, addOrUpdatePatient: addOrUpdatePatient_ }, ...args);



var registration: RegisterVisitRequestDto = { slotId: "1", patient: { type: 'LoggedIn', patientId: "1" }, serviceId: "1" };

type R = ReturnType<typeof registerVisit>;

assert(matchValue(registerVisit_({ ...registration, slotId: "3" }), error("SlotNotFound") as R));
assert(matchValue(registerVisit_({ ...registration, serviceId: "3" }), error("SlotServiceNotFound") as R));
assert(matchValue(registerVisit_({ ...registration, patient: { type: "LoggedIn", patientId: "2" } }), error("PatientNotFound") as R));

assert(matchValue(registerVisit_(registration), ok({ visitId: "1", externalPaymentId: none }) as R));


// testing only subfunctions
type R2 = ReturnType<typeof getPatientById>;

var noop: any = () => { };

assert(matchValue(
    getPatientById({ patientRepository: { getPatientById: id => none, addPatient: noop, updatePatient: noop } }, "1"),
    error("PatientNotFound") as R2
));

assert(matchValue(
    getPatientById({ patientRepository: { getPatientById: id => some({ id: "1", email: "", name: "" }), addPatient: noop, updatePatient: noop } }, "1"),
    ok({ id: "1", email: "", name: "" }) as R2
));


