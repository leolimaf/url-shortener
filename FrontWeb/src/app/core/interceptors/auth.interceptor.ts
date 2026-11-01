import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenStorageService } from '../services/token-storage.service';

const AUTH_URLS = ['/auth/access-token', '/auth/refresh-token', '/auth/user', '/auth/logout'];

function isAuthUrl(url: string): boolean {
  return AUTH_URLS.some((path) => url.includes(path));
}

function withBearer(req: HttpRequest<unknown>, token: string | null): HttpRequest<unknown> {
  if (!token || req.headers.has('Authorization')) return req;
  return req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
}

/** Anexa o Bearer quando ha sessao; em 401 tenta renovar uma vez e repete a chamada. */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const storage = inject(TokenStorageService);

  return next(withBearer(req, storage.accessToken)).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse) || error.status !== 401 || isAuthUrl(req.url)) {
        return throwError(() => error);
      }
      return auth.refresh().pipe(
        switchMap((tokens) => {
          if (!tokens) return throwError(() => error);
          return next(withBearer(req, tokens.accessToken));
        }),
      );
    }),
  );
};
