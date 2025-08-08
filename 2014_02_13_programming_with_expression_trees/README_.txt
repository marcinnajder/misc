

) No coz... 

w zasadzie nie mam co komentowac :)

zachecam Cie gorąco do wyslania swojego rozwiazania do ziomkow od driver'a tak jak chciales. Podziekowania dla inicjatora pomyslu oczywiscie sie naleza, ale jego implementacja jest nieakceptowalna.

PS. dzieki za linka do pdfmake

pozdrawiam
-- 


<<< db.zipcodes.aggregate( { $group:
                         { _id: { state: "$state", city: "$city" },
                           pop: { $sum: "$pop" } } },
                       { $sort: { pop: 1 } },
                       { $group:
                         { _id : "$_id.state",
                           biggestCity:  { $last: "$_id.city" },
                           biggestPop:   { $last: "$pop" },
                           smallestCity: { $first: "$_id.city" },
                           smallestPop:  { $first: "$pop" } } },

                       // the following $project is optional, and
                       // modifies the output format.

                       { $project:
                         { _id: 0,
                           state: "$_id",
                           biggestCity:  { name: "$biggestCity",  pop: "$biggestPop" },
                           smallestCity: { name: "$smallestCity", pop: "$smallestPop" } } } )[13 lutego 2014 12:26] Marcin Najder: 

<<< kod Michala:
[AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class ThirdTimeGroupedZip
  {
      public string Id { get; set; }
      public string BiggestCity { get; set; }
      public long BiggestPop { get; set; }
      public string SmallestCity { get; set; }
      public long SmallestPop { get; set; }
  }

  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class StateStats
  {
      public string State { get; set; }
      public CityPopPair BiggestCity { get; set; }
      public CityPopPair SmallestCity { get; set; }
  }

  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class CityPopPair
  {
      public string Name { get; set; }
      public long Pop { get; set; }
  }

var result = Collection.Aggregate(q => q.Group<FirstTimeGroupedZip>(
                    s => s.By<StateCityPair>(d => d.Map()).Computed(z => z.Pop, e => e.Sum(f => f.Pop)))
                .Sort(s => s.Pop)
                .Group<ThirdTimeGroupedZip>(d => d.By(s => s.Id.State)
                    .Computed(e => e.BiggestCity, f => f.Last(r => r.Id.City))
                    .Computed(e => e.BiggestPop, f => f.Last(r => r.Pop))
                    .Computed(e => e.SmallestCity, f => f.First(r => r.Id.City))
                    .Computed(e => e.SmallestPop, f => f.First(r => r.Pop)))
                .Project<StateStats>(
                    f => f.Unset(b => b.Id).Computed(d => d.State, e => e.Id)
                    .Complex(s => s.BiggestCity,
                        e => e.Computed(x => x.Name, g => g.BiggestCity)
                              .Computed(x => x.Pop, g => g.BiggestPop))
                    .Complex(s => s.SmallestCity,
                        e => e.Computed(x => x.Name, g => g.SmallestCity)
                              .Computed(x => x.Pop, g => g.SmallestPop))
                ));
moj kod ;) :
         var typedResult = CreateCollection<ZipCode>("zips", "zips").AggregateTyped(q => q
                    .Group((x, o) => new
                    {
                        _id = new {x.State, x.City},
                        pop = o.Sum(x.Pop)
                    })
                    .Sort(x => x.pop)
                    .Group((x, o) => new
                    {
                        _id = x._id.State,
                        biggestCity = o.Last(x._id.City),
                        biggestPop = o.Last(x.pop),
                        smallestCity = o.First(x._id.City),
                        smallestPop = o.First(x.pop),
                    })
                    .Project(x => new
                    {
                        state = x._id,
                        biggestCity = new {name = x.biggestCity, pop = x.biggestPop},
                        smallestCity = new {name = x.smallestCity, pop = x.smallestPop},
                    })
                    .Log()
                    );

pozdrawiam,
--
Lukasz
...

----

hej

