import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environmentDev } from "../../environment/environment";
import { UserModel } from "../../models/user/userModel";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class UserService {
    apiUrl:string = 'http://20.197.248.228:8090/associado';
    headers:HttpHeaders = new HttpHeaders({
        'X-Api-Key': environmentDev.xApiKeyAssociados
    })

    constructor(private http:HttpClient) {
        
    }

    createNewUser(userData: FormData): Observable<UserModel> {
        return this.http.post<UserModel>(
                `${this.apiUrl}`,
                 userData,
                { headers: this.headers }
            );
    }

    getUserById(id: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/${id}`,
            { headers: this.headers }
        );
    }

    getUserCodeRepresentation(code: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/busca-por-codigo-representante/${code}`,
            { headers: this.headers }
        );
    }

    getUserWithAddressById(id: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/com-detalhes/${id}`,
            { headers: this.headers }
        );
    }
}