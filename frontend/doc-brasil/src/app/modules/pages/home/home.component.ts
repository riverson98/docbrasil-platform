import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { DashboardStateService } from '../../../core/services/behaviorService/DashboardStateService.service';
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { AssociatesComponent } from './associates/associates.component';
import { MembersComponent } from './members/members.component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize, Observable, of, switchMap } from 'rxjs';
import { UrlService } from '../../../core/services/urlService/url.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NavbarComponent,
    RouterLink,
    RouterOutlet,
    CommonModule,
    ReactiveFormsModule 
],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements AfterViewInit{
  query: string = '';
  page:number = 1;
  pageSize:number = 10;
  filterControl = new FormControl('');
  currentRoute: string = '';
  previousRoute: string | null = null;

  @ViewChild(AssociatesComponent) associates!: AssociatesComponent;
  @ViewChild(MembersComponent) members!: MembersComponent;

  constructor(public state: DashboardStateService, public loadingService: LoadingService, private router: Router, private urlService: UrlService) {}
  
  ngAfterViewInit(): void {
    this.previousRoute = this.urlService.getPreviousUrl();
    this.currentRoute = this.router.url;

    if (this.currentRoute.includes('/associados') && this.associates) {
      this.onControlChange(this.associates, this.filterControl)
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe();
    } else if (this.currentRoute.includes('/membros') && this.members) {
      this.onControlChange(this.members, this.filterControl)
      .pipe(finalize(() => this.loadingService.hide()))
      .subscribe();
    }
  }

  private onControlChange(component:any, control: FormControl): Observable<any> {
    return control.valueChanges.pipe(
      debounceTime(1000),
      distinctUntilChanged(),
      switchMap(query => component.reloadData(this.page, this.pageSize, query)),
    )
  }

  loading (){
    console.log("valor da rota anterior", this.previousRoute)
    console.log("valor do current route ", this.currentRoute)
    if(this.router.url != this.currentRoute){
      this.loadingService.show();
    }
  }
}
