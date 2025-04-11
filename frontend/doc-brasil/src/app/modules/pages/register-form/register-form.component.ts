import { Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import {FormBuilder, Validators, FormsModule, ReactiveFormsModule, AbstractControl, ValidationErrors, FormGroup, FormControl, FormArray} from '@angular/forms';
import {StepperOrientation, MatStepperModule, MatStepper} from '@angular/material/stepper';
import {BreakpointObserver} from '@angular/cdk/layout';
import {Observable} from 'rxjs';
import {delay, finalize, map} from 'rxjs/operators';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatSelectModule} from '@angular/material/select';
import {MatIcon, MatIconModule} from '@angular/material/icon';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { MatTooltipModule } from '@angular/material/tooltip';
import {AsyncPipe, CommonModule} from '@angular/common';

import { MAT_DATE_LOCALE, NativeDateAdapter, provideNativeDateAdapter } from '@angular/material/core';
import { validateCpf } from '../../../script/_global-scripts';
import { UserService } from '../../../core/services/user/user.service';
import { LoadingService } from '../../../core/services/loading/loading.service';
import { UserCreatedModalComponent } from '../../components/modals/user-created/user-created-modal.component';

@Component({
  selector: 'app-register-form',
  standalone: true,
  imports: [
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    AsyncPipe,
    MatSelectModule,
    MatIcon,
    MatIconModule,
    MatDatepickerModule,  
    CommonModule,
    MatTooltipModule
  ],
  providers: [
    provideNativeDateAdapter(),
    { provide: NativeDateAdapter},
    { provide: MAT_DATE_LOCALE, useValue: 'pt-BR' },
  ],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss'
})
export class RegisterFormComponent implements OnInit{
  private _formBuilder = inject(FormBuilder);
  fileName: string = '';
  isRepresentation: boolean = false;
  showErrorPopup: boolean = false;
  errorMessage: string = '';
  errorDocumentRequired: boolean = false;
  whatsAppLink:string = '';

