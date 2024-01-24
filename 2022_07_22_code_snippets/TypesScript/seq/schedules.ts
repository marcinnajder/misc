import * as dayjs from 'dayjs';
import * as isSameOrBefore from 'dayjs/plugin/isSameOrBefore';
import * as customParseFormat from 'dayjs/plugin/customParseFormat';
import { pipe, flatmap, concat, map, toarray, filter, groupby, take } from "powerseq";

dayjs.extend(isSameOrBefore);
dayjs.extend(customParseFormat);


const daysOfWeek = ["sun", "mon", "tue", "wed", "thu", "fri", "sat"] as const;
type DayOfWeek = typeof daysOfWeek[number];

interface AvailableTime {
    dayOfWeek: DayOfWeek;
    startTime: string;
    endTime: string;
}

interface UnavailableTime {
    dateFrom: string;
    dateTo: string;
}

interface Schedule {
    _id: string;
    doctorId: string;
    serviceIds: string[];
    unitId: string;
    validFrom?: string;
    availableTime: AvailableTime[];
    vacations?: UnavailableTime[];
    slotDuration: number;
}


interface Slot {
    serviceId: string;
    doctorId: string;
    unitId: string;
    date: string;
    fromTime: string;
    toTime: string;
}



//const schedulesDb = [
const schedulesDb: Schedule[] = [
    {
        _id: "1", slotDuration: 30, doctorId: "doc1", unitId: "uni1", serviceIds: ["ser1", "ser2"], validFrom: "2022-09-05",
        availableTime: [
            { dayOfWeek: "mon", startTime: "09:00:00", endTime: "12:00:00" },
            { dayOfWeek: "mon", startTime: "13:00:00", endTime: "15:00:00" },
            { dayOfWeek: "wed", startTime: "15:00:00", endTime: "16:00:00" },
            { dayOfWeek: "fri", startTime: "15:00:00", endTime: "16:00:00" },
        ]
    },
    {
        _id: "2", slotDuration: 30, doctorId: "doc1", unitId: "uni2", serviceIds: ["ser2"],
        availableTime: [
            { dayOfWeek: "sun", startTime: "10:00:00", endTime: "12:00:00" },
        ]
    },
    {
        _id: "3", slotDuration: 30, doctorId: "doc2", unitId: "uni1", serviceIds: ["ser1"], validFrom: "2022-09-07",
        availableTime: [
            { dayOfWeek: "mon", startTime: "09:00:00", endTime: "12:00:00" },
            { dayOfWeek: "thu", startTime: "09:00:00", endTime: "12:00:00" },
        ]
    },
];

interface FindQuery {
    doctorId?: string;
    serviceId?: string;
    unitId?: string;
    validTo?: string;
}

function findSchedules(schedules: Schedule[], query: FindQuery) {
    return pipe(
        schedules,
        ss => query.doctorId ? filter(ss, s => s.doctorId === query.doctorId) : ss,
        ss => query.unitId ? filter(ss, s => s.unitId === query.unitId) : ss,
        ss => query.serviceId ? filter(ss, s => s.serviceIds.some(ser => ser === query.serviceId)) : ss,
        ss => query.validTo ? filter(ss, s => !s.validFrom || s.validFrom <= query.validTo!) : ss,
        toarray()
    );
}

// function findSchedules2(schedules: Schedule[], query: Query) {
//     return Object.keys(query).length === 0 ? schedules :
//         pipe(
//             schedules,
//             filter(s =>
//                 (!query.doctorId || s.doctorId === query.doctorId) &&
//                 (!query.unitId || s.unitId === query.unitId) &&
//                 (!query.serviceId || s.serviceIds.some(ser => ser === query.serviceId)) &&
//                 (!query.validTo || s.validFrom <= query.validTo!)),
//             toarray()
//         );
// }

// var queries = [
//     {},
//     { doctorId: "doc1" },
//     { unitId: "uni1" },
//     { serviceId: "ser1" },
//     { serviceId: "ser2" },
//     { validTo: "2022-09-06" },
//     { validTo: "2022-09-05" },
//     { doctorId: "doc1", unitId: "uni1" },
// ];

// for (var query of queries) {
//     var result1 = findSchedules(schedulesDb as Schedule[], query);
//     var result2 = findSchedules2(schedulesDb as Schedule[], query);
//     console.log(JSON.stringify(result1) === JSON.stringify(result2), query, result1.map(s => s._id));
// }





