export interface AddressModel {
    id: number;
    cep: string;
    rua: string;
    numero: string;
    bairro: string;
    cidade: string;
    estado: string;
    fotoDoComprovante: File;
    urlDaFotoDoComprovante: string;
}