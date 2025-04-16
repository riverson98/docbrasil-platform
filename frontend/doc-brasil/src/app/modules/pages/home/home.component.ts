import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { DashboardStateService } from '../../../core/services/behaviorService/DashboardStateService.service';
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { AssociatesComponent } from './associates/associates.component';
import { MembersComponent } from './members/members.component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Observable, of, switchMap } from 'rxjs';

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

  @ViewChild(AssociatesComponent) associates!: AssociatesComponent;
  @ViewChild(MembersComponent) members!: MembersComponent;

  constructor(public state: DashboardStateService, public loading: LoadingService, private router: Router, private cdr: ChangeDetectorRef) {}
  
  ngAfterViewInit(): void {
    const currentRoute = this.router.url;

    if (currentRoute.includes('/associados') && this.associates) {
      this.onControlChange(this.associates, this.filterControl)
      .subscribe((response) => console.log(response));
    } else if (currentRoute.includes('/membros') && this.members) {
      this.onControlChange(this.members, this.filterControl)
      .subscribe((response) => console.log(response));
    }
  }

  private onControlChange(component:any, control: FormControl): Observable<any> {
    return control.valueChanges.pipe(
      debounceTime(1000),
      distinctUntilChanged(),
      switchMap(query => component.reloadData(this.page, this.pageSize, query))
    )
  }
}
