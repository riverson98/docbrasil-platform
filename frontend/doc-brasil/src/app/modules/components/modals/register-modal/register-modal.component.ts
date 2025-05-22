import { AfterViewInit, Component, Inject, inject, Input, OnInit, signal, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { validateCpf } from '../../../../script/_global-scripts';
import { UserService } from '../../../../core/services/user/user.service';
import { LoadingService } from '../../../../core/services/loading/loading.service';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { finalize, Observable, timeout } from 'rxjs';
import { UserCreatedModalComponent } from '../user-created/user-created-modal.component';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MAT_DATE_LOCALE, NativeDateAdapter, provideNativeDateAdapter } from '@angular/material/core';
import { UserModel } from '../../../../core/models/user/userModel';
import { AddressService } from '../../../../core/services/address/address.service';

@Component({
  selector: 'app-register-modal',
  standalone: true,
  imports: [
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
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
  templateUrl: './register-modal.component.html',
  styleUrl: './register-modal.component.scss'
})
export class RegisterModalComponent implements AfterViewInit{
  private _formBuilder = inject(FormBuilder);
  fileName: string = '';
  isRepresentation: boolean = false;
  showErrorPopup: boolean = false;
  errorMessage: string = '';
  errorDocumentRequired: boolean = false;
  addressPhoto:string = ''; 
  addressId:number = 0; 

  constructor(private dialog: MatDialog, 
    private service: UserService, private loading: LoadingService, public dialogRef: MatDialogRef<RegisterModalComponent>,
    @Inject(MAT_DIALOG_DATA) public associate: UserModel, private addressService: AddressService) {}

  ngAfterViewInit() {
    if (this.associate) {
      this.personalData.get('documentPhoto')?.clearValidators();
      this.personalData.get('documentPhoto')?.updateValueAndValidity();
      this.personalData.get('associativeTerm')?.clearValidators();
      this.personalData.get('associativeTerm')?.updateValueAndValidity();
      this.personalData.get('contractTerm')?.clearValidators();
      this.personalData.get('contractTerm')?.updateValueAndValidity();
      this.personalData.get('courtOrder')?.clearValidators();
      this.personalData.get('courtOrder')?.updateValueAndValidity();
      this.addressData.get('proofOfResidence')?.clearValidators();
      this.addressData.get('proofOfResidence')?.updateValueAndValidity();

      setTimeout(() => {
        this.loading.show();
      });

      this.addressService.getAddressByAssociateId(this.associate.id)
      .pipe(
        finalize(() => this.loading.hide())
      ).subscribe({
        next: (response) => {
          this.personalData.patchValue({
            name: this.associate?.nome,
            email: this.associate?.email,
            birthdate: this.associate?.dataDeNascimento,
            gender: this.associate?.genero,
            tax: this.associate?.cpf,
            seniorCodeRepresentation: this.associate?.codigoRepresentanteSuperior,
            documentLink: this.associate?.cpfUploadUrl,
            codeAssociate: this.associate?.codigoAssociado,
            function: this.associate?.funcao,
            status: this.associate?.status,
            associativeTerm: this.associate?.fichaAssociadoDto?.fichaAssociacaoUploadUrl,
            contractTerm: this.associate?.termoAdesaoDto?.termoAdesaoUploadUrl,
            courtOrder: this.associate?.requerimentoJudicialDto?.urlDoRequerimento
          });

          this.addressData.patchValue({
            zip: response?.cep,
            street: response?.rua,
            number: response?.numero,
            district: response?.bairro,
            city: response?.cidade,
            state: response?.estado,
            proofLink: response?.comprovanteDeResidenciaUpload
          })

          this.addressPhoto = response.comprovanteDeResidenciaUpload;
          this.addressId = response.id;
          
          this.personalData.get('tax')?.disable();
          this.personalData.get('codeAssociate')?.disable();

          const SELECTED_USER_IS_ADMIN = this.personalData.get('function')?.value == '2';

          if(SELECTED_USER_IS_ADMIN){
            this.personalData.get('email')?.disable();
          }
        },
        error: () => {
          this.showErrorMessage("Algo deu errado", true);
        }
      });
    }
  }

  personalData = this._formBuilder.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    birthdate: ['', [Validators.required]],
    gender: ['', Validators.required],
    tax: ['', [Validators.required], [validateCpf]],
    seniorCodeRepresentation: ['', [Validators.required]],
    codeRepresentation: [''],
    documentPhoto: ['', [Validators.required]],
    documentLink: [''] ,
    function: ['', [Validators.required]],
    codeAssociate: [''],
    status: [null as number | null],
    courtOrder: new FormControl<File | string | null>(null, Validators.required),
    associativeTerm: new FormControl<File | string | null>(null, Validators.required),
    contractTerm: new FormControl<File | string | null>(null, Validators.required),
  });

  addressData = this._formBuilder.group({
    zip: ['', [Validators.required, Validators.maxLength(9), Validators.minLength(8)]],
    street: ['', [Validators.required, Validators.maxLength(100), Validators.minLength(3)]],
    number: ['', [Validators.required, Validators.maxLength(10), Validators.minLength(1)]],
    district: ['', [Validators.required, Validators.maxLength(30), Validators.minLength(3)]],
    city: ['', Validators.required],
    state: ['', [Validators.required, Validators.maxLength(2), Validators.minLength(2), Validators.pattern(/^[A-Za-z]{2}$/)]],
    proofOfResidence: [null, [Validators.required]],
    proofLink: [''] 
  });

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
  }

  onMultipleFilesSelected(event: Event) {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length >= 3) {
      const files = Array.from(input.files);

      const associative = files.find(file =>
        /ficha.*associativa/i.test(file.name.toLowerCase())
      );
      
      const contract = files.find(file =>
        /termo.*ades[aã]o/i.test(file.name.toLowerCase())
      );

      const order = files.find(file =>
        /requerimento.*/i.test(file.name.toLowerCase())
      );
      console.log("valor do requerimento", order)

      if(!associative || !contract || !order){
        alert("Você deve anexar os três documentos: Ficha Associativa, Termo de Adesão e Requerimento judicial.");
      }
      else {
        this.personalData.patchValue({ associativeTerm: associative });
        this.personalData.get('associativeTerm')?.updateValueAndValidity();
        
        this.personalData.patchValue({ contractTerm: contract });
        this.personalData.get('contractTerm')?.updateValueAndValidity();
        
        this.personalData.patchValue({ courtOrder:  order});
        this.personalData.get('courtOrder')?.updateValueAndValidity();
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
    if (form.invalid) {
      event.preventDefault();
      form.markAllAsTouched();
      this.showErrorMessage("Algo deu errado", true);
    } else {
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

  handleClick(event: any, termData: FormGroup){
    this.verifyForm(event, termData);
    this.register();
  }

  register() {
    if(this.addressData.invalid || this.personalData.invalid){
      this.showErrorMessage("Todos os campos do formulário precisam ser preenchidos para concluir o cadastro.", true);
      return;
    }
    this.loading.show();

    if(this.associate) {
      const userData = this.getUserFromForm();
      userData.append("id", this.associate.id);
      userData.append("enderecoDto.id", this.addressId.toString());
      userData.append("cpfUploadUrl", this.associate.cpfUploadUrl);
      userData.append("enderecoDto.comprovanteDeResidenciaUpload", this.addressPhoto);
      userData.append("fichaAssociadoDto.fichaAssociacaoUploadUrl", this.associate.fichaAssociadoDto.fichaAssociacaoUploadUrl);
      userData.append("termoAdesaoDto.termoAdesaoUploadUrl", this.associate.termoAdesaoDto.termoAdesaoUploadUrl);
      userData.append('requerimentoJudicialDto.urlDoRequerimento', this.associate.requerimentoJudicialDto.urlDoRequerimento);

      console.log("Valor do requerimento:", this.associate.requerimentoJudicialDto.urlDoRequerimento);

      this.service.updateUser(userData, this.associate.id).pipe(
        finalize(() => this.loading.hide())
      )
      .subscribe({
        next: () => {
          this.dialog.open(UserCreatedModalComponent).afterClosed().subscribe(() => {
            this.stepper.reset();
            this.dialogRef.close('refresh');
          });
        },
        error: () => {
          this.showErrorMessage("Algo deu errado", true);
        }
      })
    }
    else {
      const userData = this.getUserFromForm();
      this.service.createNewUser(userData).pipe(
        finalize(() => this.loading.hide())
      )
      .subscribe({
        next: () => {
          this.dialog.open(UserCreatedModalComponent).afterClosed().subscribe(() => {
            this.dialogRef.close('refresh');
          });
        },
        error: (error) => {
          this.showErrorMessage("Algo deu errado", true);
        }
      })
    }
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
    const codeAssociate = this.personalData.get('codeAssociate')?.value;
    const documentPhoto = this.personalData.get('documentPhoto')?.value;
    const userStatus = this.personalData.get('status')?.value ?? 1;
    const courtOrder = this.personalData.get('courtOrder')?.value;
    const userFunction = this.personalData.get('function')?.value;
    const associativeTerm = this.personalData.get('associativeTerm')?.value;
    const contractTerm = this.personalData.get('contractTerm')?.value;
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
    userDataForm.append('codigoAssociado', codeAssociate!);
    userDataForm.append('fotoDoDocumento', documentPhoto!);
    userDataForm.append('funcao', userFunction!);
    userDataForm.append('status', userStatus!.toString());
    userDataForm.append('fichaAssociadoDto.fichaAssociacao', associativeTerm!);
    userDataForm.append('requerimentoJudicialDto.arquivoDoRequerimento', courtOrder!);
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

    setTimeout(() => {
      this.showErrorPopup = false;
    } , 10000);
  }

  verifyCodeRepresentationFormToSubmit(personalData:FormGroup): Observable<void>{
    return new Observable<void>(observer => {
        const seniorCodeRepresentation = personalData.get('seniorCodeRepresentation')?.value;
  
        this.service.getUserCodeRepresentation(seniorCodeRepresentation!).subscribe({
          next: response => {
            observer.next();
            observer.complete();
          },
          error: err => {
            observer.error(err);
          }
        });
      })
  }

  close() {
    this.dialogRef.close('refresh');
  }
}
