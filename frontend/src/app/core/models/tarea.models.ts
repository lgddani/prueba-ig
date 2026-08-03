export type Prioridad = 'Baja' | 'Media' | 'Alta' | 'Urgente';

export interface TareaDto {
    id: string;
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    responsableId?: string | null;
    responsableNombre?: string | null;
    columnaId: string;
    orden: number;
    fechaCreacion: string;
}

export interface CrearTareaRequest {
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    responsableId?: string | null;
    columnaId: string;
}

export interface ActualizarTareaRequest {
    titulo: string;
    descripcion?: string | null;
    prioridad: Prioridad;
    responsableId?: string | null;
}

export interface MoverTareaRequest {
    columnaDestinoId: string;
    posicionDestino: number;
}
