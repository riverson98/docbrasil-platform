import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const authGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const token = localStorage.getItem('token');

  console.log("Valor da rota fora do token:", state.url);
  if(token){
    console.log("Valor da rota dentro do token:", state.url);
    if(state.url === '/' || state.url === ''){
      router.navigate(['/painel/associados']);
      return false;
    }
    return true;
  } 
  
  if (state.url.startsWith('/painel')) {
      router.navigate(['/']);
    return false;
  }
  
  return true;
};
