import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-created-modal',
  standalone: true,
  imports: [],
  templateUrl: './user-created-modal.component.html',
  styleUrl: './user-created-modal.component.scss'
})
export class UserCreatedModalComponent {

  constructor(private router: Router, private dialogRef: MatDialogRef<UserCreatedModalComponent>){}

  backToPage(){
    this.dialogRef.close('refresh');
  }
}