wracalismy z Piotrkiem z pracy razem i chwile rozmawialismy o Aggregation Framework czyli mozliwosci wykonywania bardziej skomplikowanych zapytan do MongoDB (w szczegolnosci wlasnie agregacji czyli np grupowania)




{
  "_id": "10280",
  "city": "NEW YORK",
  "state": "NY",
  "pop": 5574,
  "loc": [
    -74.016323,
    40.710537
  ]
}


db.zipcodes.aggregate( { $group:
                         { _id: { state: "$state", city: "$city" },
                           pop: { $sum: "$pop" } } },
                       { $sort: { pop: 1 } },
                       { $group:
                         { _id : "$_id.state",
                           biggestCity:  { $last: "$_id.city" },
                           biggestPop:   { $last: "$pop" },
                           smallestCity: { $first: "$_id.city" },
                           smallestPop:  { $first: "$pop" } } },
 
                       // the following $project is optional, and
                       // modifies the output format.
 
                       { $project:
                         { _id: 0,
                           state: "$_id",
                           biggestCity:  { name: "$biggestCity",  pop: "$biggestPop" },
                           smallestCity: { name: "$smallestCity", pop: "$smallestPop" } } } )

  var typedResult = CreateCollection<ZipCode>("zips", "zips").AggregateTyped(q => q
                    .Group((x, o) => new
                    {
                        _id = new {x.State, x.City},
                        pop = o.Sum(x.Pop)
                    })
                    .Sort(x => x.pop)
                    .Group((x, o) => new
                    {
                        _id = x._id.State,
                        biggestCity = o.Last(x._id.City),
                        biggestPop = o.Last(x.pop),
                        smallestCity = o.First(x._id.City),
                        smallestPop = o.First(x.pop),
                    })
                    .Project(x => new
                    {
                        state = x._id,
                        biggestCity = new {name = x.biggestCity, pop = x.biggestPop},
                        smallestCity = new {name = x.smallestCity, pop = x.smallestPop},
                    })
                    );
 
Oczywiście kodu jest mnie ale także:
- zapis praktycznie jak natywne API, przez co prościej jest komuś wejść w temat przeglądając choćby sample ze stronki
- u mnie rezultat jest typowany, u Michała BsonObject[] (przynajmniej tak kiedyś było)
- składanie warunku „where” robi się ręcznie budując obiekt „query” z mongo, u mnie pisze się zwykłą lambdę (jak provider LINQ) więc tutaj różnica w ilości kodu jest jeszcze większa
- mniej ważne i do sprawdzenia, ale patrząc jak korzysta się z jego biblioteki, to kodu samej biblioteki u mnie będzie znacznie mniej


var aggregate = collection.Aggregate()
                .Match(new BsonDocument { { "deviceid", deviceid } })
                .Sort( new BsonDocument { { "timestamp", -1} })
                .Group(new BsonDocument { { "groupkey", "$group" }, { "latestvalue", "$first.value" } });






----


Właśnie trafiłem na kod korzystający aggregation framework w .net Michała Kłusaka, bo coś mnie pytał Przemek i ten framework Michała jest mega przegadany. Michał albo tego nie widzi do końca (może za szybko mu to pokazywałem i był generalnie na nie wtedy), albo celowo uparł się. I teraz nie wiem, może jeszcze z nim zagadam bo to bez sensu tyle kodu pisać, a kod jak rozumiem jest wspólny tzn. nawet jeśli on nie chce mojego używać to może inny mogą wybrać (tutaj już nie omieszkałem Kalince, Marcinowi  i Przemkowi pokazać różnicę). Już pewnie Ci pisałem ale zobacz (przykład ze stronki mongo):
 
Model danych:
 
{
  "_id": "10280",
  "city": "NEW YORK",
  "state": "NY",
  "pop": 5574,
  "loc": [
    -74.016323,
    40.710537
  ]
}
 
Zapytanie w API mongowym (praktycznie identycznie to wygląda w .net ale korzysta się z BsonObject więc nie ma typowalności)
 
