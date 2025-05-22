import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class PhotoService {
    private imageUrlSubject = new BehaviorSubject<string | null>(null);
    imageUrl$ = this.imageUrlSubject.asObservable();

  setImageUrl(url: string) {
    this.imageUrlSubject.next(url);
  }
  
  clearImageUrl() {
    this.imageUrlSubject.next(null);
  }

  getImageUrlFromLocalStorage(): string {
    return localStorage.getItem('userPhoto')!;
  }

  removeImgFromLocalStorage(): void{
    localStorage.removeItem('userPhoto');
  }

  setImageUrlInLocalStorage(photo: string): void {
    localStorage.setItem("userPhoto", photo)
  }
} 