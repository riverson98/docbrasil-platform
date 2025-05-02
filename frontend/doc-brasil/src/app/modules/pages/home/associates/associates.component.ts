import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/behaviorService/DashboardStateService.service';
import { UserService } from '../../../../core/services/user/user.service';
import { PaginationParamsRequest } from '../../../../core/models/paginationParams/paginationParamsRequest';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize, Observable, tap } from 'rxjs';
import { PaginationParamsResponse } from '../../../../core/models/paginationParams/paginationParamsResponse';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { RegisterModalComponent } from '../../../components/modals/register-modal/register-modal.component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { PainelTitleComponent } from "../../../components/painel-title/painel-title.component";
import { TableComponent } from "../../../components/table/table.component";

@Component({
  selector: 'app-associates',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PainelTitleComponent,
    TableComponent
],
  templateUrl: './associates.component.html',
  styleUrl: './associates.component.scss'
})
export class AssociatesComponent implements AfterViewInit {
  currentPage:number = 1;
  pageSize: number = 0;
  itensPerPage: number = 10;
  associatesData: any[] = [];
  totalPages: number = 1;
  query:string = '';
  filterControl = new FormControl('');
    
  constructor(public state: DashboardStateService, private service: UserService, 
    private loading: LoadingService, private dialog: MatDialog,) {}
    
  ngAfterViewInit(): void {
      this.loading.show();
      this.loadAssociates(this.currentPage, this.itensPerPage, '').subscribe({
      next: (response) => {
        this.associatesData = response.itens;
        this.totalPages = response.totalDePaginas;
      },
      error: (err) => console.log(err)
    })
  }

 loadAssociates(page: number, itensPerPage: number, query:string): Observable<PaginationParamsResponse> {
    const params: PaginationParamsRequest = {
      pagina: page,
      quantidadeDeItensPorPagina: itensPerPage,
      query: query
    }
    return this.service.getAssociates(params).pipe(
      finalize(() => this.loading.hide())
    );
  }

  reloadData(page: number, itensPerPage: number, query:string): Observable<any> {
    this.query = query;
    return this.loadAssociates(page, itensPerPage, query);
  }
  
  handlePageChange(newPage: number) {
    this.currentPage = newPage;
    this.reloadData(newPage, this.itensPerPage, this.query)
      .subscribe((response) => {
        this.associatesData = response.itens;
    });
  };

  handleRegisterModal(){
    const dialogRef = this.dialog.open(RegisterModalComponent, {
      width: '50%',
      height: 'auto',
    })
  
    dialogRef.afterClosed().subscribe(() => {
      this.reloadData(this.currentPage, this.itensPerPage,'')
        .subscribe((response) => {
          this.associatesData = response.itens;
        });
    });
  };

  onFilterChanged(filter: string){
    this.reloadData(this.currentPage, this.itensPerPage, filter)
    .subscribe((response) => {
      this.associatesData = response.itens;
      this.totalPages = response.totalDePaginas;
    });
  }

  reloadPage() {
    this.reloadData(this.currentPage, this.itensPerPage, '')
    .subscribe((response) => {
      this.associatesData = response.itens;
    })
  }
}
