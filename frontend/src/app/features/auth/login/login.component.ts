import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule, ButtonModule, InputTextModule, PasswordModule],
    templateUrl: './login.component.html'
})
export class LoginComponent {
    readonly form = this.fb.group({
        correo: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required]
    });

    readonly cargando = signal(false);
    readonly error = signal<string | null>(null);

    constructor(
        private readonly fb: FormBuilder,
        private readonly auth: AuthService,
        private readonly router: Router
    ) {}

    async enviar(): Promise<void> {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.cargando.set(true);
        this.error.set(null);

        try {
            const { correo, password } = this.form.getRawValue();
            await this.auth.login({ correo: correo!, password: password! });
            await this.router.navigate(['/proyectos']);
        } catch {
            this.error.set('Correo o contraseña incorrectos.');
        } finally {
            this.cargando.set(false);
        }
    }
}
