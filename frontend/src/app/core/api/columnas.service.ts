import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { ActualizarColumnaRequest, ColumnaDto, CrearColumnaRequest, ReordenarColumnasRequest } from '../models/columna.models';

@Injectable({ providedIn: 'root' })
export class ColumnasService {
    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    private baseUrl(proyectoId: string): string {
        return `${this.config.apiUrl}/proyectos/${proyectoId}/columnas`;
    }

    listar(proyectoId: string): Observable<ColumnaDto[]> {
        return this.http.get<ColumnaDto[]>(this.baseUrl(proyectoId));
    }

    crear(proyectoId: string, request: CrearColumnaRequest): Observable<ColumnaDto> {
        return this.http.post<ColumnaDto>(this.baseUrl(proyectoId), request);
    }

    actualizar(proyectoId: string, columnaId: string, request: ActualizarColumnaRequest): Observable<ColumnaDto> {
        return this.http.put<ColumnaDto>(`${this.baseUrl(proyectoId)}/${columnaId}`, request);
    }

    eliminar(proyectoId: string, columnaId: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl(proyectoId)}/${columnaId}`);
    }

    reordenar(proyectoId: string, request: ReordenarColumnasRequest): Observable<void> {
        return this.http.put<void>(`${this.baseUrl(proyectoId)}/reordenar`, request);
    }
}
