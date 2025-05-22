import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UrlService {
  private previousUrl: string | null = null;
  private currentUrl: string | null = null;

  private previousUrlSubject = new BehaviorSubject<string | null>(null);
  previousUrl$ = this.previousUrlSubject.asObservable();

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        this.previousUrl = this.currentUrl;
        this.currentUrl = event.url;
        this.previousUrlSubject.next(this.previousUrl);
      }
    });
  }

  public hasRouteChanged(): boolean {
    return this.currentUrl !== this.previousUrl;
  }

  public getPreviousUrl(): string | null {
    return this.previousUrl;
  }

  public getCurrentUrl(): string | null {
    return this.currentUrl;
  }
}
