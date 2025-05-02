import { AddressModel } from "../address/addressModel";
import { FichaAssociadoDto } from "./fichaAssociadoDto";
import { TermoAdesaoDto } from "./termoAdesaoDto";

export interface UserModel {
    id: string;
    nome: string;
    email: string;
    dataDeNascimento:string;
    genero: string;
    cpf: string;
    fotoDoDocumento: File;
    cpfUploadUrl: string;
    codigoRepresentante: string;
    codigoRepresentanteSuperior: string;
    enderecoDto: AddressModel;
    dataDeCadastro: string;
    fichaAssociadoDto: FichaAssociadoDto; 
    termoAdesaoDto: TermoAdesaoDto;
}