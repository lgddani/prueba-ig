import { TestBed } from '@angular/core/testing';
import { Router, UrlTree } from '@angular/router';
import { authGuard } from './auth.guard';
import { AuthService } from './auth.service';

describe('authGuard', () => {
    let authServiceStub: { estaAutenticado: () => boolean };
    let routerStub: { createUrlTree: jasmine.Spy };

    beforeEach(() => {
        authServiceStub = { estaAutenticado: () => false };
        routerStub = { createUrlTree: jasmine.createSpy('createUrlTree').and.returnValue({} as UrlTree) };

        TestBed.configureTestingModule({
            providers: [
                { provide: AuthService, useValue: authServiceStub },
                { provide: Router, useValue: routerStub }
            ]
        });
    });

    function ejecutarGuard() {
        return TestBed.runInInjectionContext(() =>
            authGuard({} as any, { url: '/proyectos' } as any)
        );
    }

    it('permite el acceso cuando hay una sesión válida', () => {
        authServiceStub.estaAutenticado = () => true;

        const resultado = ejecutarGuard();

        expect(resultado).toBeTrue();
    });

    it('redirige al login cuando no hay sesión válida', () => {
        authServiceStub.estaAutenticado = () => false;

        ejecutarGuard();

        expect(routerStub.createUrlTree).toHaveBeenCalledWith(['/auth/login'], { queryParams: { redirectTo: '/proyectos' } });
    });
});
