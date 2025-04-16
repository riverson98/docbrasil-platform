import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserModel } from "../../models/user/userModel";
import { Observable } from "rxjs";
import { PaginationParamsRequest } from "../../models/paginationParams/paginationParamsRequest";
import { PaginationParamsResponse } from "../../models/paginationParams/paginationParamsResponse";

@Injectable({
  providedIn: 'root'
})
export class UserService {
    apiUrl:string = 'https://appdocdobrasil.com.br/associado';

    constructor(private http:HttpClient) {
        
    }

    createNewUser(userData: FormData): Observable<UserModel> {
        return this.http.post<UserModel>(
                `${this.apiUrl}`, userData
            );
    }

    getUserById(id: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/${id}`
        );
    }

    getUserCodeRepresentation(code: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/busca-por-codigo-representante/${code}`
        );
    }

    getUserWithAddressById(id: string): Observable<UserModel>{
        return this.http.get<UserModel>(
            `${this.apiUrl}/com-detalhes/${id}`
        );
    }

    getAssociates(params: PaginationParamsRequest): Observable<PaginationParamsResponse>{
        const httpParams = new HttpParams({fromObject: {
            Pagina:params.pagina,
            QuantidadeDeItensPorPagina: params.quantidadeDeItensPorPagina,
            Query: params.query
        }})

        return this.http.get<PaginationParamsResponse>(
            `${this.apiUrl}/busca-associados`,
            {params: httpParams}
        )
    }

    getMembers(params: PaginationParamsRequest): Observable<PaginationParamsResponse>{
        const httpParams = new HttpParams({fromObject: {
            Pagina:params.pagina,
            QuantidadeDeItensPorPagina: params.quantidadeDeItensPorPagina,
            Query: params.query
        }})

        return this.http.get<PaginationParamsResponse>(
            `${this.apiUrl}/busca-membros`,
            {params: httpParams}
        )
    }
}