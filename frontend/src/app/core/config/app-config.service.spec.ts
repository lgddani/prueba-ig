import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AppConfigService } from './app-config.service';

describe('AppConfigService', () => {
    let service: AppConfigService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({ imports: [HttpClientTestingModule], providers: [AppConfigService] });
        service = TestBed.inject(AppConfigService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpMock.verify());

    it('apiUrl lanza un error si se consulta antes de llamar a load()', () => {
        expect(() => service.apiUrl).toThrowError();
        expect(service.apiUrlOrNull).toBeNull();
    });

    it('load() obtiene assets/config.json y expone apiUrl/signalrUrl', async () => {
        const promesa = service.load();

        const req = httpMock.expectOne('assets/config.json');
        req.flush({ apiUrl: 'http://api.test/api', signalrUrl: 'http://api.test/hubs/board' });

        await promesa;

        expect(service.apiUrl).toBe('http://api.test/api');
        expect(service.signalrUrl).toBe('http://api.test/hubs/board');
        expect(service.apiUrlOrNull).toBe('http://api.test/api');
    });
});
