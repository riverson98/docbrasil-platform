import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';

import { RegisterResponseModel } from '../../models/auth/RegisterResponseModel';
import { Router } from '@angular/router';
import { LoginResponseModel } from '../../models/auth/loginResponseModel';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { LoginModel } from '../../models/auth/loginModel';
import { updatePassword } from '../../models/auth/updatePassword';
import { finalize } from 'rxjs';
import { LoadingService } from '../loading/loading.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl:string = 'https://appdocdobrasil.com.br/auth';;

  constructor(private http:HttpClient, private router: Router, 
    private loading: LoadingService) {}
  
  register(userData: RegisterResponseModel): Observable<RegisterResponseModel> {
     return this.http.post<RegisterResponseModel>(
        `${this.apiUrl}/register`, userData
    );
  }

  login(userData:LoginModel): Observable<LoginResponseModel> {
    return this.http.post<LoginResponseModel>(
        `${this.apiUrl}/login`, userData
    );
  }

  refreshToken(): Observable<any> {
    return this.http.get(
        `${this.apiUrl}/refresh-token`, {withCredentials: true}
    );
  }

  updatePassword(passwordData: updatePassword): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/update-password`, passwordData
    );
  }

  logout(): void {
    this.loading.show();
    const userEmail = this.getUserEmail();
    
    this.http.get(
      `${this.apiUrl}/logout/${userEmail}`
    ).pipe(
      finalize(() => this.loading.hide())
    )
    .subscribe(() => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    localStorage.removeItem('userPhoto');
    this.router.navigate(['/']); 
    })
  }

  getUserId(): string | undefined {
    const token = localStorage.getItem('token');
    
    if(token){
      const decodedToken = jwtDecode(token) as JwtPayload & { uid: string };
      return decodedToken.uid || undefined;
    }
    return undefined;
  }

  getToken(){
    const token = localStorage.getItem('token');

    if(token){
      return token;
    }
    else{
      return undefined;
    }
  };
  
  getUserEmail(): string | undefined {
    const token = localStorage.getItem('token');
    if (token) {
      const decodedToken: any = jwtDecode(token) as JwtPayload & { email: string };;
      return decodedToken.email || null;
    }
    return undefined;
  }
}


