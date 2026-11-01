import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ShortenResult {
  code: string;
  shortUrl: string;
}

@Injectable({ providedIn: 'root' })
export class UrlShortenerService {
  private readonly http = inject(HttpClient);

  shorten(longUrl: string): Observable<ShortenResult> {
    return this.http.post<{ code: string }>(`${environment.apiUrl}/url-shortener`, { url: longUrl }).pipe(
      map(({ code }) => ({ code, shortUrl: `${this.shortUrlBase()}/${code}` })),
    );
  }

  private shortUrlBase(): string {
    if (environment.shortUrlBase) return environment.shortUrlBase.replace(/\/$/, '');
    return window.location.origin;
  }
}
