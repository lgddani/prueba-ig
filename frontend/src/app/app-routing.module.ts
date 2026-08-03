import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { NotfoundComponent } from './shared/notfound/notfound.component';
import { AppLayoutComponent } from './layout/app.layout.component';
import { authGuard } from './core/auth/auth.guard';

@NgModule({
    imports: [
        RouterModule.forRoot(
            [
                {
                    path: '',
                    component: AppLayoutComponent,
                    canActivate: [authGuard],
                    children: [
                        { path: '', redirectTo: 'proyectos', pathMatch: 'full' },
                        {
                            path: 'proyectos',
                            loadComponent: () =>
                                import('./features/proyectos/proyectos-list.component').then((m) => m.ProyectosListComponent)
                        },
                        {
                            path: 'proyectos/:id/tablero',
                            loadComponent: () => import('./features/tablero/tablero.component').then((m) => m.TableroComponent)
                        }
                    ]
                },
                {
                    path: 'auth/login',
                    loadComponent: () => import('./features/auth/login/login.component').then((m) => m.LoginComponent)
                },
                { path: 'notfound', component: NotfoundComponent },
                { path: '**', redirectTo: '/notfound' }
            ],
            { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', onSameUrlNavigation: 'reload' }
        )
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {}
