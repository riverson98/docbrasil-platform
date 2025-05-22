import { AbstractControl, ValidationErrors } from "@angular/forms";
import { map, Observable, of } from "rxjs";

export function validateCpf(control: AbstractControl) : Observable<ValidationErrors | null> {
    return of(control.value?.replace(/\D/g, '')).pipe( 
        map((cpf: string | undefined) => {
          if (!cpf || cpf.length !== 11 || /^(\d)\1{10}$/.test(cpf)) {
            return { invalidCpf: true };
          }
    
          let sum = 0;
          let remainder;
          
          for (let i = 0; i < 9; i++) sum += parseInt(cpf.charAt(i)) * (10 - i);
          remainder = (sum * 10) % 11;
          if (remainder === 10 || remainder === 11) remainder = 0;
          if (remainder !== parseInt(cpf.charAt(9))) return { cpfInvalido: true };
    
          sum = 0;
          for (let i = 0; i < 10; i++) sum += parseInt(cpf.charAt(i)) * (11 - i);
          remainder = (sum * 10) % 11;
          if (remainder === 10 || remainder === 11) remainder = 0;
          if (remainder !== parseInt(cpf.charAt(10))) return { cpfInvalido: true };
    
          return null;
        })
      );
}

export function getStatusClass(status: number): string {
  switch (status) {
    case 1:
      return 'user-active';
    case 2:
      return 'user-inactive';
    case 3:
      return 'user-pending';
    default:
      return 'status-desconhecido';
  }
}

export function togglePassword(inputHtml:string, iconHtml:string):void {
  const passwordInput = document.getElementById(inputHtml) as HTMLInputElement;
   
  if(passwordInput){
      const type = passwordInput.type === "password" ? "text" : "password";
      passwordInput.type = type;
      const icon = document.getElementById(iconHtml) as HTMLElement;

      if (icon) {
          icon.classList.toggle("fa-eye");
          icon.classList.toggle("fa-eye-slash");
      }
  }
};