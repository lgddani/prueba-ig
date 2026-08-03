import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { ActualizarProyectoRequest, CrearProyectoRequest, PagedResult, ProyectoDto } from '../models/proyecto.models';

@Injectable({ providedIn: 'root' })
export class ProyectosService {
    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    private get baseUrl(): string {
        return `${this.config.apiUrl}/proyectos`;
    }

    listar(pagina: number, tamanoPagina: number, nombre?: string): Observable<PagedResult<ProyectoDto>> {
        let params: Record<string, string> = { pagina: String(pagina), tamanoPagina: String(tamanoPagina) };
        if (nombre) {
            params = { ...params, nombre };
        }
        return this.http.get<PagedResult<ProyectoDto>>(this.baseUrl, { params });
    }

    obtenerPorId(id: string): Observable<ProyectoDto> {
        return this.http.get<ProyectoDto>(`${this.baseUrl}/${id}`);
    }

    crear(request: CrearProyectoRequest): Observable<ProyectoDto> {
        return this.http.post<ProyectoDto>(this.baseUrl, request);
    }

    actualizar(id: string, request: ActualizarProyectoRequest): Observable<ProyectoDto> {
        return this.http.put<ProyectoDto>(`${this.baseUrl}/${id}`, request);
    }

    eliminar(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
