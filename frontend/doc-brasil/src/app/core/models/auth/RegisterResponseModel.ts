export interface RegisterResponseModel {
    id: string;
    token:string;
    refreshToken:string;
    success:boolean;
    errors:string[];
    isRegistrationCompleted:boolean;
}