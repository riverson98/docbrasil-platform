import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth/auth.service';
import { Router } from '@angular/router';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { UserService } from '../../../core/services/user/user.service';
import { finalize, switchMap, tap } from 'rxjs';
import { togglePassword } from '../../../script/_global-scripts';
import { LoginModel } from '../../../core/models/auth/loginModel';
import { CommonModule } from '@angular/common';
import { PhotoService } from '../../../core/services/user/photo.service';

@Component({
  selector: 'app-auth-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './auth-login.component.html',
  styleUrl: './auth-login.component.scss'
})
export class AuthLoginComponent {
  showErrorPopup = false;
  errorMessage = '';
 
  togglePasswordVisibility(inputHtml:string, iconHtml:string):void {
    togglePassword(inputHtml, iconHtml);
  };

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)])
  });

  constructor(private authService: AuthService, private router: Router, private photoService: PhotoService, 
    private loadingService: LoadingService, private userService: UserService) {}

  login():void {
    if(this.loginForm.valid) {
      const loginData: LoginModel = {
        email: this.loginForm.value.email!,
        password: this.loginForm.value.password!
      };

      this.loadingService.show()

      this.authService.login(loginData).pipe(
        tap(response => {
          if(!response.isAuthenticated){
            throw new Error('Email ou senha inválidos!');
            //this.showLoginError("E-mail ou senha inválidos", true)
          }

          localStorage.setItem('token', response.token);
          localStorage.setItem('username', response.name);
        }),
        switchMap(response => {
          const userId = this.authService.getUserId();
          return this.userService.getUserSummary(userId!);
        }),
        finalize(() => {
          this.loadingService.hide()
        }),
      ).subscribe({
        next: (userSummary) => {
          if(userSummary.urlFotoDePerfil){
            localStorage.setItem('userPhoto', userSummary.urlFotoDePerfil);
            localStorage.setItem('userGender', userSummary.genero);
          }

          this.router.navigate(["/painel/associados"]);
        },
        error: () => {
          this.showLoginError('Email ou senha inválidos!', true);
        }
      });
    }
    else {
      this.showLoginError('Email ou senha inválidos!', true)
    }
  }

  showLoginError(errorMessage: string, showError: boolean){
    this.errorMessage = errorMessage;
    this.showErrorPopup = showError

    setTimeout(() => {
      this.showErrorPopup = false;
    }, 10000);
  };
}

