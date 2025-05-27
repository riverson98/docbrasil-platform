import { Component, EventEmitter, Input, Output, SimpleChanges } from '@angular/core';
import { StatusPipe } from "../../../core/pipes/status.pipe";
import { FunctionPipe } from "../../../core/pipes/function.pipe";
import { getStatusClass } from '../../../script/_global-scripts';
import { CommonModule } from '@angular/common';
import { RegisterModalComponent } from '../modals/register-modal/register-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationModalComponent } from '../modals/confirmation-modal/confirmation-modal.component';
import { UserService } from '../../../core/services/user/user.service';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { finalize } from 'rxjs';
import { UserModel } from '../../../core/models/user/userModel';
import { UrlUpdated } from '../../../core/models/user/urlUpdated';

@Component({
  selector: 'app-table',
  standalone: true,
  imports: [
    StatusPipe, 
    FunctionPipe,
    CommonModule
  ],
  templateUrl: './table.component.html',
  styleUrl: './table.component.scss'
})
export class TableComponent {
  @Input({alias: 'data', required: true}) data:any[] = [];
  @Input({alias: 'currentPage', required: true}) currentPage!: number;
  @Input({alias: 'totalPages', required: true}) totalPages!: number;
  @Input({alias: 'addtitle', required: true}) addtitle!: string;
  @Output() requestDataUpdate = new EventEmitter<void>();
  @Output() pageChange = new EventEmitter<number>();
  @Output() onUserUpdated = new EventEmitter<void>();
  @Output() onUserDeleted = new EventEmitter<void>();

  constructor (private dialog: MatDialog, private userService: UserService, private loading: LoadingService) {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes['data']) {
    }
  }

  onOpenModal() {
    this.requestDataUpdate.emit();
  }
  
  getStatus(status: number): string { 
    return getStatusClass(status)
  }

  goToPrevious() {
    if (this.currentPage > 1) {
      this.pageChange.emit(this.currentPage - 1);
    }
  }
  
  goToNext() {
    if (this.currentPage < this.totalPages) {
      this.pageChange.emit(this.currentPage + 1);
    }
  }

  showUserDetails(id:string){
    const associate = this.data.find(a => a.id === id);
    this.loading.show();
    if (associate) {
      this.userService.updateDocsUrl(associate.id).pipe().subscribe({
        next: (response) => {
          associate.termoAdesaoDto.termoAdesaoUploadUrl = response.termoAdesaoUploadUrl;
          associate.fichaAssociadoDto.fichaAssociacaoUploadUrl = response.fichaAssociacaoUploadUrl;
          associate.cpfUploadUrl = response.cpfUploadUrl;
          associate.requerimentoJudicialDto.urlDoRequerimento = response.urlDoRequerimento;
          this.loading.hide();

          this.dialog.open(RegisterModalComponent, {
            data: associate,
            width: '80%',
            height: 'auto'
          }).afterClosed().subscribe(() => {
            this.onUserUpdated.emit();
          });
        }
      })
    }
  }

  openConfirmationModal(id:string){
    const associate = this.data.find(a => a.id === id);
    this.dialog.open(ConfirmationModalComponent, {
      data: associate,
      width: '50%',
      height: 'auto'
    }).afterClosed().subscribe((result) => {
      console.log("Valor do result:", result)
      if(result === 'user-deleted'){
        this.onUserDeleted.emit();
      }
    });
  }
}
