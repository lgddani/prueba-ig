import { FormBuilder } from '@angular/forms';
import { TareaDto } from '../../core/models/tarea.models';
import { TableroComponent } from './tablero.component';

function crearTarea(overrides: Partial<TareaDto>): TareaDto {
    return {
        id: overrides.id ?? 't1',
        titulo: overrides.titulo ?? 'Tarea',
        descripcion: null,
        prioridad: overrides.prioridad ?? 'Media',
        responsableId: overrides.responsableId ?? null,
        responsableNombre: null,
        columnaId: overrides.columnaId ?? 'col-1',
        orden: overrides.orden ?? 0,
        fechaCreacion: new Date().toISOString()
    };
}

// Se instancia la clase directamente (sin TestBed) porque los métodos bajo prueba
// son lógica de aplicación pura sobre los signals del componente, sin tocar DOM ni HTTP.
function crearComponente(): TableroComponent {
    const stub = {} as any;
    // fb debe ser un FormBuilder real: los campos de formulario del componente se
    // inicializan como class fields (this.fb.group(...)) antes de que corra el resto
    // del constructor, así que un stub vacío rompería la construcción del objeto.
    return new TableroComponent(stub, stub, new FormBuilder(), stub, stub, stub, stub, stub, stub, stub, stub);
}

describe('TableroComponent - lógica de aplicación', () => {
    it('tareasDeColumna filtra por columna y ordena por el campo orden', () => {
        const componente = crearComponente();
        componente.tareas.set([
            crearTarea({ id: 'a', columnaId: 'col-1', orden: 200 }),
            crearTarea({ id: 'b', columnaId: 'col-2', orden: 100 }),
            crearTarea({ id: 'c', columnaId: 'col-1', orden: 100 })
        ]);

        const resultado = componente.tareasDeColumna('col-1');

        expect(resultado.map((t) => t.id)).toEqual(['c', 'a']);
    });

    it('el filtro de texto es insensible a mayúsculas y aplica sobre el título', () => {
        const componente = crearComponente();
        componente.tareas.set([
            crearTarea({ id: 'a', titulo: 'Diseñar login', columnaId: 'col-1' }),
            crearTarea({ id: 'b', titulo: 'Implementar API', columnaId: 'col-1' })
        ]);
        componente.filtroTexto.set('LOGIN');

        const resultado = componente.tareasDeColumna('col-1');

        expect(resultado.map((t) => t.id)).toEqual(['a']);
    });

    it('el filtro de prioridad combinado con responsable reduce correctamente el listado', () => {
        const componente = crearComponente();
        componente.tareas.set([
            crearTarea({ id: 'a', prioridad: 'Alta', responsableId: 'u1', columnaId: 'col-1' }),
            crearTarea({ id: 'b', prioridad: 'Alta', responsableId: 'u2', columnaId: 'col-1' }),
            crearTarea({ id: 'c', prioridad: 'Baja', responsableId: 'u1', columnaId: 'col-1' })
        ]);
        componente.filtroPrioridad.set('Alta');
        componente.filtroResponsableId.set('u1');

        const resultado = componente.tareasDeColumna('col-1');

        expect(resultado.map((t) => t.id)).toEqual(['a']);
    });

    it('severidadPrioridad mapea cada prioridad a la severidad visual esperada', () => {
        const componente = crearComponente();

        expect(componente.severidadPrioridad('Baja')).toBe('success');
        expect(componente.severidadPrioridad('Media')).toBe('info');
        expect(componente.severidadPrioridad('Alta')).toBe('warning');
        expect(componente.severidadPrioridad('Urgente')).toBe('danger');
    });
});
