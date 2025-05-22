import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { updatePassword } from '../../../../core/models/auth/updatePassword';
import { AuthService } from '../../../../core/services/auth/auth.service';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-password',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule, 
    MatButtonModule, 
    MatIconModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './password.component.html',
  styleUrl: './password.component.scss'
})
export class PasswordComponent {
  hide = signal(true);
  hideCurrent = signal(true);
  passwordForm!: FormGroup;
  showErrorPopup: boolean = false;
  errorMessage: string[] = [];
  showSucessfulyPopup: boolean = false;
  successfulyMessage: string[] = [];
  message: string[] = [];

  constructor(private fb: FormBuilder, private authService: AuthService, private loading: LoadingService) {
    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmNewPassword: ['', Validators.required]
    });
  }
  
  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  clickEventCurrentPassword(event: MouseEvent) {
    this.hideCurrent.set(!this.hideCurrent());
    event.stopPropagation();
  }

  updatePassword(){
    if (this.passwordForm.invalid) {
      this.message.push("Todos os campos devem está preenchidos e válidos")
      this.showMessage(true, this.message, true);
      return;
    }

    const updatePasswordDto: updatePassword = {
      userId: this.authService.getUserId()!,
      currentPassword: this.passwordForm.get("currentPassword")?.value,
      newPassword: this.passwordForm.get("newPassword")?.value,
      confirmNewPassword: this.passwordForm.get("confirmNewPassword")?.value
    };

    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$/;

    if(!regex.test(updatePasswordDto.newPassword)){
      this.message.push("Sua senha deve ter pelo menos 6 caracteres.");
      this.message.push("Deve conter pelo menos um número (1, 2, 3...).");
      this.message.push("Deve conter pelo menos uma letra maiúscula (A, B, C...)");
      this.message.push("Deve conter pelo menos uma letra minúscula (a, b, c...)");
      this.message.push("Deve conter pelo menos um símbolo especial (@, #, ?...)");
      
      this.showMessage(true, this.message, true);

      return;
    }

    if(updatePasswordDto.newPassword !== updatePasswordDto.confirmNewPassword){
      this.message.push("As senhas devem coincidir.")
      this.showMessage(true, this.message, true)
      return;
    }

    this.loading.show();

    this.authService.updatePassword(updatePasswordDto).pipe(
      finalize(() => this.loading.hide())
    )
    .subscribe({
      next: (response) => {
        if(response){
          this.message.push("Senha alterada com sucesso!")
          this.showMessage(false, this.message, true)
        }
        else {
          this.message.push("A senha atual está inválida")
          this.showMessage(true, this.message, true)
        }
      },
      error: (error) => {
        this.message.push("Algo deu errado");
        this.showMessage(true, this.message, true)
      }
    })
  }

  showMessage(isError:boolean, message: string[], showPopUp:boolean) {
    if(isError){
      this.errorMessage = message;
      this.showErrorPopup = showPopUp;
      this.message = [];

      setTimeout(() => {
        this.showErrorPopup = false;
      } , 9000);
    }
    else {
      this.showSucessfulyPopup = showPopUp;
      this.successfulyMessage = message;
      this.message = [];

      setTimeout(() => {
        this.showSucessfulyPopup = false;
      } , 9000);
    }
  }
}
