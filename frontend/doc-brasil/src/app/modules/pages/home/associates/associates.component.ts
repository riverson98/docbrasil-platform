import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/behaviorService/DashboardStateService.service';
import { UserService } from '../../../../core/services/user/user.service';
import { PaginationParamsRequest } from '../../../../core/models/paginationParams/paginationParamsRequest';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize, Observable } from 'rxjs';
import { PaginationParamsResponse } from '../../../../core/models/paginationParams/paginationParamsResponse';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { getStatusClass } from '../../../../script/_global-scripts';
import { RegisterModalComponent } from '../../../components/modals/register-modal/register-modal.component';
import { StatusPipe } from "../../../../core/pipes/status.pipe";
import { FunctionPipe } from "../../../../core/pipes/function.pipe";
import { PaginationComponent } from "../../../components/pagination/pagination.component";

@Component({
  selector: 'app-associates',
  standalone: true,
  imports: [CommonModule, StatusPipe, FunctionPipe, PaginationComponent],
  templateUrl: './associates.component.html',
  styleUrl: './associates.component.scss'
})
export class AssociatesComponent implements AfterViewInit {
  currentPage = 1;
  pageSize = 0;
  itensPerPage = 10;
  associatesData: any[] = [];
  totalPages = 1;
    
  constructor(public state: DashboardStateService, private service: UserService, 
    private loading: LoadingService, private dialog: MatDialog,) {}
    
  ngAfterViewInit(): void {
    setTimeout(() => {
      this.state.setTitle("Associados");
      this.state.setSubscription("Cadastre ou gerencie os associados")
    })
      this.loading.show();
      this.loadAssociates(this.currentPage, this.itensPerPage).subscribe({
      next: (response) => {
        this.associatesData = response.itens;
        this.totalPages = response.totalDePaginas;
      },
      error: (err) => console.log(err)
    })
  }

 loadAssociates(page: number, itensPerPage: number): Observable<PaginationParamsResponse> {
    const params: PaginationParamsRequest = {
      pagina: page,
      quantidadeDeItensPorPagina: itensPerPage
    }
    return this.service.getAssociates(params).pipe(
      finalize(() => this.loading.hide())
    );
  }

  reloadData(page: number, itensPerPage: number): void {
    this.loadAssociates(page, itensPerPage).subscribe({
      next: (response) => {
        this.associatesData = response.itens;
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
