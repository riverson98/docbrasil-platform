import { HttpErrorResponse, HttpHandler, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { inject } from '@angular/core';
import { catchError, filter, Observable, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';


let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req:HttpRequest<any>, next:HttpHandlerFn): Observable<any> => {
  const authService = inject(AuthService);
  const token = localStorage.getItem('token');

  const authReq = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
                        : req;
                        
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if(error.status === 401){
        return handle401Error(req, next, authService);
      }

      return throwError(() => error);
    })
  );
};

const handle401Error = (request: HttpRequest<any>, next: HttpHandlerFn, authService: AuthService): Observable<any> => {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    return authService.refreshToken().pipe(
      switchMap((token: any) => {
        isRefreshing = false;
        refreshTokenSubject.next(token.accessToken);
        return next(addTokenHeader(request, token.accessToken));
      }),
      catchError((err) => {
        isRefreshing = false;
        authService.logout();
        return throwError(() => err);
      })
    );
  } else {
    return refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap(token => next(addTokenHeader(request, token as string)))
    );
  }
};

const addTokenHeader = (req: HttpRequest<any>, token: string) => {
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
};