interface GenerateQuery {
    fromDate: string;
    toDate: string;
    serviceId?: string;
}


function* generateDays(fromDate: string, toDate: string) {
    const toDateD = new Date(toDate);
    let dM = dayjs(new Date(fromDate));
    for (let d = new Date(fromDate); d <= toDateD; d = (dM = dM.add(1, "day")).toDate()) {
        yield d;
    }
}

function toDateOnlyString(date: Date) {
    return date.toISOString().substring(0, 10);
}

function isDayIncludedInSchedule(schedule: Schedule, date: Date, dayOfWeek: DayOfWeek) {
    const dateString = toDateOnlyString(date);
    if (schedule.vacations && schedule.vacations.some(t => dateString >= t.dateFrom && dateString <= t.dateTo)) {
        return false;
    }
    if (schedule.availableTime && schedule.availableTime.some(t => t.dayOfWeek == dayOfWeek)) {
        return true;
    }
    return false;
}

function* generateTimeSlotsForSchedule(schedule: Schedule, dayOfWeek: DayOfWeek) {
    for (const at of filter(schedule.availableTime, t => t.dayOfWeek == dayOfWeek)) {
        const endTimeM = dayjs(at.endTime, "HH:mm:ss");
        let t = dayjs(at.startTime, "HH:mm:ss");
        let start = t.format("HH:mm").toString(), finish = "";

        while (true) {

            if ((t = t.add(schedule.slotDuration, "minute")).isSameOrBefore(endTimeM)) {
                yield [start, finish = t.format("HH:mm").toString()];
                start = finish;
            }
            else {
                break;
            }
        }
    }
}

function groupSchedulesByDoctors(schedules: Schedule[]): Schedule[][] {
    return pipe(
        schedules,
        filter(s => !!s.validFrom),
        groupby(s => s.doctorId),
        map(([_, items]) => toarray(items).sort(({ validFrom: vf1 }, { validFrom: vf2 }) => vf1 === vf2 ? 0 : vf1! < vf2! ? 1 : -1)),
        toarray()
    );
}

function generateSlots(schedules: Schedule[], query: GenerateQuery): Iterable<Slot> {
    const schedulesWithoutValidFrom = schedules.filter(s => !s.validFrom);
    var schedulesWithValidFrom = groupSchedulesByDoctors(schedules);

    return pipe(
        generateDays(query.fromDate, query.toDate),
        map((d, index) => ({ day: d, dayOfWeek: daysOfWeek[d.getDay()], dayAsString: toDateOnlyString(d), dayNumber: index + 1 })),
        flatmap(
            dd => concat(schedulesWithoutValidFrom, flatmap(schedulesWithValidFrom, ss => pipe(ss, filter(s => s.validFrom! <= dd.dayAsString), take(1)))),
            (dd, s) => ({ dd, s })),
        filter(({ dd, s }) => isDayIncludedInSchedule(s, dd.day, dd.dayOfWeek)),
        flatmap(({ dd, s }) => generateTimeSlotsForSchedule(s, dd.dayOfWeek), ({ dd, s }, [fromTime, toTime]) => ({ dd, s, fromTime, toTime })),
        flatmap(({ s }) => query.serviceId ? [query.serviceId] : s.serviceIds, ({ dd, s, fromTime, toTime }, serviceId) => (<Slot>{
            date: toDateOnlyString(dd.day),
            unitId: s.unitId,
            doctorId: s.doctorId,
            serviceId,
            fromTime,
            toTime
        }))
    );
}


type SearchQuery = Omit<FindQuery, "validTo"> & GenerateQuery;

function searchSlots(schedules: Schedule[], query: SearchQuery) {
    const matchingSchedules = findSchedules(schedules, { ...query, validTo: query.toDate });
    return [...generateSlots(matchingSchedules, query)];
}


var slots = searchSlots(schedulesDb, {
    fromDate: "2022-09-05",
    toDate: "2022-09-11",
    // doctorId: "doc1",
    // unitId: "uni2",
    // serviceId: "ser2"
});

console.log(slots.map(formatSlot));

function formatSlot(slot: Slot) {
    return `${slot.date} ${slot.fromTime}-${slot.toTime} ${slot.doctorId}/${slot.unitId}/${slot.serviceId}`;
}
