import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpResponse, HttpErrorResponse, HttpContextToken } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap } from 'rxjs/operators';


export interface CacheContext {
  enable: boolean;
  refresh?: boolean;
  invalidate?: boolean;
  /** ms */
  duration?: number;
}

export const CACHE_HTTPCONTEXT_TOKEN = new HttpContextToken<CacheContext>(() => ({ enable: false }));

interface CacheEntry {
  expirationTime: number;
  value: HttpResponse<any>;
}


@Injectable()
export class CacheInterceptor implements HttpInterceptor {
  private entries: Dictionary<CacheEntry> = {};

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const cacheContext = req.context.get(CACHE_HTTPCONTEXT_TOKEN); // returns default value if not exists

    if (cacheContext.enable) {
      if (req.method !== "GET") {
        throw new HttpErrorResponse({ status: 405, statusText: "Proxy cache is only supported for GET requests" }); //405 Method Not Allowed
      }

      const entry = this.entries[req.urlWithParams];

      if (entry) {
        if (cacheContext.refresh || (Date.now() > entry.expirationTime)) {
          delete this.entries[req.urlWithParams]; // clear cache
        } else if (cacheContext.invalidate) {
          delete this.entries[req.urlWithParams]; // clear cache
          return of(entry.value.clone()); // return from cache
        } else {
          return of(entry.value.clone()); // return from cache
        }
      }
    }

    return next.handle(req).pipe(tap({
      next: (event: HttpEvent<any>) => {
        if (cacheContext.enable && event instanceof HttpResponse) {
          this.entries[req.urlWithParams] = {
            value: event,
            expirationTime: cacheContext.duration ? Date.now() + cacheContext.duration : Infinity,
          };
        }
      }
    }));
  }
}

// - pierwssza implementacja ktora nie spiera stanu 'pending' tzn gdy kesz jest pusty i leci wiele strzalow na starcie, wszystkie poleca na serwer
// - niestety dosyc typowym scenariuszem bylo cos takiego, tzn wchodzimy do aplikacji i wiele niezaleznych kontrolek strzela o te same dane