  personalData = this._formBuilder.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    birthdate: ['', [Validators.required]],
    gender: ['', Validators.required],
    tax: ['', [Validators.required], [validateCpf]],
    seniorCodeRepresentation: [null, [Validators.required]],
    codeRepresentation: [null],
    documentPhoto: [null, [Validators.required]],
  });
  
  addressData = this._formBuilder.group({
    zip: ['', [Validators.required, Validators.maxLength(9), Validators.minLength(8)]],
    street: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
    number: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(1)]],
    district: ['', [Validators.required, Validators.maxLength(30), Validators.minLength(3)]],
    city: ['', Validators.required],
    state: ['', [Validators.required, Validators.maxLength(2), Validators.minLength(2), Validators.pattern(/^[A-Za-z]{2}$/)]],
    proofOfResidence: [null, [Validators.required]]
  });

  termData = this._formBuilder.group ({
    associativeTerm: new FormControl<File | null>(null, Validators.required),
    contractTerm: new FormControl<File | null>(null, Validators.required),
  })

  stepperOrientation: Observable<StepperOrientation>;

  constructor(private dialog: MatDialog, private userService: UserService, private loading: LoadingService) {
    const breakpointObserver = inject(BreakpointObserver);

    this.stepperOrientation = breakpointObserver
      .observe('(min-width: 600px)')
      .pipe(map(({matches}) => (matches ? 'horizontal' : 'vertical')));
  }

  ngOnInit(): void {
    const numero = '5547992325526';
    const mensagem = 'Olá! Estou com dúvidas sobre o código do associado. Como posso obtê-lo?';

    const mensagemCodificada = encodeURIComponent(mensagem);
    
    const isMobile = /iPhone|Android/i.test(navigator.userAgent);
    const baseUrl = isMobile ? 'https://wa.me' : 'https://web.whatsapp.com/send';

    this.whatsAppLink = `${baseUrl}?phone=${numero}&text=${mensagemCodificada}`;
  }
  
  @ViewChild(MatStepper) private stepper!: MatStepper;

  hide = signal(true);
  clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }

  onCepInput(event: any): void {
  let zip = event.target.value.replace(/\D/g, '');

  if (zip.length > 5) zip = zip.replace(/^(\d{5})(\d)/, "$1-$2");
  
  this.addressData.controls['zip'].setValue(zip);
  }

  onFileSelected(event: Event, form: FormGroup, nameOfProperty: string) {
    const input = event.target as HTMLInputElement;
  
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
  
      form.patchValue({ [nameOfProperty]: file });
  
      form.get(nameOfProperty)?.updateValueAndValidity();
    }
  
    console.log("Nome da propriedade recebida:", nameOfProperty);
  }

  onMultipleFilesSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length >= 2) {
      const files = Array.from(input.files);

      const associative = files.find(file =>
        /ficha.*associativa/i.test(file.name.toLowerCase())
      );
      
      const contract = files.find(file =>
        /termo.*ades[aã]o/i.test(file.name.toLowerCase())
      );

      console.log("Valor do associative:", associative);
      console.log("Valor do contract:", contract);

      if(!associative || !contract){
        alert("Você deve anexar os dois documentos: Ficha Associativa e Termo de Adesão.");
      }
      else {
        this.termData.patchValue({ associativeTerm: associative });
        this.termData.get('associativeTerm')?.updateValueAndValidity();
        
        this.termData.patchValue({ contractTerm: contract });
        this.termData.get('contractTerm')?.updateValueAndValidity();
      }
    }
  }

  onlyNumbers(event: KeyboardEvent) {
    const char = event.key;
    if (!/^\d$/.test(char)) {
      event.preventDefault();
    }
  }

  onCpfInput(event:any) {
    let tax = event.target.value.replace(/\D/g, '');
    
    if (tax.length > 3) tax = tax.replace(/^(\d{3})(\d)/, '$1.$2');
    if (tax.length > 6) tax = tax.replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3');
    if (tax.length > 9) tax = tax.replace(/^(\d{3})\.(\d{3})\.(\d{3})(\d)/, '$1.$2.$3-$4');

    (event.target as HTMLInputElement).value = tax;
  }

  selectIsRepresentation () {
    this.isRepresentation = !this.isRepresentation;
  }

  verifyForm(event: Event, form: FormGroup):   void {
    console.log("o formulario esta invalido:", form.invalid)
    console.log("Formulario:", form.value)
    if (form.invalid) {
      event.preventDefault();
      form.markAllAsTouched();
    } else {
      console.log("valor do form:", form);
      if(this.stepper.selectedIndex === 0){
        this.loading.show();
        this.verifyCodeRepresentationFormToSubmit(form).pipe(
          finalize(() => this.loading.hide())
        )
        .subscribe({
          next: () => {
            this.stepper.next();
          },
          error: (error) => {
            console.log("valor do status do erro:", error.status)
            if(error.status === 404){
              this.showErrorMessage("Nenhum representante encontrado com este codigo, se não possui um entre em contato conosco", true);
              event.preventDefault();
            }
            else
            this.showErrorMessage("Algo deu errado", true);
            console.log(error);
          }
        })
      }
      else{
        this.stepper.next();
      }
    }
  }

  onDataInput(event: any) {
    let date = event.target.value.replace(/\D/g, '');
  
    if (date.length > 2) date = date.replace(/^(\d{2})(\d)/, '$1/$2');
    if (date.length > 4) date = date.replace(/^(\d{2})\/(\d{2})(\d)/, '$1/$2/$3');
  
    (event.target as HTMLInputElement).value = date;
    
    console.log("valor do length: ", date.length);
   
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
      console.log("Valor do age", age);
  
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

  handleClick(event: any, termData: FormGroup){
    this.verifyForm(event, termData);
    this.register();
  }

  register() {
    if(this.addressData.invalid || this.personalData.invalid || this.termData.invalid){
      this.showErrorMessage("Todos os campos do formulário precisam ser preenchidos para concluir o cadastro.", true);
      return;
    }
    this.loading.show();
    const userData = this.getUserFromForm();
    this.userService.createNewUser(userData).pipe(
      finalize(() => this.loading.hide())
    )
    .subscribe({
      next: () => {
        const viewWidth = window.innerWidth;
        if(viewWidth < 1024){
          this.dialog.open(UserCreatedModalComponent, {
            maxWidth: '100vw',
            maxHeight: '100vh',
            height: '100%',
            width: '100%'
          }).afterClosed().subscribe(() => {
            this.resetFormState(this.personalData);
            this.resetFormState(this.addressData);
            this.resetFormState(this.termData);
            this.stepper.reset();
          });
        }
        else {
          this.dialog.open(UserCreatedModalComponent).afterClosed().subscribe(() => {
            this.resetFormState(this.personalData);
            this.resetFormState(this.addressData);
            this.resetFormState(this.termData);
            this.stepper.reset();
          });
        }
      },
      error: (error) => {
        console.log("O error foi:", error)
        this.showErrorMessage("Algo deu errado", true);
      }
    })
  }

  getUserFromForm(): FormData {
    const userName = this.personalData.get('name')?.value;
    const userEmail = this.personalData.get('email')?.value;
    const userBirthDate = new Date(this.personalData.get('birthdate')?.value!);   
    const date = new Date(userBirthDate);
    const formattedDate = date.toISOString().split('T')[0];
    const userGender = this.personalData.get('gender')?.value;
    const tax = this.personalData.get('tax')?.value;
    const seniorCodeRepresentation = this.personalData.get('seniorCodeRepresentation')?.value;
    const codeRepresentation = this.personalData.get('codeRepresentation')?.value;
    const documentPhoto = this.personalData.get('documentPhoto')?.value;
    const associativeTerm = this.termData.get('associativeTerm')?.value;
    const contractTerm = this.termData.get('contractTerm')?.value;
    const zip = this.addressData.get('zip')?.value;
    const street = this.addressData.get('street')?.value;
    const number = this.addressData.get('number')?.value;
    const district = this.addressData.get('district')?.value;
    const city = this.addressData.get('city')?.value;
    const state = this.addressData.get('state')?.value;
    const proofOfResidence = this.addressData.get('proofOfResidence')?.value;

    const userDataForm = new FormData();
    userDataForm.append('nome', userName!);
    userDataForm.append('email', userEmail!);
    userDataForm.append('dataDeNascimento', formattedDate);
    userDataForm.append('genero', userGender!);
    userDataForm.append('cpf', tax!);
    userDataForm.append('CodigoRepresentanteSuperior', seniorCodeRepresentation!);
    userDataForm.append('codigoRepresentante', codeRepresentation!);
    userDataForm.append('fotoDoDocumento', documentPhoto!);
    userDataForm.append('fichaAssociadoDto.fichaAssociacao', associativeTerm!);
    userDataForm.append('termoAdesaoDto.termoAdesao', contractTerm!);
    userDataForm.append('enderecoDto.cep', zip!);
    userDataForm.append('enderecoDto.rua', street!);
    userDataForm.append('enderecoDto.numero', number!);
    userDataForm.append('enderecoDto.bairro', district!);
    userDataForm.append('enderecoDto.cidade', city!);
    userDataForm.append('enderecoDto.estado', state!);
    userDataForm.append('enderecoDto.fotoDoComprovante', proofOfResidence!);

    return userDataForm;
  }

  resetFormState(form: FormGroup | FormArray) {
    form.reset();
  
    Object.keys(form.controls).forEach(key => {
      const control = form.get(key);
  
      if (control instanceof FormControl) {
        control.setValue('');
        control.markAsPristine();
        control.markAsUntouched();
        control.setErrors(null);
      }
  
      if (control instanceof FormGroup || control instanceof FormArray) {
        this.resetFormState(control);
      }
    });
    form.updateValueAndValidity();
  }
  
  showErrorMessage(erroMessage: string, showError:boolean) {
    this.errorMessage = erroMessage;
    this.showErrorPopup = showError;
    this.showErrorPopup = true;

    setTimeout(() => {
      this.showErrorPopup = false;
    } , 10000);
  }

  verifyCodeRepresentationFormToSubmit(personalData:FormGroup): Observable<void>{
    return new Observable<void>(observer => {
        const seniorCodeRepresentation = personalData.get('seniorCodeRepresentation')?.value;
        console.log("Valor do codigo do representante:", seniorCodeRepresentation)

        this.userService.getUserCodeRepresentation(seniorCodeRepresentation!).subscribe({
          next: response => {
            console.log(response);
            observer.next();
            observer.complete();
          },
          error: err => {
            observer.error(err);
          }
        });
    })
  }
}