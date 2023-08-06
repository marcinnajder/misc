import { Option, some, none, pipe, error, ok, Result, matchValue, isUnion } from "powerfp";
import { Id, Patient, Payment, PatientRegistrationInfoDto, RegisterVisitRequestDto, RegisterVisitResponseDto, RegistrationError, Visit, Slot } from "./domain";
import { IEmailServiceP, IExternalPaymentServiceP, ILoggerP, IP, IPatientRepositoryP, IPaymentRepositoryP, IScheduleRepositoryP, InMemoryEmailService, createEnvWithSlots } from "./interfaces";
import * as assert from "assert";

type FirstArg<T extends (...args: any) => any> = T extends (arg1: infer A, ...args: infer P) => infer R ? A : any;


function getPatientById(env: IPatientRepositoryP, patientId: Id): Result<Patient, RegistrationError> {
    const patient = env.patientRepository.getPatientById(patientId);
    switch (patient.type) {
        case "some": return ok(patient.value);
        case "none": return error("PatientNotFound");
    }
}




// type SkipFirst<T extends (...args: any) => any> = T extends (arg1: any, ...args: infer P) => infer R ? (...args: P) => R : any;
// env: { getPatientById: SkipFirst<typeof getPatientById> } & IPatientRepositoryP & ILoggerP,


function addOrUpdatePatient(env: FirstArg<typeof getPatientById> & IPatientRepositoryP & ILoggerP, patient: PatientRegistrationInfoDto)
    : Result<[Option<Id>, string], RegistrationError> {

    switch (patient.type) {
        case "Anonymous": return ok([none, patient.email]);
        case "LoggedIn": return getPatientById(env, patient.patientId).bind(p => ok([some(p.id), p.email]));
        case "AnonymousWithCreatingNewAccount":
            const patientId = env.patientRepository.addPatient({ id: "", name: patient.email, email: patient.email });
            return ok([some(patientId), patient.email]);
        case "LoggedInWithUpdatingEmail":
            return getPatientById(env, patient.patientId)
                .bind(p => {
                    env.patientRepository.updatePatient({ ...p, email: patient.email });
                    env.logger.info(`Patient email has been changed from '${p.email}' to '${patient.email}' `);
                    return ok([some(p.id), patient.email])
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
    env: FirstArg<typeof addOrUpdatePatient> & FirstArg<typeof createPayment> & IScheduleRepositoryP & IEmailServiceP,
    registration: RegisterVisitRequestDto): Result<RegisterVisitResponseDto, RegistrationError> {

    const slotO = env.scheduleRepository.getSlotById(registration.slotId);
    switch (slotO.type) {
        case "none": return error("SlotNotFound");
        case "some":
            if (slotO.value.serviceIds.indexOf(registration.serviceId) === -1) {
                return error("SlotServiceNotFound");
            } else {
                const slot = slotO.value;
                return addOrUpdatePatient(env, registration.patient)
                    .bind(([patientIdO, email]) => {
                        const visit: Visit = { id: "", slotId: slot.id, from: slot.from, to: slot.to, serviceId: registration.serviceId, unitId: slot.unitId, patientId: patientIdO, patientEmail: email }
                        const visitId = env.scheduleRepository.addVisit(visit);
                        if (slot.price === 0) {
                            const emailContent = "New visit created ... ";
                            env.emailService.sendEmail(emailContent, email)
                            return ok({ visitId, externalPaymentId: none }) as Result<RegisterVisitResponseDto, RegistrationError>;
                        } else {
                            const externalPaymentId = createPayment(env, visitId, slot.price, email);
                            const emailContent = `New visit created, please pay in 15 minutes here $'{externalPaymentId}' `;
                            env.emailService.sendEmail(emailContent, email);
                            return ok({ visitId, externalPaymentId: some(externalPaymentId) });
                        }
                    });
            }

    }
}


var appEnv = createEnvWithSlots();

var registration: RegisterVisitRequestDto = { slotId: "1", patient: { type: 'LoggedIn', patientId: "1" }, serviceId: "1" };

type R = ReturnType<typeof registerVisit>;

assert(matchValue(registerVisit(createEnvWithSlots(), { ...registration, slotId: "3" }), error("SlotNotFound") as R));
assert(matchValue(registerVisit(createEnvWithSlots(), { ...registration, serviceId: "3" }), error("SlotServiceNotFound") as R));
assert(matchValue(registerVisit(createEnvWithSlots(), { ...registration, patient: { type: "LoggedIn", patientId: "2" } }), error("PatientNotFound") as R));

assert(matchValue(registerVisit(createEnvWithSlots(), registration), ok({ visitId: "1", externalPaymentId: none }) as R));
assert(matchValue(registerVisit(createEnvWithSlots(), { ...registration, slotId: "2" }), ok({ visitId: "1", externalPaymentId: some("payment_1") }) as R));


appEnv = createEnvWithSlots();
assert(matchValue(
    registerVisit(appEnv, { ...registration, patient: { type: "LoggedInWithUpdatingEmail", patientId: "1", email: "najder.marcin@gmail.com" } }),
    ok({ visitId: "1", externalPaymentId: none }) as R));
assert((appEnv.emailService as InMemoryEmailService).store[0].email === "najder.marcin@gmail.com");
let patient = appEnv.patientRepository.getPatientById("1");
assert(isUnion(patient, "some") && patient.value.email === "najder.marcin@gmail.com");



// testing only subfunctions

type R2 = ReturnType<typeof addOrUpdatePatient>;

assert(matchValue(
    addOrUpdatePatient(createEnvWithSlots(), { type: "Anonymous", email: "marcin.najder@gmail.com" }),
    ok([none, "marcin.najder@gmail.com"]) as R2));

assert(matchValue(
    addOrUpdatePatient(createEnvWithSlots(), { type: "AnonymousWithCreatingNewAccount", email: "marcin.najder@gmail.com" }),
    ok([some("2"), "marcin.najder@gmail.com"]) as R2));

assert(matchValue(
    addOrUpdatePatient(createEnvWithSlots(), { type: "LoggedIn", patientId: "1" }),
    ok([some("1"), "marcin.najder@gmail.com"]) as R2));

assert(matchValue(
    addOrUpdatePatient(createEnvWithSlots(), { type: "LoggedIn", patientId: "2" }),
    error("PatientNotFound") as R2));

appEnv = createEnvWithSlots();
assert(matchValue(
    addOrUpdatePatient(appEnv, { type: "LoggedInWithUpdatingEmail", patientId: "1", email: "najder.marcin@gmail.com" }),
    ok([some("1"), "najder.marcin@gmail.com"]) as R2));

