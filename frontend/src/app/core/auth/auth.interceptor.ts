import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AppConfigService } from '../config/app-config.service';
import { AuthService } from './auth.service';

/**
 * Adjunta el token JWT a cada petición dirigida a la API y gestiona la respuesta 401
 * de forma coherente: cierra la sesión local y redirige al login.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const auth = inject(AuthService);
    const config = inject(AppConfigService);
    const router = inject(Router);

    const apiUrl = config.apiUrlOrNull;
    const esPeticionApi = apiUrl !== null && req.url.startsWith(apiUrl);
    const token = auth.token;

    const request = esPeticionApi && token
        ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
        : req;

    return next(request).pipe(
        catchError((error: HttpErrorResponse) => {
            if (esPeticionApi && error.status === 401) {
                auth.logout();
                router.navigate(['/auth/login']);
            }
            return throwError(() => error);
        })
    );
};
