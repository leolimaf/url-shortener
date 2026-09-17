import { Component, ElementRef, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterDialog } from '../register-dialog/register-dialog';

@Component({
  selector: 'app-login-dialog',
  imports: [MatButtonModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatProgressSpinnerModule, ReactiveFormsModule],
  templateUrl: './login-dialog.html',
  styleUrl: './login-dialog.scss',
})
export class LoginDialog {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly dialogRef = inject(MatDialogRef<LoginDialog>);
  private readonly dialog = inject(MatDialog);
  private readonly host = inject(ElementRef<HTMLElement>);

  readonly busy = signal(false);
  readonly serverError = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  submit(): void {
    if (this.busy()) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.focusFirstError();
      return;
    }
    this.busy.set(true);
    this.serverError.set(null);
    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.dialogRef.close(true),
      error: () => {
        this.busy.set(false);
        this.serverError.set('Credenciais inválidas.');
      },
    });
  }

  goToRegister(): void {
    this.dialogRef.close(false);
    this.dialog.open(RegisterDialog, { width: '480px', maxWidth: '95vw' });
  }

  private focusFirstError(): void {
    const invalid = Object.keys(this.form.controls).find((key) => this.form.get(key)?.invalid);
    if (invalid) {
      (this.host.nativeElement.querySelector(`[formControlName="${invalid}"]`) as HTMLElement | null)?.focus();
    }
  }
}
