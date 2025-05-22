import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, inject, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE, NativeDateAdapter, provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { UserService } from '../../../../core/services/user/user.service';
import { AuthService } from '../../../../core/services/auth/auth.service';
import { finalize } from 'rxjs';
import { FunctionPipe } from "../../../../core/pipes/function.pipe";
import { AssociadoResumidoDto } from '../../../../core/models/user/AssociadoResumidoDto';
import { ImageCroppedEvent, ImageCropperComponent, ImageTransform } from 'ngx-image-cropper';
import { PhotoService } from '../../../../core/services/user/photo.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatSelectModule,
    CommonModule,
    FunctionPipe,
    ImageCropperComponent 
],
  providers: [
    provideNativeDateAdapter(),
    { provide: NativeDateAdapter},
    { provide: MAT_DATE_LOCALE, useValue: 'pt-BR' },
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements AfterViewInit {
private _formBuilder = inject(FormBuilder);
errorMessage: string = '';
showErrorPopup: boolean = false;
showSucessfulyPopup: boolean = false;
successfulyMessage: string = '';
userFunction!: number;
imageChangedEvent: any = '';
transform: ImageTransform = {scale : 1};
showImageEditor: boolean = false;
croppedImageBlob: Blob | undefined;
temporatyPhoto: string | undefined;
userPhoto: string | undefined;

@ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

personalData = this._formBuilder.group({
    name: [{value: '', disabled: false}, [Validators.required, Validators.minLength(3)]],
    email: [{value: '', disabled: true}, [Validators.required, Validators.email]],
    birthdate: [{value: '', disabled: false}, [Validators.required]],
    gender: [{value: '', disabled: false}, Validators.required],
    codeAssociate: [{value: '', disabled: true}],
    photo: [null as File | null]
  });

  constructor(private loading: LoadingService, private userService: UserService, private authService: AuthService, private photoService: PhotoService) {}

  ngAfterViewInit(){
    const userId = this.authService.getUserId();

    setTimeout(() => {
      this.loading.show;
    });

    this.userService.getUserSummary(userId!).pipe(
      finalize(() => {
        this.loading.hide();
      })
    ).subscribe({
      next: (userSummary) => {
        this.personalData.patchValue({
          name: userSummary.nome,
          email: userSummary.email,
          birthdate: userSummary.dataDeNascimento,
          gender: userSummary.genero,
          codeAssociate: userSummary.codigoAssociado,
        });

        this.userFunction = userSummary.funcao;
        this.userPhoto = userSummary.urlFotoDePerfil;
      },
      error: (error) => {
        this.showMessage(true, "Algo deu errado ao trazer suas informações", true)
      }
    })
  }

  formatUserName(): string {
    return this.userService.getUserName();
  }
  
  UpdateUser() {
    if(this.personalData.invalid){
      this.showMessage(true, "Todos os dados devem está preenchidos.", true);
      return;
    }

    const userData = this.getUserFromForm();

    this.loading.show();

    this.userService.updateSummary(userData).pipe(
      finalize(() => {
        this.loading.hide();
      })
    ).subscribe({
      next: (user) => {
        this.showMessage(false, "Dados atualizados com sucesso", true);
      },
      error: (error) => {
        this.showMessage(true, "Algo deu errado ao atualizar os seus dados", true);
      }
    })
  }

  getUserFromForm(): AssociadoResumidoDto {
    const userName = this.personalData.get('name')?.value;
    const userBirthDate = new Date(this.personalData.get('birthdate')?.value!);   
    const date = new Date(userBirthDate);
    const formattedDate = date.toISOString().split('T')[0];
    const userGender = this.personalData.get('gender')?.value;
    const userId = this.authService.getUserId();

    const userData: AssociadoResumidoDto = {
      id: userId!,
      nome: userName!,
      dataDeNascimento: formattedDate!,
      genero: userGender!
    };

    return userData;
  }

  onDataInput(event: any) {
    let date = event.target.value.replace(/\D/g, '');
    
    if (date.length > 2) date = date.replace(/^(\d{2})(\d)/, '$1/$2');
    if (date.length > 4) date = date.replace(/^(\d{2})\/(\d{2})(\d)/, '$1/$2/$3');
    
    (event.target as HTMLInputElement).value = date;
     
    if (date.length === 10) {
      this.personalData.get('birthdate')!.setErrors(null);
      const [day, month, year] = date.split('/').map((part:any) => parseInt(part, 10));
        
      const enteredDate = new Date(year, month - 1, day);
      const today = new Date();
       
      let age = today.getFullYear() - enteredDate.getFullYear();
      const m = today.getMonth() - enteredDate.getMonth();
      if (m < 0 || (m === 0 && today.getDate() < enteredDate.getDate())) {
        age--;
      }
    
      if (age < 18 || age > 100) {
        this.personalData.get('birthdate')!.setErrors({ ageInvalid: true });
      }
      else {
        const formattedBirthDate = `${year}-${month}-${day}`;
        this.personalData.get('birthdate')!.setValue(formattedBirthDate);
        this.personalData.get('birthdate')!.updateValueAndValidity();
        (event.target as HTMLInputElement).value = date;
        this.personalData.get('birthdate')!.setErrors(null);
      }
    }
  }

  showMessage(isError:boolean, message: string, showPopUp:boolean) {
    if(isError){
      this.errorMessage = message;
      this.showErrorPopup = showPopUp;

      setTimeout(() => {
        this.showErrorPopup = false;
      } , 9000);
    }
    else {
      this.showSucessfulyPopup = showPopUp;
      this.successfulyMessage = message;

      setTimeout(() => {
        this.showSucessfulyPopup = false;
      } , 9000);
    }
  }

  openEditor() {
    this.showImageEditor = true;
    this.fileInput.nativeElement.click();
  }

  onImageCropped(event: ImageCroppedEvent) {
    if (event.blob) {
      this.croppedImageBlob = event.blob;
    }

    this.temporatyPhoto = event.objectUrl!;
  }

  closeEditor() {
    this.showImageEditor = false;
    this.imageChangedEvent = '';
  }

  cancelProfilePhoto() {
    this.personalData.get('photo')?.setValue(null);
    
    const fileInput = document.getElementById('photo') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = ''; 
      }
    
    this.temporatyPhoto = '';
    this.closeEditor();
  }

  saveCroppedImage() {
    if (this.croppedImageBlob) {
      this.loading.show();
      const file = new File([this.croppedImageBlob], 'cropped-image.png', { type: 'image/png' })!;
      const userId = this.authService.getUserId();

      if(this.temporatyPhoto)
        this.photoService.setImageUrl(this.temporatyPhoto);

      this.personalData.controls['photo'].setValue(file);
      this.userService.saveProfilePhoto(userId!, file).pipe(
        finalize(() => {
          this.loading.hide();
          this.closeEditor();
        })
      ).subscribe((response) => {
        const newPhotoUrl = response.fotoDePerfilUrl;
        this.photoService.removeImgFromLocalStorage();
        this.photoService.setImageUrlInLocalStorage(newPhotoUrl);
        const expiration = new Date();
        expiration.setMinutes(expiration.getMinutes() + 4);
        localStorage.setItem('photoTokenExpiration', expiration.toISOString());
        this.userPhoto = this.photoService.getImageUrlFromLocalStorage();
        this.photoService.setImageUrlInLocalStorage(newPhotoUrl);
        this.userPhoto = newPhotoUrl;
      });
    }
  }

  onImageLoaded() {
  }

  onLoadImageFailed() {
    this.showMessage(true, "Falha ao carregar imagem", true);
    this.closeEditor();
  }

  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
  }

  getDefaultImageByGender(): string{
    const gender = this.personalData.get('gender')?.value;

    switch(gender){
      case 'M':
        return "../../../../../assets/default-male.jpg";
      case 'F':
        return "../../../../../assets/default-female.jpg";
      case 'O':
        return "../../../../../assets/default-female.jpg";
      default:
       return "../../../../../assets/logo1.png";
    }
  }
}
