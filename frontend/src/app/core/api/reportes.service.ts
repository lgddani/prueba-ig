import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';

@Injectable({ providedIn: 'root' })
export class ReportesService {
    constructor(
        private readonly http: HttpClient,
        private readonly config: AppConfigService
    ) {}

    descargarPdf(proyectoId: string): Observable<HttpResponse<Blob>> {
        return this.descargar(proyectoId, 'pdf');
    }

    descargarExcel(proyectoId: string): Observable<HttpResponse<Blob>> {
        return this.descargar(proyectoId, 'excel');
    }

    private descargar(proyectoId: string, formato: 'pdf' | 'excel'): Observable<HttpResponse<Blob>> {
        return this.http.get(`${this.config.apiUrl}/proyectos/${proyectoId}/reportes/${formato}`, {
            observe: 'response',
            responseType: 'blob'
        });
    }
}
