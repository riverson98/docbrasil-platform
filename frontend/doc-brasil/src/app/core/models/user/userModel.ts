import { AddressModel } from "../address/addressModel";

export interface UserModel {
    id: string;
    nome: string;
    email: string;
    dataDeNascimento:string;
    genero: string;
    cpf: string;
    fotoDoDocumento: File;
    urlDaFotoDoDocumento: string;
    codigoRepresentante: string;
    codigoRepresentanteSuperior: string;
    endereco: AddressModel;
    dataDeCadastro: string;
}