db.zipcodes.aggregate( { $group:
                         { _id: { state: "$state", city: "$city" },
                           pop: { $sum: "$pop" } } },
                       { $sort: { pop: 1 } },
                       { $group:
                         { _id : "$_id.state",
                           biggestCity:  { $last: "$_id.city" },
                           biggestPop:   { $last: "$pop" },
                           smallestCity: { $first: "$_id.city" },
                           smallestPop:  { $first: "$pop" } } },
 
                       // the following $project is optional, and
                       // modifies the output format.
 
                       { $project:
                         { _id: 0,
                           state: "$_id",
                           biggestCity:  { name: "$biggestCity",  pop: "$biggestPop" },
                           smallestCity: { name: "$smallestCity", pop: "$smallestPop" } } } )
 
 
U Michała:
 
var result = Collection.Aggregate(q => q.Group<FirstTimeGroupedZip>(
                    s => s.By<StateCityPair>(d => d.Map()).Computed(z => z.Pop, e => e.Sum(f => f.Pop)))
                .Sort(s => s.Pop)
                .Group<ThirdTimeGroupedZip>(d => d.By(s => s.Id.State)
                    .Computed(e => e.BiggestCity, f => f.Last(r => r.Id.City))
                    .Computed(e => e.BiggestPop, f => f.Last(r => r.Pop))
                    .Computed(e => e.SmallestCity, f => f.First(r => r.Id.City))
                    .Computed(e => e.SmallestPop, f => f.First(r => r.Pop)))
                .Project<StateStats>(
                    f => f.Unset(b => b.Id).Computed(d => d.State, e => e.Id)
                    .Complex(s => s.BiggestCity,
                        e => e.Computed(x => x.Name, g => g.BiggestCity)
                              .Computed(x => x.Pop, g => g.BiggestPop))
                    .Complex(s => s.SmallestCity,
                        e => e.Computed(x => x.Name, g => g.SmallestCity)
                              .Computed(x => x.Pop, g => g.SmallestPop))
                ));
[AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class ThirdTimeGroupedZip
  {
      public string Id { get; set; }
      public string BiggestCity { get; set; }
      public long BiggestPop { get; set; }
      public string SmallestCity { get; set; }
      public long SmallestPop { get; set; }
  }
 
  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
 public class StateStats
  {
      public string State { get; set; }
      public CityPopPair BiggestCity { get; set; }
      public CityPopPair SmallestCity { get; set; }
  }
 
  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class CityPopPair
  {
      public string Name { get; set; }
      public long Pop { get; set; }
  }
 
U mnie:
 
  var typedResult = CreateCollection<ZipCode>("zips", "zips").AggregateTyped(q => q
                    .Group((x, o) => new
                    {
                        _id = new {x.State, x.City},
                        pop = o.Sum(x.Pop)
                    })
                    .Sort(x => x.pop)
                    .Group((x, o) => new
                    {
                        _id = x._id.State,
                        biggestCity = o.Last(x._id.City),
                        biggestPop = o.Last(x.pop),
                        smallestCity = o.First(x._id.City),
                        smallestPop = o.First(x.pop),
                    })
                    .Project(x => new
                    {
                        state = x._id,
                        biggestCity = new {name = x.biggestCity, pop = x.biggestPop},
                        smallestCity = new {name = x.smallestCity, pop = x.smallestPop},
                    })
                    );
 
Oczywiście kodu jest mnie ale także:
- zapis praktycznie jak natywne API, przez co prościej jest komuś wejść w temat przeglądając choćby sample ze stronki
- u mnie rezultat jest typowany, u Michała BsonObject[] (przynajmniej tak kiedyś było)
- składanie warunku „where” robi się ręcznie budując obiekt „query” z mongo, u mnie pisze się zwykłą lambdę (jak provider LINQ) więc tutaj różnica w ilości kodu jest jeszcze większa
- mniej ważne i do sprawdzenia, ale patrząc jak korzysta się z jego biblioteki, to kodu samej biblioteki u mnie będzie znacznie mniej
 
Zagadać do niego jeszcze czy odpuścić sobie temat ??? wiem że z jednej strony chwale swoje, ale kurde raczej na jego miejscu sam przyznałbym że prościej i wziąłbym lepsze rozwiązanie
 
Pozdrawiam,
marcin


 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- 
 
 
 
 
 
 
 
Model danych:
 
{
  "_id": "10280",
  "city": "NEW YORK",
  "state": "NY",
  "pop": 5574,
  "loc": [
    -74.016323,
    40.710537
  ]
}
 
 
Przykładowy sample z dokumentacji MongoDB:
 
db.zipcodes.aggregate( { $group:
                         { _id: { state: "$state", city: "$city" },
                           pop: { $sum: "$pop" } } },
                       { $sort: { pop: 1 } },
                       { $group:
                         { _id : "$_id.state",
                           biggestCity:  { $last: "$_id.city" },
                           biggestPop:   { $last: "$pop" },
                           smallestCity: { $first: "$_id.city" },
                           smallestPop:  { $first: "$pop" } } },
 
                       // the following $project is optional, and
                       // modifies the output format.
 
                       { $project:
                         { _id: 0,
                           state: "$_id",
                           biggestCity:  { name: "$biggestCity",  pop: "$biggestPop" },
                           smallestCity: { name: "$smallestCity", pop: "$smallestPop" } } } )
 
 
Kod Michała:
 
[AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class ThirdTimeGroupedZip
  {
      public string Id { get; set; }
      public string BiggestCity { get; set; }
      public long BiggestPop { get; set; }
      public string SmallestCity { get; set; }
      public long SmallestPop { get; set; }
  }
 
  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
 public class StateStats
  {
      public string State { get; set; }
      public CityPopPair BiggestCity { get; set; }
      public CityPopPair SmallestCity { get; set; }
  }
 
  [AggregationOptions(CamelCaseConvention = true, IdProperty = "Id")]
  public class CityPopPair
  {
      public string Name { get; set; }
      public long Pop { get; set; }
  }
 
var result = Collection.Aggregate(q => q.Group<FirstTimeGroupedZip>(
                    s => s.By<StateCityPair>(d => d.Map()).Computed(z => z.Pop, e => e.Sum(f => f.Pop)))
                .Sort(s => s.Pop)
                .Group<ThirdTimeGroupedZip>(d => d.By(s => s.Id.State)
                    .Computed(e => e.BiggestCity, f => f.Last(r => r.Id.City))
                    .Computed(e => e.BiggestPop, f => f.Last(r => r.Pop))
                    .Computed(e => e.SmallestCity, f => f.First(r => r.Id.City))
                    .Computed(e => e.SmallestPop, f => f.First(r => r.Pop)))
                .Project<StateStats>(
                    f => f.Unset(b => b.Id).Computed(d => d.State, e => e.Id)
                    .Complex(s => s.BiggestCity,
                        e => e.Computed(x => x.Name, g => g.BiggestCity)
                              .Computed(x => x.Pop, g => g.BiggestPop))
                    .Complex(s => s.SmallestCity,
                        e => e.Computed(x => x.Name, g => g.SmallestCity)
                              .Computed(x => x.Pop, g => g.SmallestPop))
                ));
 
 
Kod mój ;) :
 
  var typedResult = CreateCollection<ZipCode>("zips", "zips").AggregateTyped(q => q
                    .Group((x, o) => new
                    {
                        _id = new {x.State, x.City},
                        pop = o.Sum(x.Pop)
                    })
                    .Sort(x => x.pop)
                    .Group((x, o) => new
                    {
                        _id = x._id.State,
                        biggestCity = o.Last(x._id.City),
                        biggestPop = o.Last(x.pop),
                        smallestCity = o.First(x._id.City),
                        smallestPop = o.First(x.pop),
                    })
                    .Project(x => new
                    {
                        state = x._id,
                        biggestCity = new {name = x.biggestCity, pop = x.biggestPop},
                        smallestCity = new {name = x.smallestCity, pop = x.smallestPop},
                    })
                    .Log()
                    );
