import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface RuntimeConfig {
    apiUrl: string;
    signalrUrl: string;
}

/**
 * Configuración externa del frontend: se lee en runtime desde assets/config.json
 * (no desde código embebido en componentes/servicios). En Docker, el contenedor
 * de la SPA regenera este archivo a partir de variables de entorno antes de servir
 * los estáticos con nginx, de modo que la misma imagen sirve para cualquier entorno
 * sin reconstruir el build de Angular.
 */
@Injectable({ providedIn: 'root' })
export class AppConfigService {
    private config?: RuntimeConfig;

    constructor(private readonly http: HttpClient) {}

    async load(): Promise<void> {
        this.config = await firstValueFrom(this.http.get<RuntimeConfig>('assets/config.json'));
    }

    get apiUrl(): string {
        if (!this.config) {
            throw new Error('AppConfigService no ha sido inicializado todavía.');
        }
        return this.config.apiUrl;
    }

    /** Variante segura para el interceptor: se invoca antes de que load() resuelva
     * (la propia petición a assets/config.json también pasa por el interceptor). */
    get apiUrlOrNull(): string | null {
        return this.config?.apiUrl ?? null;
    }

    get signalrUrl(): string {
        if (!this.config) {
            throw new Error('AppConfigService no ha sido inicializado todavía.');
        }
        return this.config.signalrUrl;
    }
}
