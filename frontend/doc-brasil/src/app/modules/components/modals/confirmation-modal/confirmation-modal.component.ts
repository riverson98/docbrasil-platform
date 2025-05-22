import { Component, EventEmitter, Inject, Output } from '@angular/core';
import { UserModel } from '../../../../core/models/user/userModel';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UserService } from '../../../../core/services/user/user.service';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './confirmation-modal.component.html',
  styleUrl: './confirmation-modal.component.scss'
})
export class ConfirmationModalComponent {

  errorMessage: string = '';
  showErrorPopup: boolean = false;
  showSucessfulyPopup: boolean = false;
  successfulyMessage: string = '';

  constructor(@Inject(MAT_DIALOG_DATA) private associate: UserModel,  
  public dialogRef: MatDialogRef<ConfirmationModalComponent>, private userService: UserService, private loading: LoadingService){}

  closeModal(message:string = ''){
    console.log("Fechando o modal de confirmação")
    this.dialogRef.close(message);
  }

  deleteUser(){
    this.loading.show();

    const associateId = this.associate.id;

    this.userService.deleteUser(associateId).pipe(
      finalize(() => this.loading.hide())
    ).subscribe({
      next: () => {
        this.showMessage(false, "Associado deletado com sucesso!", true)

        setTimeout(() => {
          this.closeModal('user-deleted');
        }, 3000)
      },
      error: () => {
        this.showMessage(true, "Algo deu errado ao deletar o associado", true)
        
        setTimeout(() => {
          this.closeModal();
        }, 3000)
      }
    })
  }

  showMessage(isError:boolean, message: string, showPopUp:boolean) {
    if(isError){
      this.errorMessage = message;
      this.showErrorPopup = showPopUp;

      setTimeout(() => {
        this.showErrorPopup = false;
      } , 9000);
    }
    else {
      this.showSucessfulyPopup = showPopUp;
      this.successfulyMessage = message;

      setTimeout(() => {
        this.showSucessfulyPopup = false;
      } , 9000);
    }
  }
}
