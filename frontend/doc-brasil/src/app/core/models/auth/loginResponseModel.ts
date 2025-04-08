export interface LoginResponseModel {
    id:string
    message:string;
    isAuthenticated:boolean;
    roles:[];
    token:string;
    refreshToken:string;
    refreshTokenExpiration:Date;
    isRegistrationCompleted:boolean;
}