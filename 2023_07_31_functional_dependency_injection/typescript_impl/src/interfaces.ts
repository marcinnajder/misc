import { Option, some, none, pipe } from "powerfp";
import { readFileSync, writeFileSync, existsSync } from "fs";
import { Id, Patient, Payment, PatientRegistrationInfoDto, RegisterVisitRequestDto, RegisterVisitResponseDto, RegistrationError, Visit, Slot } from "./domain";

interface ILogger {
    info(message: string): void;
    error(message: string): void;
}


export interface IScheduleRepository {
    addVisit(visit: Visit): string,
    getSlotById(id: Id): Option<Slot>;
}

export interface IPatientRepository {
    addPatient(patient: Patient): string;
    getPatientById(id: Id): Option<Patient>;
    updatePatient(patient: Patient): void;
}

export interface IPaymentRepository {
    addPayment(payment: Payment): Id;
}

export interface IExternalPaymentService {
    createPayment(visitId: string, price: number, email: string): string;
}

export interface IEmailService {
    sendEmail(content: string, email: string): void;
}




// // providers


export interface ILoggerP {
    logger: ILogger
}

export interface IScheduleRepositoryP {
    scheduleRepository: IScheduleRepository;
}

export interface IPatientRepositoryP {
    patientRepository: IPatientRepository;
}

export interface IPaymentRepositoryP {
    paymentRepository: IPaymentRepository;
}

export interface IExternalPaymentServiceP {
    externalPaymentService: IExternalPaymentService;
}

export interface IEmailServiceP {
    emailService: IEmailService;
}

export type IP<N extends string, T> = {
    [P in N]: T;
}

// // service implementations

class ConsoleLogger implements ILogger {
    info(message: string): void {
        console.log(`${new Date()} info: ${message}`);
    }
    error(message: string): void {
        console.error(`${new Date()} info: ${message}`);
    }
}

class FilePatientRepository implements IPatientRepository {
    constructor(private logger: ILogger) { }
    addPatient(patient: Patient): Id {
        const id = new Date().getTime().toString();
        writeFileSync(this.idToFilePath(id), JSON.stringify({ ...patient, id }))
        return id;
    }
    getPatientById(id: string): Option<Patient> {
        const filePath = this.idToFilePath(id);
        return existsSync(filePath) ? some(JSON.parse(readFileSync(filePath, "utf-8"))) : none;
    }
    updatePatient(patient: Patient): void {
        throw new Error("Method not implemented.");
    }
    private idToFilePath(id: Id) {
        return `${id}.txt`;
    }
}


// var consoleLogger = new ConsoleLogger();
// var filePatientRepository: IPatientRepository = new FilePatientRepository(consoleLogger);
// var patientId = filePatientRepository.AddPatient({ id: "", name: "marcin najder", email: "marcin.najder@gmail.com" })
// var patient = filePatientRepository.GetPatientById(patientId);
// console.log(patient);





// // in-memory implementations

type Dictionary<T> = { [id: string]: T }

class InMemoryStorage<T extends { id: Id }>{
    data: Dictionary<T> = {};
    constructor(private items: Iterable<T>) {
        for (const item of items) {
            this.data[item.id] = item;
        }
    }
    add(value: T) {
        const id = this.generateId();
        this.data[id] = { ...value, id };
        return id;
    }
    tryGetById(id: Id): Option<T> {
        return id in this.data ? some(this.data[id]) : none;
    }
    deleteById(id: Id) {
        delete this.data[id];
    }
    update(value: T) {
        if (value.id in this.data) {
            this.data[value.id] = value;
        }
    }
    private generateId(): Id {
        return Object.keys(this.data).map(id => parseInt(id)).reduce((a, b) => Math.max(a, b), 0) + 1 + "";
    }
}

