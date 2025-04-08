import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';

import { RegisterResponseModel } from '../../models/auth/RegisterResponseModel';
import { Router } from '@angular/router';
import { RegisterModel } from '../../models/auth/registerModel';
import { LoginResponseModel } from '../../models/auth/loginResponseModel';
import { jwtDecode, JwtPayload } from 'jwt-decode';
import { environmentDev } from '../../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  apiUrl:string = 'http://associados_api-gateway:8090/auth';
  headers:HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
    'X-Api-Key': environmentDev.xApiKeyAuth
  })

  constructor(private http:HttpClient, private router: Router) {}
  
  register(userData: RegisterResponseModel): Observable<RegisterResponseModel> {
     return this.http.post<RegisterResponseModel>(
        `${this.apiUrl}/register`,
         userData,
        { headers: this.headers }
    );
  }

  login(userData:RegisterModel): Observable<LoginResponseModel> {
    return this.http.post<LoginResponseModel>(
        `${this.apiUrl}/login`, 
        userData,
        { headers: this.headers }
    );
  }

  refreshToken(): Observable<any> {
    const refreshToken = localStorage.getItem('refreshToken');
    return this.http.post(
        `${this.apiUrl}/refreshtoken`, 
        { refreshToken },
        {headers: this.headers}
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.router.navigate(['/auth-login']); 
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


