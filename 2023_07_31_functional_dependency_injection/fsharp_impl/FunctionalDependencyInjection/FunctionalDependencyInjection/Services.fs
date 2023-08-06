module Services

open Domain

open System
open System.IO
open System.Collections.Generic


type ILogger =
    abstract Info: message: string -> unit
    abstract Error: message: string -> unit

type IScheduleRepository =
    abstract GetSlotById: id: Id -> Slot option
    abstract AddVisit: visit: Visit -> Id

type IPatientRepository =
    abstract GetPatientById: id: Id -> Patient option
    abstract AddPatient: patient: Patient -> Id
    abstract UpdatePatient: patient: Patient -> unit

type IPaymentRepository =
    abstract AddPayment: payment: Payment -> Id

type IExternalPaymentService =
    abstract CreatePayment: visitId: Id * price: decimal * email: string -> string

type IEmailService =
    abstract SendEmail: content: string * email: string -> unit


// providers

// type IP<'T> =
//     abstract Service: 'T

type ILoggerP =
    abstract Logger: ILogger

type IScheduleRepositoryP =
    abstract ScheduleRepository: IScheduleRepository

type IPatientRepositoryP =
    abstract PatientRepository: IPatientRepository

type IPaymentRepositoryP =
    abstract PaymentRepository: IPaymentRepository

type IExternalPaymentServiceP =
    abstract ExternalPaymentService: IExternalPaymentService

type IEmailServiceP =
    abstract EmailService: IEmailService


// service implementations

type ConsoleLogger() =
    interface ILogger with
        member this.Info(message) = printfn "%A info: %s " DateTime.Now message
        member this.Error(message) = printfn "%A error: %s " DateTime.Now message



type FilePatientRepository(logger: ILogger) =
    let idToFilePath id = $"{id}.txt"
    let searialize (patient: Patient) = $"{patient.Id}|{patient.Name}|{patient.Email}"
    let deserialize (text: string) : Patient =
        match text.Split('|') with
        | [| id; name; email |] -> { Id = id; Name = name; Email = email }
        | _ -> failwithf "wrong data format for patient: %s" text

    interface IPatientRepository with
        member this.GetPatientById id =
            let filePath = idToFilePath id
            if File.Exists(filePath) then File.ReadAllText(filePath) |> deserialize |> Some else None
        member this.AddPatient patient =
            let id = Guid.NewGuid().ToString()
            let filePath = idToFilePath id
            File.WriteAllText(filePath, (searialize { patient with Id = id }))
            logger.Info("new patient has been created ... ")
            id
        member this.UpdatePatient patient = failwith "not implemented"

// let consoleLogger = ConsoleLogger()
// let filePatientRepository: IPatientRepository = FilePatientRepository(consoleLogger)
// let patientId = filePatientRepository.AddPatient({ Id = ""; Name = "marcin najder"; Email = "marcin.najder@gmail.com" })
// let patient = filePatientRepository.GetPatientById patientId





// in-memory implementations

// type InMemoryStorage<'v when 'v: (member Id: Id)>(items: seq<Id * 'v>, getId: 'v -> Id, setId: 'v -> Id -> 'v) =
type InMemoryStorage<'v>(items: seq<'v>, getId: 'v -> Id, setId: 'v -> Id -> 'v) =
    let data = Dictionary<Id, 'v>()

    let generateId () : Id = if data.Count = 0 then "1" else data.Keys |> Seq.map int |> Seq.max |> (+) 1 |> string

    do items |> Seq.iter (fun v -> data.Add(getId v, v))
    member this.Data = data

    member this.TryGetById id =
        match data.TryGetValue id with
        | false, _ -> None
        | true, value -> Some value

    member this.Add value =
        let id = generateId ()
        data.Add(id, setId value id)
        id

    member this.Update value =
        let id = getId value

        match this.TryGetById id with
        | None -> ()
        | Some _ -> data.[id] <- value

    member this.DeleteById id = data.Remove(id) |> ignore

