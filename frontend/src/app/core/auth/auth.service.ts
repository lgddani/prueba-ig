import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { LoginRequest, LoginResponse, UsuarioDto } from '../models/auth.models';

const TOKEN_KEY = 'kanban.token';
const USUARIO_KEY = 'kanban.usuario';

@Injectable({ providedIn: 'root' })
export class AuthService {
    private readonly usuarioSignal = signal<UsuarioDto | null>(this.leerUsuarioAlmacenado());
    readonly usuario = this.usuarioSignal.asReadonly();
    readonly estaAutenticado = computed(() => this.usuarioSignal() !== null && !!this.token);

    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    async login(request: LoginRequest): Promise<void> {
        const response = await firstValueFrom(
            this.http.post<LoginResponse>(`${this.config.apiUrl}/auth/login`, request)
        );

        localStorage.setItem(TOKEN_KEY, response.token);
        localStorage.setItem(USUARIO_KEY, JSON.stringify(response.usuario));
        this.usuarioSignal.set(response.usuario);
    }

    logout(): void {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USUARIO_KEY);
        this.usuarioSignal.set(null);
    }

    get token(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }

    private leerUsuarioAlmacenado(): UsuarioDto | null {
        const raw = localStorage.getItem(USUARIO_KEY);
        return raw ? (JSON.parse(raw) as UsuarioDto) : null;
    }
}
