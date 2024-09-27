import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse, HttpContextToken, HttpEventType } from '@angular/common/http';
import { AsyncSubject, Observable, of, from, defer, empty, timer } from 'rxjs';
import { map, share, shareReplay, take, takeUntil, tap, TapObserver } from 'rxjs/operators';

export type CacheModes = "none" | "use" | "reload" | "clear";
export type CacheContext = CacheModes | { mode: CacheModes; } | { mode: "set-expiration"; expiration: number; };


export const CACHE_HTTPCONTEXT_TOKEN = new HttpContextToken<CacheContext>(() => "none");

const emptyResponse = of(new HttpResponse<any>({ body: undefined }));

interface CacheEntry {
  readonly expirationTime: number;
  readonly value$: Observable<HttpEvent<any>>;
}

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  private entries: Dictionary<CacheEntry> = {};
  private expirations: Dictionary<number> = {};

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const cacheKey = req.urlWithParams;
    const cacheContext = req.context.get(CACHE_HTTPCONTEXT_TOKEN); // returns default value if not exists
    const cacheContextObj: Exclude<CacheContext, CacheModes> = typeof cacheContext === "string" ? { mode: cacheContext } : cacheContext;

    if (cacheContextObj.mode !== "none" && req.method !== "GET") {
      throw new HttpErrorResponse({ status: 405, statusText: "Proxy cache is only supported for GET requests" }); //405 Method Not Allowed
    }

    if (cacheContextObj.mode === "clear") {
      this.deleteEntry(cacheKey);
      return CacheInterceptor.clone(emptyResponse);
    }

    if (cacheContextObj.mode === "set-expiration") {
      this.expirations[cacheKey] = cacheContextObj.expiration;
      return CacheInterceptor.clone(emptyResponse);
    }

    if (cacheContextObj.mode === 'use' || cacheContextObj.mode === 'reload') {
      const entry = this.getEntry(cacheKey);

      if (entry) {
        if (cacheContextObj.mode === 'reload' || (Date.now() > entry.expirationTime)) {
          this.deleteEntry(cacheKey);
        } else {
          return CacheInterceptor.clone(entry.value$);
        }
      }

      // dzieki 'refCount: true' odpiecie wszystkich observer-ow przed powrotem odpowiedzi zadania http, anuluje je
      const value$ = next.handle(req).pipe(shareReplay({ refCount: true }));

      this.setEntry(cacheKey, {
        value$,
        expirationTime: Date.now() + (this.expirations[cacheKey] ?? Infinity),
      });

      return value$;
    }

    return next.handle(req);
  }

  private static clone(obs: Observable<HttpEvent<any>>): Observable<HttpEvent<any>> {
    return obs.pipe(map(event => event instanceof HttpResponse ? event.clone() : event));
  }
  private deleteEntry(key: string) {
    delete this.entries[key];
  }
  private getEntry(key: string) {
    return this.entries[key];
  }
  private setEntry(key: string, entry: CacheEntry) {
    this.entries[key] = entry;
  }
}

// - implementacja powinna wspierac kilka scenariuszy:
// -- mozna wiele razy zapiac sie na starcie, ale strzal na serwer powinien poleciec tylko raz, czyli wspieramy jakby stan 'pending'
// -- mozna anulowac zapiecie sie i jesli wszystkie sie odebna a strzal jeszcze nie wrocil z serwera, to zdanie http powinno byc anulowane
// -- moze wrocic blad i nie powinnismy go keszowac, kolejny strzal powinien poleciec na nowo

// - inne uwagi
// -- interceptor wolant wolany jest dopiero gdy ktos wykonuje 'subscribe', to prowadzana miejscami "dziwne przypadki" opisane nizej,
// chodz przewaznie u nas w kodzie konwertujemy Observable do Promise wiec strzal lecii od razu i te "dziwne przypadki" sa mniej widoczne
// -- operacje "set-expiration/clear" powinny byc od razu subskrybowane tak aby sie od razu wykonaly w miejcu ich wywolania
// (najczesniej korzystamy z Promise, nie Observable, wiec tak sie pewnie stanie)
// -- operacje "set-expiration/clear" nigdy nie strzelaja na serwer, zawsze zwracajac "undefined"
// -- oepracja ""set-expiration" ustawia czas wygasniecie kesza ktory bdzie obowiazywal dopiero od kolejnych subskrybcji
// -- moge wywolac wiele razy te sama metode proxy (z tym samym adresem URL czy tym samym 'key'), do kazdej przekazac rozne CacheContext,
// ale dopiero momencie pierwszego 'subscribe' ustawiaja sie parametry cache na 'CacheContext' na ktrego sie zapisamy
// -- czas 'expirationTime' ustawia sie w momencie pierwszego wywolania 'subscribe' i to jeszcze przed samym wywolaniem, mozna niby ustawiac
// po powrocie  odpowiedzia ale to troche komplikuje implementacje, zakladajac ze sam strzal zajmie max kilka sekund, a kesz pewnie bedzie
// ustawiony na kilka minut, to nie ma to tutaj wiekszego znaczenia
// -- czy moze cala obsluge (refresh/duration/invalide) mozna zrobic samymi operatorami rxjs, bez tych ifow, trzeba pewnie dluzej pomyslec jak
// polaczyc ten shareReplay z czyms jeszcze lub uzyc innego podobnego opratora jak connect(...)
