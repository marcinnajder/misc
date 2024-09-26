import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse, HttpContextToken } from '@angular/common/http';
import { AsyncSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

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
  readonly value: AsyncSubject<HttpEvent<any>>;
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
          return entry.value.pipe(map(res => res instanceof HttpResponse ? res.clone() : res));
        }
      }

      const value = new AsyncSubject<HttpEvent<any>>();

      this.setEntry(entryKey, {
        value,
        expirationTime: cacheContext.duration ? Date.now() + cacheContext.duration : Infinity,
      });


      return next.handle(req).pipe(tap(value), tap({ error: () => this.deleteEntry(entryKey) }));
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


// - ta implementacja wspiera juz stan 'pending' ale pojawia sie np problem anulowania subskrybcji 
// - mozemy wykonac kilka razy strzal za pomoca HttpClient, ale dopiero zapiecie sie na Observable powoduje ze wykonuje sie interceptor
// - dla tego samego klucza w keszu (ten sam adres URL), pierwsze wywolanie idzie inna sciezka (wykonuje sie 'next.handle(req).pipe(...'), 
// natomiast kolejne wywolania ida inna sciezka (wywoluje sie 'entry.value.pipe(map(res => res.clone()));')
// - odpiecie sie od pierwszej subskrybcji anuluje faktycznie strzal na serwer, ale odpiecie tych pozostalych nie robi nic, poniewaz
// odpinamy sie zupelnie niezaleznego AsyncSubject
// - powinno to dzialac tak ze gdy wszyscy sie odepna i odpowiedz jeszcze nie wroci, to wtedy zadanie http jest anulowane
// - kolejny problem ze w sumie strzal na serwer generuje 2 zdarzenia (Send i HttpResponse), a my w AsyncSubject zapisujemy jedynie jeden
// ten ostatni, to widac nie jest wielki problem bo "ogolnie to dziala", to mozna pewnie zamienic na ReplaySubject i ten problem zostanie akurat naprawiony
// - generalnie byc moze tutaj jest wiecej problemow ... ja po prostu widzac to co juz wyszlo pisalem nastepna implementacje