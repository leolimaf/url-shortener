import { Injectable } from '@angular/core';

const ACCESS_KEY = 'urlshortener.accessToken';
const REFRESH_KEY = 'urlshortener.refreshToken';
const PROFILE_KEY = 'urlshortener.profile';

export interface StoredProfile {
  email: string;
  displayName: string;
}

/** Persistencia simples em localStorage (decisao de design: sessao sobrevive ao F5). */
@Injectable({ providedIn: 'root' })
export class TokenStorageService {
  get accessToken(): string | null {
    return this.read(ACCESS_KEY);
  }

  get refreshToken(): string | null {
    return this.read(REFRESH_KEY);
  }

  get profile(): StoredProfile | null {
    try {
      const raw = localStorage.getItem(PROFILE_KEY);
      return raw ? (JSON.parse(raw) as StoredProfile) : null;
    } catch {
      return null;
    }
  }

  saveTokens(accessToken: string, refreshToken: string): void {
    try {
      localStorage.setItem(ACCESS_KEY, accessToken);
      localStorage.setItem(REFRESH_KEY, refreshToken);
    } catch {
      // armazenamento indisponivel: sessao vive so em memoria nesta sessao
    }
  }

  saveProfile(profile: StoredProfile): void {
    try {
      localStorage.setItem(PROFILE_KEY, JSON.stringify(profile));
    } catch {
      // ignora
    }
  }

  clear(): void {
    try {
      localStorage.removeItem(ACCESS_KEY);
      localStorage.removeItem(REFRESH_KEY);
      localStorage.removeItem(PROFILE_KEY);
    } catch {
      // ignora
    }
  }

  private read(key: string): string | null {
    try {
      return localStorage.getItem(key);
    } catch {
      return null;
    }
  }
}
