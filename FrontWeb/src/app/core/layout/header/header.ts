import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../services/auth.service';
import { ThemeService } from '../../services/theme.service';
import { LoginDialog } from '../../../features/auth/login-dialog/login-dialog';
import { RegisterDialog } from '../../../features/auth/register-dialog/register-dialog';

@Component({
  selector: 'app-header',
  imports: [MatButtonModule, MatDialogModule, MatIconModule, MatMenuModule, MatSnackBarModule, MatToolbarModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  protected readonly auth = inject(AuthService);
  protected readonly theme = inject(ThemeService);
  private readonly dialog = inject(MatDialog);
  private readonly snackBar = inject(MatSnackBar);

  openLogin(): void {
    this.dialog.open(LoginDialog, { width: '440px', maxWidth: '95vw' });
  }

  openRegister(): void {
    this.dialog.open(RegisterDialog, { width: '480px', maxWidth: '95vw' });
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => this.snackBar.open('Você saiu.', 'Fechar', { duration: 3000 }),
      error: () => this.snackBar.open('Você saiu.', 'Fechar', { duration: 3000 }),
    });
  }
}
