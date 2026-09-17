import { DOCUMENT } from '@angular/common';
import { Injectable, effect, inject, signal } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const STORAGE_KEY = 'urlshortener.theme';
const DARK_CLASS = 'dark-mode';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);

  readonly mode = signal<ThemeMode>(this.readInitial());

  constructor() {
    effect(() => {
      const mode = this.mode();
      this.document.documentElement.classList.toggle(DARK_CLASS, mode === 'dark');
      try {
        localStorage.setItem(STORAGE_KEY, mode);
      } catch {
        // armazenamento indisponivel: mantem apenas em memoria
      }
    });
  }

  toggle(): void {
    this.mode.update((m) => (m === 'dark' ? 'light' : 'dark'));
  }

  private readInitial(): ThemeMode {
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved === 'light' || saved === 'dark') return saved;
    } catch {
      // ignora e usa preferencia do sistema
    }
    const prefersDark = window.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
    return prefersDark ? 'dark' : 'light';
  }
}
