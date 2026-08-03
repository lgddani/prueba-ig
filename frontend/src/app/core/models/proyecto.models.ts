export type EstadoProyecto = 'Planificado' | 'EnProgreso' | 'Pausado' | 'Completado' | 'Cancelado';

export interface ProyectoDto {
    id: string;
    nombre: string;
    descripcion?: string | null;
    fechaInicio: string;
    fechaFinPrevista: string;
    estado: EstadoProyecto;
    fechaCreacion: string;
    totalColumnas: number;
}

export interface CrearProyectoRequest {
    nombre: string;
    descripcion?: string | null;
    fechaInicio: string;
    fechaFinPrevista: string;
    estado: EstadoProyecto;
}

export type ActualizarProyectoRequest = CrearProyectoRequest;

export interface PagedResult<T> {
    items: T[];
    paginaActual: number;
    tamanoPagina: number;
    totalRegistros: number;
    totalPaginas: number;
}
