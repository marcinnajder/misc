import { Option, some, none, pipe, error, ok, Result, matchValue, isUnion } from "powerfp";

// domain
type Id = string;
type Patient = { id: Id; name: string; email: string; }
type PatientRegistrationInfoDto =
    | { type: "Anonymous"; email: string; }
    | { type: "AnonymousWithCreatingNewAccount"; email: string; }
    | { type: "LoggedIn"; patientId: Id; }
    | { type: "LoggedInWithUpdatingEmail"; patientId: Id; email: string; }
type RegistrationError = "SlotNotFound" | "SlotServiceNotFound" | "PatientNotFound"

// services
interface ILogger {
    info(message: string): void;
    error(message: string): void;
}

export interface IPatientRepository {
    addPatient(patient: Patient): string;
    getPatientById(id: Id): Option<Patient>;
    updatePatient(patient: Patient): void;
}

export interface IEmailService {
    sendEmail(content: string, email: string): void;
}


interface ILoggerP {
    logger: ILogger
}
interface IPatientRepositoryP {
    patientRepository: IPatientRepository;
}
interface IEmailServiceP {
    emailService: IEmailService;
}


// scenarios

function getPatientById(env: IPatientRepositoryP, patientId: Id): Result<Patient, RegistrationError> {
    const patient = env.patientRepository.getPatientById(patientId);
    switch (patient.type) {
        case "some": return ok(patient.value);
        case "none": return error("PatientNotFound");
    }
}


type FirstArg<T extends (...args: any) => any> = T extends (arg1: infer A, ...args: infer P) => infer R ? A : any;
// type FirstArg<T extends (...args: any) => any> = Parameters<T>[0];

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

///


type SkipFirst<T extends (...args: any) => any> = T extends (arg1: any, ...args: infer P) => infer R ? (...args: P) => R : any;


function addOrUpdatePatient2(env: { getPatientById: SkipFirst<typeof getPatientById> } & IPatientRepositoryP & ILoggerP,
    patient: PatientRegistrationInfoDto): Result<[Option<Id>, string], RegistrationError> {
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