// var inMemoryStorage = new InMemoryStorage<Patient>([{ id: "1", name: "marcin najder", email: "marcin.najder@gmail.com" }]);
// inMemoryStorage.add({ id: "", name: "marcin najder 2", email: "marcin.najder.2@gmail.com" })
// inMemoryStorage.add({ id: "", name: "marcin najder 2", email: "marcin.najder.2@gmail.com" })
// console.log(inMemoryStorage.tryGetById("1"));
// console.log(inMemoryStorage.Data);




export class InMemoryLogger implements ILogger {
    enties: { time: Date; isError: boolean; message: string; }[] = [];
    info(message: string): void {
        this.enties.push({ time: new Date(), isError: false, message });
    }
    error(message: string): void {
        this.enties.push({ time: new Date(), isError: true, message });
    }

}

export class InMemoryPatientRepository implements IPatientRepository {
    store: InMemoryStorage<Patient>;
    constructor(items: Iterable<Patient>) {
        this.store = new InMemoryStorage<Patient>(items);
    }
    addPatient(patient: Patient): Id {
        return this.store.add(patient);
    }
    getPatientById(id: string): Option<Patient> {
        return this.store.tryGetById(id);
    }
    updatePatient(patient: Patient): void {
        this.store.update(patient);
    }
}

export class InMemoryScheduleRepository implements IScheduleRepository {
    slotsStore: InMemoryStorage<Slot>;
    visitStore: InMemoryStorage<Visit>;
    constructor(slots: Iterable<Slot>, visits: Iterable<Visit>) {
        this.slotsStore = new InMemoryStorage(slots);
        this.visitStore = new InMemoryStorage(visits);
    }
    addVisit(visit: Visit): Id {
        return this.visitStore.add(visit);
    }
    getSlotById(id: string): Option<Slot> {
        return this.slotsStore.tryGetById(id);
    }
}

export class InMemoryPaymentRepository implements IPaymentRepository {
    store: InMemoryStorage<Payment>;
    constructor(items: Iterable<Payment>) {
        this.store = new InMemoryStorage(items);
    }
    addPayment(payment: Payment): Id {
        return this.store.add(payment);
    }
}
export class InMemoryExternalPaymentService implements IExternalPaymentService {
    store: { visitId: string; price: number; email: string; }[] = [];
    createPayment(visitId: string, price: number, email: string): string {
        this.store.push({ visitId, price, email });
        return `payment_${this.store.length}`;
    }
}
export class InMemoryEmailService implements IEmailService {
    store: { content: string; email: string }[] = [];
    sendEmail(content: string, email: string): void {
        this.store.push({ content, email });
    }
}



export class Env {
    constructor(
        public logger: ILogger,
        public scheduleRepository: IScheduleRepository,
        public patientRepository: IPatientRepository,
        public paymentRepository: IPaymentRepository,
        public externalPaymentService: IExternalPaymentService,
        public emailService: IEmailService,
    ) { }
}



export function createEnv() {
    return new Env(
        new InMemoryLogger(),
        new InMemoryScheduleRepository([], []),
        new InMemoryPatientRepository([]),
        new InMemoryPaymentRepository([]),
        new InMemoryExternalPaymentService(),
        new InMemoryEmailService()
    );
}

export function createEnvWithSlots() {
    const env = createEnv();
    const scheduleRepository = env.scheduleRepository as InMemoryScheduleRepository;
    const patientRepository = env.patientRepository as InMemoryPatientRepository;
    const slots: Slot[] = [{
        id: "",
        from: new Date(2023, 7, 31, 9, 0, 0),
        to: new Date(2023, 7, 31, 9, 30, 0),
        serviceIds: ["1", "2"],
        unitId: "1",
        price: 0
    },
    {
        id: "",
        from: new Date(2023, 7, 31, 9, 30, 0),
        to: new Date(2023, 7, 31, 10, 0, 0),
        serviceIds: ["1"],
        unitId: "2",
        price: 10
    }];
    for (const slot of slots) {
        scheduleRepository.slotsStore.add(slot);
    }
    const patient = { id: "", name: "marcin najder", email: "marcin.najder@gmail.com" };
    patientRepository.store.add(patient);
    return env;
}
