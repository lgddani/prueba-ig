export interface ColumnaDto {
    id: string;
    nombre: string;
    orden: number;
    proyectoId: string;
    totalTareas: number;
}

export interface CrearColumnaRequest {
    nombre: string;
}

export type ActualizarColumnaRequest = CrearColumnaRequest;

export interface ReordenarColumnasRequest {
    columnaIdsEnOrden: string[];
}
