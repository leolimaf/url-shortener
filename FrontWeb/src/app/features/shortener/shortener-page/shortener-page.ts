import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { ShortenResult, UrlShortenerService } from '../../../core/services/url-shortener.service';

function isAbsoluteHttpUrl(value: string): boolean {
  try {
    const parsed = new URL(value.trim());
    return parsed.protocol === 'http:' || parsed.protocol === 'https:';
  } catch {
    return false;
  }
}

@Component({
  selector: 'app-shortener-page',
  imports: [
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    ReactiveFormsModule,
  ],
  templateUrl: './shortener-page.html',
  styleUrl: './shortener-page.scss',
})
export class ShortenerPage {
  private readonly fb = inject(FormBuilder);
  private readonly shortener = inject(UrlShortenerService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly host = inject(ElementRef<HTMLElement>);

  readonly busy = signal(false);
  readonly result = signal<(ShortenResult & { originalUrl: string }) | null>(null);
  readonly formatError = signal(false);

  readonly form = this.fb.nonNullable.group({
    url: ['', Validators.required],
  });

  shorten(): void {
    const raw = this.form.controls.url.value.trim();
    if (!raw) {
      this.form.markAllAsTouched();
      this.focusField('url');
      return;
    }
    if (!isAbsoluteHttpUrl(raw)) {
      this.formatError.set(true);
      this.focusField('url');
      return;
    }
    this.formatError.set(false);
    this.busy.set(true);
    this.shortener.shorten(raw).subscribe({
      next: (shortened) => {
        this.busy.set(false);
        this.result.set({ ...shortened, originalUrl: raw });
      },
      error: (error: unknown) => {
        this.busy.set(false);
        this.snackBar.open(this.mapError(error), 'Fechar', { duration: 5000 });
      },
    });
  }

  copy(): void {
    const current = this.result();
    if (!current) return;
    const done = () => this.snackBar.open('Link copiado!', 'Fechar', { duration: 3000 });
    if (navigator.clipboard?.writeText) {
      navigator.clipboard.writeText(current.shortUrl).then(done, () => this.legacyCopy(current.shortUrl) && done());
    } else if (this.legacyCopy(current.shortUrl)) {
      done();
    } else {
      this.snackBar.open('Copie manualmente: ' + current.shortUrl, 'Fechar', { duration: 6000 });
    }
  }

  shortenAnother(): void {
    this.result.set(null);
    this.form.reset();
  }

  private focusField(controlName: string): void {
    (this.host.nativeElement.querySelector(`[formControlName="${controlName}"]`) as HTMLElement | null)?.focus();
  }

  private legacyCopy(text: string): boolean {
    try {
      const area = document.createElement('textarea');
      area.value = text;
      area.style.position = 'fixed';
      area.style.opacity = '0';
      document.body.appendChild(area);
      area.select();
      const ok = document.execCommand('copy');
      document.body.removeChild(area);
      return ok;
    } catch {
      return false;
    }
  }

  private mapError(error: unknown): string {
    if (error instanceof HttpErrorResponse && error.status === 400) {
      return 'URL inválida. Confira o endereço e tente novamente.';
    }
    return 'Não foi possível encurtar agora. Tente novamente.';
  }
}
