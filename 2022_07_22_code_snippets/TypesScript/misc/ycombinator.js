// https://www.youtube.com/watch?v=9T8A89jgeTI
// Essentials: Functional Programming's Y Combinator - Computerphile
//-  nizej to krotnie nagranie zaimplementowane jest w JS

// przyklad funkcji rekurencyjnej np factorial
var fac = n => n === 1 ? 1 : n * fac(n - 1);
fac(1); // 1
fac(2); // 2
fac(3); // 6
fac(4); // 24
fac(5); // 120


// wywolanie kod po prawej stronie "=" nigdy sie nie skonczy
var loop = (x => x(x))(x => x(x))


// przyklad pomocniczej ogolnej funkcji realizujacej rekurencje
// var rec = f => f(rec(f))

// ale cos takiego mozna zapisac "na papierze" lub w haskell bo jest leniwy,  js to sie zapeli i np musimy opakowac w thunka
var rec = f => f(() => rec(f));

// o tutaj faktycznie ta dziala dziala
var fac2 = rec(f => n => n === 1 ? 1 : n * f()(n - 1));
fac2(4); // 24 :)

// tylko wyzej funkcja rec wola sama sobie, czyli wykorzystuje rekurancje ktora jest w JS
// y-combinator pozwala zaimplementowac sprytnie rekurencje 
// var rec = f => (x => f(x(x))) (x => f(x(x)))


// ale tutaj podobnie ten kod wyzej sie zapetli i w JS trzeba zapisac z thunk-ami cos takieg
// ( wywolujac funkcje trzeba kazdy argument opakowac w funkcje, dodatkowo korzystaja z argumentu trzeba go wywolac boto funkcja jest)
var rec_ = f => (x => f()(() => x()(() => x())))(() => x => f()(() => x()(() => x())));


// i niesamowite, to dziala poprawnie
var fac3 = rec_(() => f => n => n === 1 ? 1 : n * f()(n - 1))
fac3(4); // 24 :)

// *********************
// wyjasnienie przeslane na skype


// to jest definicja y-combinatora
// @Michał mowil nawet w pokoju wczoraj co to jest :)
// generalnie chodzi o to ...
// ... no ze ja sie nie znam ...
// wiec @Sebastian musi nas wprowadzic w "lambda calculus" (rachunek lamnda)
// to jest cos takiego, ze mamy taki sposob "zapisu funkcji" np "\x.x+1" oznacza "x => x +1"
// kazda funkcja moze miec tylko jeden argument, kazda funkcje mozna wywylac (aplikowac argumenty)
// np "\x.\y.x+y+10" to "x => y => x + y + 10"
// czyli tutaj jest przyklad funkcji z wieloma argumentami, a wiec tutaj mamy currying funkcji (funkcja ktora zwraca nowa funkcje jesli chce wspierac wiecej jak 1 argument)
// no wiec za pomoca czegos tak prostego (definicja funkcja i mozliwosc jej aplikowania do argumentow) mozna zbudowac "wszystko"
// a matematyk Haskell Curry wpadl na pomysl za pomoce tego zapisac rekurencje ... w czyms co nie ma rekurencji tzn my tutaj nie nazywamy funkcji do ktorej mozemy potem ponownie odwalac sie po nazwie w jej definicji
// i to jest wlasnie wyzej napisany y-combinator
// i moze nie trzeba tego rozumiec w 100% (ja nie rozumiem), ale zastanawialem sie czy mozna sobie to zapisac w js tak aby dzialo
// cala idea jest taka ze ten zapis definiuje jak funkcje zapocnicza "rec" ktora zapewnia nam rekurencje ktora mozemy potem wszedzie wykorzystac gdzie ... potrzebujemy rekurencji i przykladowo
// to zwyklad definicji funkcji rekurencyjnej factorial
// var fac = n => n === 1 ? 1 : n * fac(n - 1);

// fac(1); // 1
// fac(2); // 2
// fac(3); // 3
// fac(4); // 24
// fac(5); // 120
// to kazdy chyba rozumie ?
// no i teraz mozna sobie zdefiniowac pomocnicza fukcje "rec" w taki sposob
// var rec = f => f( rec(f) )

// (ale uwaga w js to nie dziala, bo js nie jest leniwy i trzeba zaraz zrobic pewna sztuczke, ale to za moment)
// no i dzieki tej funkcji "rec" mozna teraz zdefinkowac factorial
// var fac2 = rec(f => n => n === 1 ? 1 : n * (n - 1));

// fac2(4); // 24 :)

// czyli cala ide jest taka ze jak chcemy napisac dowolna funkcje rekurencyjna to musimy ja opakowac w "rec(f => ...)" a ten parametr f to uchwyt do samej siebie jakby
// czy to jest zrozumiale mniej wiecej ?
// no wiec tutaj niestety taka definicja funkcji "rec" kompletnie sie zapetla poniewaz js nie jest leniwy i wywolanie tej funkcji "f => f( rec(f) )" z dowolnym parametrem sie zapetli poniewaz w wiekszosci jezykow jak chcecie wywolac funkcje to najpierw trzeba wyliczyc wartosci wszystkich argumentow
// np wywolujac"add(1+1, 10*13)" to trzeba najpierw wyliczyc te obliczenia, ale
// mozna stworzy tak zwany "tunk" czyli opakowac cos w funkcje ktora bedzie wolana na zyczenie czyli "add( () => 1+1, () => 10*13)"
// no i sama funkcja wtedy musi byc zdefiniowana tak "var add = (a,b) => a() + b();"
// czy to jest zrozumiale ?
// no wiec mozemy zmienic teraz definicje funkcje rec aby sie nie zapetlala i wyglada tak
// var rec = f => f(() => rec(f))
// wtedy factorial wyglada tak
// var fac2 = rec(f => n => n === 1 ? 1 : n * f()(n - 1));
// i faktycznie to dziala
// fac2(4); // 24 :)
// no wlasnie ale ...
// tutaj tak na prawde definicja "rec" jest rekurencyjna poniewaz jej implementacja korzysta z "rec"
// a y-combinator pozwala zapisac funkcje "rec" aby rekurencji nie bylo
// zgodnie z definicja ze zdjecia y-combintora powinnismy zapisac cos takiego
// var rec = f => (x => f(x(x)))(x => f(x(x)))
// ale to nie zadziala poniewaz ... js nie jest leniwy :)
// no wiec
// uwaga
// musimy napisac cos takiego
// var rec_ = f => (x => f()(() => x()(() => x())))(() => x => f()(() => x()(() => x())));
// i cos takiego zadziala :D
// var fac3 = rec_(() => f => n => n === 1 ? 1 : n * f()(n - 1))
// fac3(4); // 24 :)
// nawet chcialem napisac sobie funcje pomoczna ktora przyjmuje cala definicje funkcji jako string
// i automatycznie zwraca nowa definicje funkcje ale juz z tymi nawiasami/funkcjami/thunkami
// ale odpuscilem
// czy rozumiecie mniej wiecej o co chodzilo w ogole ?
// no i ogolnie dosyc ciekawe ze funkcja moze przyjmowac 2 argumenty i my zapisujemy jako "f => n => ..." (a powinno byc np "(f,n) => ..." ) i to dziala wszystko poprawnie
// np przemyslcie sobie w glowie jak to moze dzialac
// var rec = f => f(() => rec(f));

// var fac2 = rec(f => n => n === 1 ? 1 : n * f()(n - 1));

// fac2(4); // 24 