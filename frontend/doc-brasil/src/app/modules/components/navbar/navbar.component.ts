import { Component, Input, OnInit } from '@angular/core';
import { UserService } from '../../../core/services/user/user.service';
import { RouterLink } from '@angular/router';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { PhotoService } from '../../../core/services/user/photo.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit{
  photoPath:string | null = '';
  temporaryPhotoPath:string | null = '';
  tokenExpiration: Date | undefined;
  userId = this.authService.getUserId()!;

  constructor(private userService: UserService, private loading: LoadingService, private authService: AuthService, private photoService: PhotoService){}
 
 @Input({alias: 'userName', required: true}) userName!: string;

 ngOnInit(): void {
  const savedExpiration = localStorage.getItem('photoTokenExpiration');
  
  const photo = this.photoService.getImageUrlFromLocalStorage();
  this.photoService.imageUrl$.subscribe(img => this.photoPath = img);
  console.log("valor da foto:", photo);
  console.log("valor da foto temporaria:", this.temporaryPhotoPath);
  
  if(savedExpiration){
      this.tokenExpiration = new Date(savedExpiration);
    }

  if (this.tokenExpiration && new Date(this.tokenExpiration).getTime() > Date.now()){
      this.photoPath = localStorage.getItem('photoPath') ?? this.setDefaultImageByGender(null);
    } 
  else {
      this.getPhoto();
  }
}

getPhoto(){
    this.userService.getUserSummary(this.userId).subscribe((userSummary) => {
      if (userSummary?.urlFotoDePerfil) {
        this.photoPath = userSummary.urlFotoDePerfil!;
        const expiration = new Date();
        expiration.setHours(expiration.getMinutes() + 4);
        localStorage.setItem('photoPath', this.photoPath!);
        localStorage.setItem('photoTokenExpiration', expiration.toISOString());
      } 
      else {
        this.photoPath = this.setDefaultImageByGender(userSummary.genero);
      }
    });
  };

 formatUserName(): string {
  return this.userService.getUserName();
 }

 logout(): void {
    this.loading.show();
    this.authService.logout();
    this.loading.hide();
  }

  setDefaultImageByGender(gender:string | null): string {
    if(gender){
      switch(gender){
        case 'M':
          return"../../../../assets/default-male.jpg";
        case 'F':
          return "../../../../assets/default-female.jpg";
        case 'O':
          return "../../../../assets/default-female.jpg";
        default:
         return "../../../../assets/logo1.png";
        }
      }
      else {
        return "../../../../assets/logo1.png";
      }
  }
}
