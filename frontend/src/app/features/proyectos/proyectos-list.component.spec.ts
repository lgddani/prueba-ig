import { FormBuilder } from '@angular/forms';
import { ProyectosListComponent } from './proyectos-list.component';

function crearComponente(): ProyectosListComponent {
    const stub = {} as any;
    return new ProyectosListComponent(stub, stub, new FormBuilder(), stub, stub);
}

describe('ProyectosListComponent - lógica de aplicación', () => {
    it('severidadEstado mapea cada estado de proyecto a la severidad visual esperada', () => {
        const componente = crearComponente();

        expect(componente.severidadEstado('Planificado')).toBe('secondary');
        expect(componente.severidadEstado('EnProgreso')).toBe('info');
        expect(componente.severidadEstado('Pausado')).toBe('warning');
        expect(componente.severidadEstado('Completado')).toBe('success');
        expect(componente.severidadEstado('Cancelado')).toBe('danger');
    });
});
