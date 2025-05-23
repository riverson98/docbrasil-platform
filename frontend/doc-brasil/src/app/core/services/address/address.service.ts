import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AddressModel } from "../../models/address/addressModel";

@Injectable({
  providedIn: 'root'
})
export class AddressService {
    apiUrl:string = 'https://appdocdobrasil.com.br/endereco';
    
    constructor(private http:HttpClient) {}

    getAddressByAssociateId(id: string): Observable<AddressModel>{
        return this.http.get<AddressModel>(
           `${this.apiUrl}/${id}`
        );
    }
}