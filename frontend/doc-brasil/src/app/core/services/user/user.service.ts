import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { UserModel } from "../../models/user/userModel";
import { Observable } from "rxjs";
import { PaginationParamsRequest } from "../../models/paginationParams/paginationParamsRequest";
import { PaginationParamsResponse } from "../../models/paginationParams/paginationParamsResponse";
import { AssociateSummary } from "../../models/user/associateSummary";
import { AssociadoResumidoDto } from "../../models/user/AssociadoResumidoDto";
import { ProfilePhotoRequest } from "../../models/user/ProfilePhotoRequest";
import { UrlUpdated } from "../../models/user/urlUpdated";

@Injectable({
  providedIn: 'root'
})
export class UserService {
    apiUrl:string = 'https://appdocdobrasil.com.br/associado';

    constructor(private http:HttpClient) {
        
    }

    createNewUser(userData: FormData): Observable<UserModel> {
        return this.http.post<UserModel>(
                `${this.apiUrl}/cria-associado`, userData
            );
    }

    updateUser(userData: FormData, userId:string): Observable<UserModel> {
        return this.http.put<UserModel>(
                `${this.apiUrl}/${userId}`, userData
            );
    }

    updateSummary(data: AssociadoResumidoDto): Observable<AssociadoResumidoDto> {
        const patchDoc = [
            {
                "op": "replace",
                "path": "/nome",
                "value": data.nome
            },
            {
                "op": "replace",
                "path": "/dataDeNascimento",
                "value": data.dataDeNascimento
            },
            {
                "op": "replace",
                "path": "/genero",
                "value": data.genero
            },
        ];
        return this.http.patch<AssociadoResumidoDto>(
            `${this.apiUrl}/${data.id}`, patchDoc
        )
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

    getUserSummary(id:string): Observable<AssociateSummary>{
        return this.http.get<AssociateSummary>(
            `${this.apiUrl}/associado-resumido/${id}`
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

    deleteUser(id:string): Observable<any>{
        return this.http.delete(
            `${this.apiUrl}/deleta-associado/${id}`
        )
    }

    saveProfilePhoto(userId: string, file: File): Observable<ProfilePhotoRequest>{
        const userDataPhoto = new FormData();
        userDataPhoto.append("ArquivoDaFotoDePerfil", file);

        return this.http.post<ProfilePhotoRequest>(
            `${this.apiUrl}/adiciona-foto-de-perfil/${userId}`, userDataPhoto
        );
    }
    
    updateDocsUrl(id: string): Observable<UrlUpdated>{
      return this.http.put<UrlUpdated>(
            `${this.apiUrl}/atualiza-urls/${id}`, {}
        );
    }

    getUserName(): string{
        const username = localStorage.getItem('username')!;
        const firstName = username?.split(' ')[0];
        return firstName.charAt(0).toUpperCase() + firstName.slice(1).toLowerCase();
    }
}
