import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-user-created-modal',
  standalone: true,
  imports: [],
  templateUrl: './user-created-modal.component.html',
  styleUrl: './user-created-modal.component.scss'
})
export class UserCreatedModalComponent {

  constructor(private dialogRef: MatDialogRef<UserCreatedModalComponent>){}

  reload() {
    this.dialogRef.close();
  }
}
