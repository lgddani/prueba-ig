import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { ActualizarTareaRequest, CrearTareaRequest, MoverTareaRequest, TareaDto } from '../models/tarea.models';

@Injectable({ providedIn: 'root' })
export class TareasService {
    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    private baseUrl(proyectoId: string): string {
        return `${this.config.apiUrl}/proyectos/${proyectoId}/tareas`;
    }

    listar(proyectoId: string): Observable<TareaDto[]> {
        return this.http.get<TareaDto[]>(this.baseUrl(proyectoId));
    }

    crear(proyectoId: string, request: CrearTareaRequest): Observable<TareaDto> {
        return this.http.post<TareaDto>(this.baseUrl(proyectoId), request);
    }

    actualizar(proyectoId: string, tareaId: string, request: ActualizarTareaRequest): Observable<TareaDto> {
        return this.http.put<TareaDto>(`${this.baseUrl(proyectoId)}/${tareaId}`, request);
    }

    eliminar(proyectoId: string, tareaId: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl(proyectoId)}/${tareaId}`);
    }

    mover(proyectoId: string, tareaId: string, request: MoverTareaRequest): Observable<TareaDto> {
        return this.http.post<TareaDto>(`${this.baseUrl(proyectoId)}/${tareaId}/mover`, request);
    }
}
