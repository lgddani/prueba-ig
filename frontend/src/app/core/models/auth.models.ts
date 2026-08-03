export interface LoginRequest {
    correo: string;
    password: string;
}

export interface UsuarioDto {
    id: string;
    nombre: string;
    correo: string;
}

export interface LoginResponse {
    token: string;
    expiraUtc: string;
    usuario: UsuarioDto;
}
