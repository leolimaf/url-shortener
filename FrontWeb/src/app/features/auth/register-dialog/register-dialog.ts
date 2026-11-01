import { HttpErrorResponse } from '@angular/common/http';
import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { LoginDialog } from '../login-dialog/login-dialog';

@Component({
  selector: 'app-register-dialog',
  imports: [MatButtonModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatProgressSpinnerModule, ReactiveFormsModule],
  templateUrl: './register-dialog.html',
  styleUrl: './register-dialog.scss',
})
export class RegisterDialog {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly dialogRef = inject(MatDialogRef<RegisterDialog>);
  private readonly dialog = inject(MatDialog);
  private readonly host = inject(ElementRef<HTMLElement>);

  readonly busy = signal(false);
  readonly created = signal(false);
  readonly serverError = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    birthDate: [''],
    phone: ['', Validators.pattern(/^[()\s+.-]*\d[()\s+.\d-]*$/)],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  submit(): void {
    if (this.busy()) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      const invalid = Object.keys(this.form.controls).find((key) => this.form.get(key)?.invalid);
      if (invalid) {
        (this.host.nativeElement.querySelector(`[formControlName="${invalid}"]`) as HTMLElement | null)?.focus();
      }
      return;
    }
    this.busy.set(true);
    this.serverError.set(null);
    const value = this.form.getRawValue();
    this.auth
      .register({
        firstName: value.firstName.trim(),
        lastName: value.lastName.trim(),
        birthDate: value.birthDate || null,
        phone: value.phone?.trim() || null,
        email: value.email.trim(),
        password: value.password,
      })
      .subscribe({
        next: () => {
          this.busy.set(false);
          this.created.set(true);
        },
        error: (error: unknown) => {
          this.busy.set(false);
          this.serverError.set(this.mapError(error));
        },
      });
  }

  goToLogin(): void {
    this.dialogRef.close(false);
    this.dialog.open(LoginDialog, { width: '440px', maxWidth: '95vw' });
  }

  private mapError(error: unknown): string {
    if (error instanceof HttpErrorResponse) {
      if (error.status === 409) return 'Este e-mail já está cadastrado.';
      const backendMessage =
        typeof error.error === 'object' && error.error !== null
          ? ((error.error as Record<string, unknown>)['errorMessage'] as string | undefined)
          : undefined;
      if (backendMessage) return backendMessage;
      if (error.status === 400) return 'Verifique os dados informados.';
    }
    return 'Não foi possível criar a conta. Tente novamente.';
  }
}
