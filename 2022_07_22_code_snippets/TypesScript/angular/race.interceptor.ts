import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpContextToken } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

export const CANCELLATION_KEY_HTTPCONTEXT_TOKEN = new HttpContextToken<CancellationContext | undefined>(() => undefined);

export type CancellationContext =  string | { key: string; defaultValue?: any; };

@Injectable()
export class RaceInterceptor implements HttpInterceptor {

  private cancellationKyes$ = new Subject<string>();

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const cancellationContext = req.context.get(CANCELLATION_KEY_HTTPCONTEXT_TOKEN); // returns default value if not exists

    if (typeof cancellationContext !== "undefined") {
      const cancellationKey = typeof cancellationContext === "string"  ? cancellationContext : cancellationContext.key;

      this.cancellationKyes$.next(cancellationKey);
      return next.handle(req).pipe(takeUntil(this.cancellationKyes$.pipe(filter(ck => ck === cancellationKey))));
    }

    return next.handle(req);
  }
}