let inline getId<'v when 'v: (member Id: Id)> (v: 'v) = v.Id

// let inline setId<'v when 'v: (member Id: Id)> (v: 'v) id : Id = { v with Id = idd }
// let patient1 = { Id = "1"; Name = "marcin"; Email = "marcin.najder@gmail.com" }
// let patient2 = { Id = "2"; Name = "lukasz"; Email = "lukasz.najder@gmail.com" }
// let store = InMemoryStorage([ patient1 ], getId, (fun v id -> { v with Id = id }))
// store.Data |> ignore
// store.Add(patient2)
// store.Update({ Id = "1"; Name = "marcin123"; Email = "marcin.najder@gmail.com" })
// store.DeleteById("1")






type InMemoryLogger() =
    let entries = ResizeArray<{| Time: DateTime; IsError: bool; Message: string |}>()
    let addEntry isError message = entries.Add({| Time = DateTime.Now; IsError = isError; Message = message |})
    member this.Store = entries
    interface ILogger with
        member this.Info(message) = addEntry false message
        member this.Error(message) = addEntry true message

type InMemoryPatientRepository(items: seq<Patient>) =
    let store = InMemoryStorage<Patient>(items, getId, (fun v id -> { v with Id = id }))
    member this.Store = store
    interface IPatientRepository with
        member this.GetPatientById id = store.TryGetById id
        member this.AddPatient patient = store.Add patient
        member this.UpdatePatient patient = store.Update patient

type InMemoryScheduleRepository(slots: seq<Slot>, visits: seq<Visit>) =
    let slotsStore = InMemoryStorage<Slot>(slots, getId, (fun v id -> { v with Id = id }))

    let visitsStore = InMemoryStorage<Visit>(visits, getId, (fun v id -> { v with Id = id }))

    member this.SlotsStore = slotsStore
    member this.VisitsStore = visitsStore

    interface IScheduleRepository with
        member this.GetSlotById id = slotsStore.TryGetById id
        member this.AddVisit visit = visitsStore.Add visit

type InMemoryPaymentRepository(items: seq<Payment>) =
    let store = InMemoryStorage<Payment>(items, getId, (fun v id -> { v with Id = id }))
    member this.Store = store

    interface IPaymentRepository with
        member this.AddPayment payment = store.Add payment




type InMemoryExternalPaymentService() =
    let store = ResizeArray<{| VisitId: string; Price: decimal; Email: string |}>()

    member this.Store = store

    interface IExternalPaymentService with
        member this.CreatePayment(visitId, price, email) =
            store.Add {| VisitId = visitId; Price = price; Email = email |}

            $"payment_{store.Count.ToString()}"

type InMemoryEmailService() =
    let store = ResizeArray<{| Content: string; Email: string |}>()
    member this.Store = store

    interface IEmailService with
        member this.SendEmail(content, email) = store.Add {| Content = content; Email = email |}


type Env() =
    let services = Dictionary<Type, obj>()
    member this.getService<'t>() = services.[typeof<'t>] :?> 't
    member this.RegisterService<'t>(service: 't) = services.Add(typeof<'t>, service)

    interface ILoggerP with
        member this.Logger = this.getService<ILogger> ()

    interface IScheduleRepositoryP with
        member this.ScheduleRepository = this.getService<IScheduleRepository> ()

    interface IPatientRepositoryP with
        member this.PatientRepository = this.getService<IPatientRepository> ()

    interface IPaymentRepositoryP with
        member this.PaymentRepository = this.getService<IPaymentRepository> ()

    interface IExternalPaymentServiceP with
        member this.ExternalPaymentService = this.getService<IExternalPaymentService> ()

    interface IEmailServiceP with
        member this.EmailService = this.getService<IEmailService> ()



let createEnv () =
    let env = Env()
    env.RegisterService<ILogger>(InMemoryLogger())
    env.RegisterService<IPaymentRepository>(InMemoryPaymentRepository([]))
    env.RegisterService<IPatientRepository>(InMemoryPatientRepository([]))
    env.RegisterService<IScheduleRepository>(InMemoryScheduleRepository([], []))
    env.RegisterService<IExternalPaymentService>(InMemoryExternalPaymentService())
    env.RegisterService<IEmailService>(InMemoryEmailService())
    env

let createEnvWithSlots () =
    let env = createEnv ()
    let scheduleRepository = env.getService<IScheduleRepository> () :?> InMemoryScheduleRepository
    let patientRepository = env.getService<IPatientRepository> () :?> InMemoryPatientRepository
    let slots =
        [ { Id = ""
            From = DateTime(2023, 7, 31, 9, 0, 0)
            To = DateTime(2023, 7, 31, 9, 30, 0)
            ServiceIds = [| "1"; "2" |]
            UnitId = "1"
            Price = 0m }
          { Id = ""
            From = DateTime(2023, 7, 31, 9, 30, 0)
            To = DateTime(2023, 7, 31, 10, 0, 0)
            ServiceIds = [| "1" |]
            UnitId = "2"
            Price = 10m } ]
    for slot in slots do
        scheduleRepository.SlotsStore.Add slot |> ignore
    let patient = { Id = ""; Name = "marcin najder"; Email = "marcin.najder@gmail.com" }
    patientRepository.Store.Add(patient) |> ignore
    env
