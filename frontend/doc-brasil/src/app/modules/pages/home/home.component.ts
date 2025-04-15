import { RouterLink, RouterOutlet } from '@angular/router';
import { DashboardStateService } from '../../../core/services/behaviorService/DashboardStateService.service';
import { NavbarComponent } from "../../components/navbar/navbar.component";
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { LoadingService } from '../../../core/services/loading/loading.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    NavbarComponent,
    RouterLink,
    RouterOutlet,
    CommonModule
],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  constructor(public state: DashboardStateService, public loading: LoadingService) {}
}
