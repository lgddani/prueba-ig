import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppConfigService } from '../config/app-config.service';
import { AuthService } from './auth.service';

describe('AuthService', () => {
    let service: AuthService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        localStorage.clear();

        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [AuthService, { provide: AppConfigService, useValue: { apiUrl: 'http://api.test/api' } }]
        });

        service = TestBed.inject(AuthService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpMock.verify());

    it('estaAutenticado es false cuando no hay sesión almacenada', () => {
        expect(service.estaAutenticado()).toBeFalse();
        expect(service.token).toBeNull();
    });

    it('login almacena el token y el usuario, y actualiza el signal', async () => {
        const promesa = service.login({ correo: 'ana@kanban.dev', password: 'Kanban#2026' });

        const req = httpMock.expectOne('http://api.test/api/auth/login');
        expect(req.request.method).toBe('POST');
        req.flush({
            token: 'jwt-de-prueba',
            expiraUtc: new Date().toISOString(),
            usuario: { id: '1', nombre: 'Ana Torres', correo: 'ana@kanban.dev' }
        });

        await promesa;

        expect(service.token).toBe('jwt-de-prueba');
        expect(service.estaAutenticado()).toBeTrue();
        expect(service.usuario()?.nombre).toBe('Ana Torres');
    });

    it('logout limpia el token y el usuario del almacenamiento', async () => {
        const promesa = service.login({ correo: 'ana@kanban.dev', password: 'Kanban#2026' });
        httpMock.expectOne('http://api.test/api/auth/login').flush({
            token: 'jwt-de-prueba',
            expiraUtc: new Date().toISOString(),
            usuario: { id: '1', nombre: 'Ana Torres', correo: 'ana@kanban.dev' }
        });
        await promesa;

        service.logout();

        expect(service.token).toBeNull();
        expect(service.estaAutenticado()).toBeFalse();
    });
});
