import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { UsuarioDto } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class UsuariosService {
    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    listar(): Observable<UsuarioDto[]> {
        return this.http.get<UsuarioDto[]>(`${this.config.apiUrl}/usuarios`);
    }
}
