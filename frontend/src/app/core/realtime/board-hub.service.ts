import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { AppConfigService } from '../config/app-config.service';
import { TareaDto } from '../models/tarea.models';

export interface TareaEliminadaEvento {
    tareaId: string;
    columnaId: string;
}

export interface TareaMovidaEvento {
    tarea: TareaDto;
    columnaOrigenId: string;
}

/**
 * Cliente del hub SignalR "board:{proyectoId}". Una única conexión viva a la vez;
 * al cambiar de tablero o destruir el componente se cierra la suscripción y la
 * conexión explícitamente para no dejar conexiones huérfanas.
 */
@Injectable({ providedIn: 'root' })
export class BoardHubService {
    private connection?: signalR.HubConnection;
    private proyectoSuscritoId?: string;

    private readonly tareaCreadaSubject = new Subject<TareaDto>();
    private readonly tareaActualizadaSubject = new Subject<TareaDto>();
    private readonly tareaEliminadaSubject = new Subject<TareaEliminadaEvento>();
    private readonly tareaMovidaSubject = new Subject<TareaMovidaEvento>();
    private readonly usuariosConectadosSubject = new Subject<string[]>();

    readonly tareaCreada$ = this.tareaCreadaSubject.asObservable();
    readonly tareaActualizada$ = this.tareaActualizadaSubject.asObservable();
    readonly tareaEliminada$ = this.tareaEliminadaSubject.asObservable();
    readonly tareaMovida$ = this.tareaMovidaSubject.asObservable();
    readonly usuariosConectados$ = this.usuariosConectadosSubject.asObservable();

    constructor(
        private readonly config: AppConfigService,
        private readonly auth: AuthService
    ) {}

    async conectarYSuscribir(proyectoId: string): Promise<void> {
        await this.desconectar();

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(`${this.config.signalrUrl}?access_token=${this.auth.token}`)
            .withAutomaticReconnect()
            .build();

        this.connection.on('TareaCreada', (tarea: TareaDto) => this.tareaCreadaSubject.next(tarea));
        this.connection.on('TareaActualizada', (tarea: TareaDto) => this.tareaActualizadaSubject.next(tarea));
        this.connection.on('TareaEliminada', (evento: TareaEliminadaEvento) => this.tareaEliminadaSubject.next(evento));
        this.connection.on('TareaMovida', (evento: TareaMovidaEvento) => this.tareaMovidaSubject.next(evento));
        this.connection.on('UsuariosConectados', (nombres: string[]) => this.usuariosConectadosSubject.next(nombres));

        this.connection.onreconnected(async () => {
            if (this.proyectoSuscritoId) {
                await this.connection?.invoke('SuscribirseATablero', this.proyectoSuscritoId);
            }
        });

        await this.connection.start();
        await this.connection.invoke('SuscribirseATablero', proyectoId);
        this.proyectoSuscritoId = proyectoId;
    }

    async desconectar(): Promise<void> {
        if (!this.connection) {
            return;
        }

        try {
            if (this.proyectoSuscritoId && this.connection.state === signalR.HubConnectionState.Connected) {
                await this.connection.invoke('DesuscribirseDeTablero', this.proyectoSuscritoId);
            }
        } finally {
            await this.connection.stop();
            this.connection = undefined;
            this.proyectoSuscritoId = undefined;
        }
    }
}
