import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppConfigService } from '../config/app-config.service';
import { AuthService } from './auth.service';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
    let http: HttpClient;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(withInterceptors([authInterceptor])),
                provideHttpClientTesting(),
                { provide: AppConfigService, useValue: { apiUrlOrNull: 'http://api.test/api' } },
                { provide: AuthService, useValue: { token: 'jwt-de-prueba', logout: () => {} } }
            ]
        });

        http = TestBed.inject(HttpClient);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpMock.verify());

    it('adjunta el header Authorization en peticiones a la API', () => {
        http.get('http://api.test/api/proyectos').subscribe();

        const req = httpMock.expectOne('http://api.test/api/proyectos');
        expect(req.request.headers.get('Authorization')).toBe('Bearer jwt-de-prueba');
        req.flush({});
    });

    it('no adjunta el header en peticiones fuera de la API (assets)', () => {
        http.get('assets/config.json').subscribe();

        const req = httpMock.expectOne('assets/config.json');
        expect(req.request.headers.has('Authorization')).toBeFalse();
        req.flush({});
    });
});
