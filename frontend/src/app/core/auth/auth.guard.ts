import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/**
 * Guardia de ruta: impide el acceso al tablero (y a cualquier ruta protegida)
 * sin una sesión válida, redirigiendo al login.
 */
export const authGuard: CanActivateFn = (_route, state) => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.estaAutenticado()) {
        return true;
    }

    return router.createUrlTree(['/auth/login'], { queryParams: { redirectTo: state.url } });
};
