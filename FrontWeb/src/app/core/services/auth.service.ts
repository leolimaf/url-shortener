import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenStorageService } from './token-storage.service';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  birthDate?: string | null;
  phone?: string | null;
  email: string;
  password: string;
}

export interface TokenPair {
  accessToken: string;
  refreshToken: string;
}

export interface SessionUser {
  email: string;
  displayName: string;
}

function displayNameFromEmail(email: string): string {
  const local = email.split('@')[0] ?? email;
  const first = local.split(/[._-]+/)[0] ?? local;
  return first.charAt(0).toUpperCase() + first.slice(1);
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly storage = inject(TokenStorageService);
  private readonly baseUrl = environment.apiUrl;

  private readonly userSignal = signal<SessionUser | null>(this.restore());

  /** Nulo quando visitante; preenchido quando logado. */
  readonly currentUser = this.userSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.userSignal() !== null);

  login(request: LoginRequest): Observable<TokenPair> {
    return this.http.post<TokenPair>(`${this.baseUrl}/auth/access-token`, request).pipe(
      tap((tokens) => {
        this.storage.saveTokens(tokens.accessToken, tokens.refreshToken);
        const previous = this.storage.profile;
        const profile =
          previous && previous.email.toLowerCase() === request.email.toLowerCase()
            ? previous
            : { email: request.email, displayName: displayNameFromEmail(request.email) };
        this.storage.saveProfile(profile);
        this.userSignal.set(profile);
      }),
    );
  }

  register(request: RegisterRequest): Observable<unknown> {
    const payload = {
      firstName: request.firstName,
      lastName: request.lastName,
      birthDate: request.birthDate || null,
      phone: request.phone || null,
      email: request.email,
      password: request.password,
    };
    return this.http.post(`${this.baseUrl}/auth/user`, payload).pipe(
      tap(() => {
        // Guarda o nome para exibir no header apos o login (o backend nao expoe endpoint "me").
        this.storage.saveProfile({ email: request.email, displayName: request.firstName });
      }),
    );
  }

  refresh(): Observable<TokenPair | null> {
    const refreshToken = this.storage.refreshToken;
    if (!refreshToken) return of(null);
    return this.http.post<TokenPair>(`${this.baseUrl}/auth/refresh-token`, { refreshToken }).pipe(
      tap((tokens) => this.storage.saveTokens(tokens.accessToken, tokens.refreshToken)),
      catchError(() => {
        this.clearSession();
        return of(null);
      }),
    );
  }

  logout(): Observable<unknown> {
    const refreshToken = this.storage.refreshToken;
    // O endpoint de logout ainda nao existe no backend: tenta e ignora qualquer falha.
    const remote$ = refreshToken
      ? this.http.post(`${this.baseUrl}/auth/logout`, { refreshToken }).pipe(catchError(() => of(null)))
      : of(null);
    return remote$.pipe(tap(() => this.clearSession()));
  }

  private restore(): SessionUser | null {
    if (!this.storage.accessToken) return null;
    return this.storage.profile;
  }

  private clearSession(): void {
    this.storage.clear();
    this.userSignal.set(null);
  }
}
