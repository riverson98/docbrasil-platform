import { AddressModel } from "../address/addressModel";
import { FichaAssociadoDto } from "./fichaAssociadoDto";
import { RequerimentoJudicialDto } from "./RequerimentoJudicialDto";
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
    codigoAssociado: string;
    funcao: string;
    status: number;
    enderecoDto: AddressModel;
    dataDeCadastro: string;
    fichaAssociadoDto: FichaAssociadoDto; 
    termoAdesaoDto: TermoAdesaoDto;
    requerimentoJudicialDto: RequerimentoJudicialDto;
}