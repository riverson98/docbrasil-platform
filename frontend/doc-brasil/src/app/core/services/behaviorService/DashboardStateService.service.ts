import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardStateService {
  private tableDataSubject = new BehaviorSubject<any[]>([]);
  private tipoDeConteudo: 'associados' | 'membros' | null = null;

  private pageTitle = new BehaviorSubject<string>('');
  pageTitle$ = this.pageTitle.asObservable();

  private tableData = new BehaviorSubject<any[]>([]);
  tableData$ = this.tableData.asObservable();

  private subscription = new BehaviorSubject<string>('');
  subscription$ = this.subscription.asObservable();

  private totalOfPages = new BehaviorSubject<number>(1);
  totalOfPages$ = this.totalOfPages.asObservable();

  setTitle(title: string) {
    this.pageTitle.next(title);
  }

  setTableData(data: any[]) {
    this.tableData.next(data);
  }

  setSubscription(subs: string){
    this.subscription.next(subs);
  }

  setTotalOfPages(totalPages: number){
    this.totalOfPages.next(totalPages);
  }

  setTipoDeConteudo(tipo: 'associados' | 'membros') {
    this.tipoDeConteudo = tipo;
  }

  getTipoDeConteudo(): 'associados' | 'membros' | null {
    return this.tipoDeConteudo;
  }

  getTitle(): Observable<string> {
    return this.pageTitle$;
  }

  getTableData(): Observable<any[]> {
    return this.tableData$;
  }

  getSubscription(): Observable<string> {
    return this.subscription$;
  }

  getTotalOfPages(): Observable<number> {
    return this.totalOfPages$;
  }

  clearTableData() {
    this.tableDataSubject.next([]);
  }
}