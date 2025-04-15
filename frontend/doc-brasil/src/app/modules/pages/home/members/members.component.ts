import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/behaviorService/DashboardStateService.service';
import { UserService } from '../../../../core/services/user/user.service';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize, Observable } from 'rxjs';
import { PaginationParamsResponse } from '../../../../core/models/paginationParams/paginationParamsResponse';
import { PaginationParamsRequest } from '../../../../core/models/paginationParams/paginationParamsRequest';
import { AssociateSummary } from '../../../../core/models/user/associateSummary';
import { CommonModule } from '@angular/common';
import { RegisterModalComponent } from '../../../components/modals/register-modal/register-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { getStatusClass } from '../../../../script/_global-scripts';
import { StatusPipe } from "../../../../core/pipes/status.pipe";
import { FunctionPipe } from "../../../../core/pipes/function.pipe";
import { PaginationComponent } from "../../../components/pagination/pagination.component";

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [
    CommonModule,
    StatusPipe,
    FunctionPipe,
    PaginationComponent
],
  templateUrl: './members.component.html',
  styleUrl: './members.component.scss'
})
export class MembersComponent implements AfterViewInit{
  associates: AssociateSummary[] = [];
  currentPage = 1;
  pageSize = 0;
  itensPerPage = 10;
  membersData: any[] = [];
  totalPages = 1;
  
  constructor(public state: DashboardStateService, private service: UserService, 
    private loading: LoadingService, private dialog: MatDialog,) {}
  
  ngAfterViewInit(): void {
    setTimeout(() => {
      this.state.setTitle("Membros");
      this.state.setSubscription("Cadastre ou gerencie os membros da equipe")
    })
      this.loading.show();
      this.loadMembers(this.currentPage, this.itensPerPage).subscribe({
      next: (response) => {
        this.membersData = response.itens;
        this.totalPages = response.totalDePaginas;
      },
      error: (err) => console.log(err)
    })
  }
    
  loadMembers(page: number, itensPerPage: number): Observable<PaginationParamsResponse> {
    const params: PaginationParamsRequest = {
      pagina: page,
      quantidadeDeItensPorPagina: itensPerPage
    }
    return this.service.getMembers(params).pipe(
      finalize(() => this.loading.hide())
    );
  }
  
  reloadData(page: number, itensPerPage: number): void {
    this.loadMembers(page, itensPerPage).subscribe({
      next: (response) => {
        this.membersData = response.itens;
      },
      error: (err) => console.log(err)
    });
  }

  openRegisterModal() {
    const dialogRef = this.dialog.open(RegisterModalComponent, {
      width: '50%',
      height: 'auto',
    })

    dialogRef.afterClosed().subscribe(() => {
      this.reloadData(this.currentPage, this.itensPerPage)
    });
  }

  onPageChange(newPage: number) {
    this.currentPage = newPage;
    this.reloadData(newPage, this.itensPerPage);
  };

  getStatus(status: number): string { 
    return getStatusClass(status)
  }
}
