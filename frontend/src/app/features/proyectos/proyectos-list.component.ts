import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ProyectosService } from '../../core/api/proyectos.service';
import { EstadoProyecto, ProyectoDto } from '../../core/models/proyecto.models';

const ESTADOS: { label: string; value: EstadoProyecto }[] = [
    { label: 'Planificado', value: 'Planificado' },
    { label: 'En progreso', value: 'EnProgreso' },
    { label: 'Pausado', value: 'Pausado' },
    { label: 'Completado', value: 'Completado' },
    { label: 'Cancelado', value: 'Cancelado' }
];

@Component({
    selector: 'app-proyectos-list',
    standalone: true,
    imports: [
        CommonModule, ReactiveFormsModule, FormsModule,
        TableModule, ButtonModule, DialogModule, InputTextModule, InputTextareaModule,
        CalendarModule, DropdownModule, TagModule, ToastModule, ConfirmDialogModule, TooltipModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './proyectos-list.component.html'
})
export class ProyectosListComponent implements OnInit {
    readonly estados = ESTADOS;

    readonly proyectos = signal<ProyectoDto[]>([]);
    readonly totalRegistros = signal(0);
    readonly cargando = signal(false);
    readonly tamanoPagina = 10;

    readonly dialogVisible = signal(false);
    readonly editando = signal<ProyectoDto | null>(null);
    readonly guardando = signal(false);

    filtroNombre = '';
    private filtroTimeout?: ReturnType<typeof setTimeout>;
    private paginaActual = 1;

    readonly form = this.fb.group({
        nombre: ['', [Validators.required, Validators.maxLength(150)]],
        descripcion: [''],
        fechaInicio: [new Date(), Validators.required],
        fechaFinPrevista: [new Date(), Validators.required],
        estado: ['Planificado' as EstadoProyecto, Validators.required]
    });

    constructor(
        private readonly proyectosService: ProyectosService,
        private readonly router: Router,
        private readonly fb: FormBuilder,
        private readonly messageService: MessageService,
        private readonly confirmationService: ConfirmationService
    ) {}

    ngOnInit(): void {
        this.cargar();
    }

    cargarLazy(event: TableLazyLoadEvent): void {
        this.paginaActual = Math.floor((event.first ?? 0) / this.tamanoPagina) + 1;
        this.cargar();
    }

    onFiltroChange(): void {
        clearTimeout(this.filtroTimeout);
        this.filtroTimeout = setTimeout(() => {
            this.paginaActual = 1;
            this.cargar();
        }, 350);
    }

    private cargar(): void {
        this.cargando.set(true);
        this.proyectosService.listar(this.paginaActual, this.tamanoPagina, this.filtroNombre || undefined).subscribe({
            next: (resultado) => {
                this.proyectos.set(resultado.items);
                this.totalRegistros.set(resultado.totalRegistros);
                this.cargando.set(false);
            },
            error: () => {
                this.cargando.set(false);
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudieron cargar los proyectos.' });
            }
        });
    }

    abrirNuevo(): void {
        this.editando.set(null);
        this.form.reset({
            nombre: '', descripcion: '', fechaInicio: new Date(), fechaFinPrevista: new Date(), estado: 'Planificado'
        });
        this.dialogVisible.set(true);
    }

    abrirEdicion(proyecto: ProyectoDto): void {
        this.editando.set(proyecto);
        this.form.reset({
            nombre: proyecto.nombre,
            descripcion: proyecto.descripcion ?? '',
            fechaInicio: new Date(proyecto.fechaInicio),
            fechaFinPrevista: new Date(proyecto.fechaFinPrevista),
            estado: proyecto.estado
        });
        this.dialogVisible.set(true);
    }

    guardar(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const valores = this.form.getRawValue();
        const request = {
            nombre: valores.nombre!,
            descripcion: valores.descripcion || null,
            fechaInicio: this.aFechaIso(valores.fechaInicio!),
            fechaFinPrevista: this.aFechaIso(valores.fechaFinPrevista!),
            estado: valores.estado!
        };

        this.guardando.set(true);
        const editando = this.editando();
        const operacion = editando
            ? this.proyectosService.actualizar(editando.id, request)
            : this.proyectosService.crear(request);

        operacion.subscribe({
            next: () => {
                this.guardando.set(false);
                this.dialogVisible.set(false);
                this.messageService.add({ severity: 'success', summary: 'Listo', detail: editando ? 'Proyecto actualizado.' : 'Proyecto creado.' });
                this.cargar();
            },
            error: () => {
                this.guardando.set(false);
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar el proyecto.' });
            }
        });
    }

    confirmarEliminar(proyecto: ProyectoDto): void {
        this.confirmationService.confirm({
            message: `¿Eliminar el proyecto "${proyecto.nombre}"? Esta acción no se puede deshacer.`,
            header: 'Confirmar eliminación',
            icon: 'pi pi-exclamation-triangle',
            accept: () => this.eliminar(proyecto)
        });
    }

    private eliminar(proyecto: ProyectoDto): void {
        this.proyectosService.eliminar(proyecto.id).subscribe({
            next: () => {
                this.messageService.add({ severity: 'success', summary: 'Listo', detail: 'Proyecto eliminado.' });
                this.cargar();
            },
            error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar el proyecto.' })
        });
    }

    irAlTablero(proyecto: ProyectoDto): void {
        this.router.navigate(['/proyectos', proyecto.id, 'tablero']);
    }

    severidadEstado(estado: EstadoProyecto): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
        switch (estado) {
            case 'EnProgreso': return 'info';
            case 'Completado': return 'success';
            case 'Pausado': return 'warning';
            case 'Cancelado': return 'danger';
            default: return 'secondary';
        }
    }

    private aFechaIso(fecha: Date): string {
        const year = fecha.getFullYear();
        const month = String(fecha.getMonth() + 1).padStart(2, '0');
        const day = String(fecha.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }
}
