import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { RegisterModalComponent } from '../modals/register-modal/register-modal.component';

@Component({
  selector: 'app-user-not-found',
  standalone: true,
  imports: [],
  templateUrl: './user-not-found.component.html',
  styleUrl: './user-not-found.component.scss'
})
export class UserNotFoundComponent {

  constructor(private dialog: MatDialog) {}

  @Input({alias:'entity', required: true}) entity: string = '';
  @Output() requestDataUpdate = new EventEmitter<void>();

  openRegisterModal(){
    this.requestDataUpdate.emit(); 
  }
}
