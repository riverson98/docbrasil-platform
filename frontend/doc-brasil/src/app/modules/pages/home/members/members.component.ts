import { AfterViewInit, Component } from '@angular/core';
import { DashboardStateService } from '../../../../core/services/behaviorService/DashboardStateService.service';
import { UserService } from '../../../../core/services/user/user.service';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { finalize, Observable, tap } from 'rxjs';
import { PaginationParamsResponse } from '../../../../core/models/paginationParams/paginationParamsResponse';
import { PaginationParamsRequest } from '../../../../core/models/paginationParams/paginationParamsRequest';
import { CommonModule } from '@angular/common';
import { RegisterModalComponent } from '../../../components/modals/register-modal/register-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { PainelTitleComponent } from "../../../components/painel-title/painel-title.component";
import { TableComponent } from "../../../components/table/table.component";
import { UserNotFoundComponent } from "../../../components/user-not-found/user-not-found.component";

@Component({
  selector: 'app-members',
  standalone: true,
  imports: [
    CommonModule,
    PainelTitleComponent,
    TableComponent,
    UserNotFoundComponent
],
  templateUrl: './members.component.html',
  styleUrl: './members.component.scss'
})
export class MembersComponent implements AfterViewInit{
  currentPage: number = 1;
  pageSize: number = 0;
  itensPerPage: number = 10;
  membersData: any[] = [];
  totalPages:number = 1;
  query: string = '';
  hasNotFoundError: boolean = false;
  
  constructor(public state: DashboardStateService, private service: UserService, 
    private loading: LoadingService, private dialog: MatDialog,) {}
  
  ngAfterViewInit(): void {
      this.loading.show();
      this.loadMembers(this.currentPage, this.itensPerPage, '').subscribe({
      next: (response) => {
        this.membersData = response.itens;
        this.totalPages = response.totalDePaginas;
      },
      error: (err) => console.log(err)
    })
  }
    
  loadMembers(page: number, itensPerPage: number, query: string): Observable<PaginationParamsResponse> {
    const params: PaginationParamsRequest = {
      pagina: page,
      quantidadeDeItensPorPagina: itensPerPage,
      query: query
    }
    return this.service.getMembers(params).pipe(
      finalize(() => this.loading.hide())
    );
  }
  
  reloadData(page: number, itensPerPage: number, query:string): Observable<any> {
    this.query = query;
    return this.loadMembers(page, itensPerPage, query);
  }

  handleRegisterModal() {
    const dialogRef = this.dialog.open(RegisterModalComponent, {
      width: '80%',
      height: 'auto',
    })

    dialogRef.afterClosed().subscribe(() => {
      this.reloadData(this.currentPage, this.itensPerPage, '')
      .subscribe((response) => this.membersData = response.itens)
    });
  }

  handleDeleteUser() {
    console.log("usuario deletado atualizado:")
    this.reloadData(this.currentPage, this.itensPerPage,'')
      .subscribe({
        next: (response) => {
        console.log("usuario deletado atualizado:", response)
        this.membersData = response.itens || [];
        this.hasNotFoundError = this.membersData.length === 0;
      }, 
      error: (error) => {
        if(error.status === 404){
          this.membersData = [];
          this.hasNotFoundError = true;
        }
      } 
    });
  }

  handlePageChange(newPage: number) {
    this.currentPage = newPage;
    this.reloadData(newPage, this.itensPerPage, this.query)
    .subscribe((response) => this.membersData = response.itens);
  };

  onFilterChanged(filter: string){
    this.reloadData(this.currentPage, this.itensPerPage, filter)
    .subscribe((response) => {
      this.membersData = response.itens;
      this.totalPages = response.totalDePaginas;
    });
  }

  reloadPage() {
    this.reloadData(this.currentPage, this.itensPerPage, '')
    .subscribe((response) => {
      this.membersData = response.itens;
    })
  }
}
