import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse, HttpContextToken } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

export interface CacheContext {
  enable: boolean;
  refresh?: boolean;
  invalidate?: boolean;
  /** ms */
  duration?: number;
}

export const CACHE_HTTPCONTEXT_TOKEN = new HttpContextToken<CacheContext>(() => ({ enable: false }));

const emptyResponse = of(new HttpResponse<any>({ body: undefined }));

interface CacheEntry {
  readonly expirationTime: number;
  readonly value$: Observable<HttpEvent<any>>;
}

@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  private entries: Dictionary<CacheEntry> = {};

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const entryKey = req.urlWithParams;
    const cacheContext = req.context.get(CACHE_HTTPCONTEXT_TOKEN); // returns default value if not exists

    if (cacheContext.invalidate) {
      this.deleteEntry(entryKey);
      return emptyResponse.pipe(map(res => res.clone()));
    }

    if (cacheContext.enable) {
      if (req.method !== "GET") {
        throw new HttpErrorResponse({ status: 405, statusText: "Proxy cache is only supported for GET requests" }); //405 Method Not Allowed
      }

      const entry = this.getEntry(entryKey);

      if (entry) {
        if (cacheContext.refresh || (Date.now() > entry.expirationTime)) {
          this.deleteEntry(entryKey);
        } else {
          return entry.value$;
        }
      }

      // dzieki 'refCount: true' odpiecie wszystkich observer-ow przed powrotem odpowiedzi zadania http, anuluje je
      const value$ = next.handle(req).pipe(shareReplay({ refCount: true }));

      this.setEntry(entryKey, {
        value$,
        expirationTime: cacheContext.duration ? Date.now() + cacheContext.duration : Infinity,
      });

      return value$;
    }

    return next.handle(req); // just call without using cache
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
// -- moge wywolac wiele razy te sama metode proxy (z tym samym adresem URL czy tym samym 'key'), do kazdej przekazac rozne CacheContext,
// ale dopiero momencie pierwszego 'subscribe' ustawiaja sie parametry cache na 'CacheContext' na ktrego sie zapisamy
// --- np 'var obs1 = ...({...,duration : 1000})' i 'var obs2 = ...({...,duration : 4000})', jesli najpierw zapniemy sie na obs2 to czas
// ustawi sie na 4000, nawet jesli potem ktos sie zapnie na obs1 to czas caly czas bedzie dla 4000, nawet jak ktos potem jeszcze wykona
// 'var obs3 = ...({...,duration : 7000})' to dalej czas bedzie 4000, chyba ze ktos wywola 'var obs3 = ...({...,duration : 7000, refresh: true})
// i oczywiscie wykona 'subscribe', wtedy poleci nowy strzal wyliczajacy nowy cas wygasniecia
// -- czas 'expirationTime' ustawia sie w momencie pierwszego wywolania 'subscribe' i to jeszcze przed samym wywolaniem, mozna niby ustawiac
// po powrocie  odpowiedzia ale to troche komplikuje implementacje, zakladajac ze sam strzal zajmie max kilka sekund, a kesz pewnie bedzie
// ustawiony na kilka minut, to nie ma to tutaj wiekszego znaczenia
// -- czy moze cala obsluge (refresh/duration/invalide) mozna zrobic samymi operatorami rxjs, bez tych ifow, trzeba pewnie dluzej pomyslec jak
// polaczyc ten shareReplay z czyms jeszcze lub uzyc innego podobnego opratora jak connect(...)
