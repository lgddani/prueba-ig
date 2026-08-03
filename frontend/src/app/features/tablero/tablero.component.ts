import { CommonModule } from '@angular/common';
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, OnDestroy, OnInit, computed, signal } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { TooltipModule } from 'primeng/tooltip';
import { ColumnasService } from '../../core/api/columnas.service';
import { ProyectosService } from '../../core/api/proyectos.service';
import { ReportesService } from '../../core/api/reportes.service';
import { TareasService } from '../../core/api/tareas.service';
import { UsuariosService } from '../../core/api/usuarios.service';
import { UsuarioDto } from '../../core/models/auth.models';
import { ColumnaDto } from '../../core/models/columna.models';
import { ProyectoDto } from '../../core/models/proyecto.models';
import { Prioridad, TareaDto } from '../../core/models/tarea.models';
import { BoardHubService } from '../../core/realtime/board-hub.service';

const PRIORIDADES: { label: string; value: Prioridad }[] = [
    { label: 'Baja', value: 'Baja' },
    { label: 'Media', value: 'Media' },
    { label: 'Alta', value: 'Alta' },
    { label: 'Urgente', value: 'Urgente' }
];

@Component({
    selector: 'app-tablero',
    standalone: true,
    imports: [
        CommonModule, ReactiveFormsModule, FormsModule, RouterModule, DragDropModule,
        ButtonModule, DialogModule, InputTextModule, InputTextareaModule, DropdownModule,
        TagModule, AvatarModule, ToastModule, ConfirmDialogModule, ProgressSpinnerModule, TooltipModule
    ],
    providers: [MessageService, ConfirmationService],
    templateUrl: './tablero.component.html'
})
export class TableroComponent implements OnInit, OnDestroy {
    readonly prioridades = PRIORIDADES;

    readonly proyecto = signal<ProyectoDto | null>(null);
    readonly columnas = signal<ColumnaDto[]>([]);
    readonly tareas = signal<TareaDto[]>([]);
    readonly usuarios = signal<UsuarioDto[]>([]);
    readonly usuariosConectados = signal<string[]>([]);
    readonly cargando = signal(true);

    readonly filtroResponsableId = signal<string | null>(null);
    readonly filtroPrioridad = signal<Prioridad | null>(null);
    readonly filtroTexto = signal('');

    readonly tareasFiltradas = computed(() => {
        const responsable = this.filtroResponsableId();
        const prioridad = this.filtroPrioridad();
        const texto = this.filtroTexto().trim().toLowerCase();

        return this.tareas().filter((t) => {
            if (responsable && t.responsableId !== responsable) return false;
            if (prioridad && t.prioridad !== prioridad) return false;
            if (texto && !t.titulo.toLowerCase().includes(texto)) return false;
            return true;
        });
    });

    private proyectoId!: string;

    // Diálogo de columna
    readonly columnaDialogVisible = signal(false);
    readonly columnaEditando = signal<ColumnaDto | null>(null);
    readonly columnaForm = this.fb.group({ nombre: ['', [Validators.required, Validators.maxLength(100)]] });

    // Diálogo de tarea
    readonly tareaDialogVisible = signal(false);
    readonly tareaEditando = signal<TareaDto | null>(null);
    readonly columnaDestinoNuevaTarea = signal<string | null>(null);
    readonly tareaForm = this.fb.group({
        titulo: ['', [Validators.required, Validators.maxLength(200)]],
        descripcion: [''],
        prioridad: ['Media' as Prioridad, Validators.required],
        responsableId: [null as string | null]
    });

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly fb: FormBuilder,
        private readonly proyectosService: ProyectosService,
        private readonly columnasService: ColumnasService,
        private readonly tareasService: TareasService,
        private readonly usuariosService: UsuariosService,
        private readonly reportesService: ReportesService,
        private readonly boardHub: BoardHubService,
        private readonly messageService: MessageService,
        private readonly confirmationService: ConfirmationService
    ) {}

    ngOnInit(): void {
        this.proyectoId = this.route.snapshot.paramMap.get('id')!;
        this.cargarTodo();
        this.suscribirTiempoReal();
    }

    ngOnDestroy(): void {
        this.boardHub.desconectar();
    }

    tareasDeColumna(columnaId: string): TareaDto[] {
        return this.tareasFiltradas()
            .filter((t) => t.columnaId === columnaId)
            .sort((a, b) => a.orden - b.orden);
    }

    idsColumnasDropList(): string[] {
        return this.columnas().map((c) => `col-${c.id}`);
    }

    private cargarTodo(): void {
        this.cargando.set(true);
        this.proyectosService.obtenerPorId(this.proyectoId).subscribe((p) => this.proyecto.set(p));
        this.usuariosService.listar().subscribe((u) => this.usuarios.set(u));

        this.columnasService.listar(this.proyectoId).subscribe({
            next: (columnas) => {
                this.columnas.set(columnas.sort((a, b) => a.orden - b.orden));
                this.tareasService.listar(this.proyectoId).subscribe({
                    next: (tareas) => {
                        this.tareas.set(tareas);
                        this.cargando.set(false);
                    },
                    error: () => this.cargando.set(false)
                });
            },
            error: () => this.cargando.set(false)
        });
    }

    private suscribirTiempoReal(): void {
        this.boardHub.conectarYSuscribir(this.proyectoId);

        this.boardHub.tareaCreada$.subscribe((tarea) => this.upsertTarea(tarea));
        this.boardHub.tareaActualizada$.subscribe((tarea) => this.upsertTarea(tarea));
        this.boardHub.tareaMovida$.subscribe(({ tarea }) => this.upsertTarea(tarea));
        this.boardHub.tareaEliminada$.subscribe(({ tareaId }) => this.eliminarTareaLocal(tareaId));
        this.boardHub.usuariosConectados$.subscribe((nombres) => this.usuariosConectados.set(nombres));
    }

    private upsertTarea(tarea: TareaDto): void {
        const actuales = this.tareas();
        const index = actuales.findIndex((t) => t.id === tarea.id);
        if (index === -1) {
            this.tareas.set([...actuales, tarea]);
        } else {
            const copia = [...actuales];
            copia[index] = tarea;
            this.tareas.set(copia);
        }
    }

    private eliminarTareaLocal(tareaId: string): void {
        this.tareas.set(this.tareas().filter((t) => t.id !== tareaId));
    }

    // ----- Drag & drop de tareas -----

    onDropTarea(event: CdkDragDrop<TareaDto[]>, columnaDestino: ColumnaDto): void {
        const tarea = event.item.data as TareaDto;
        const snapshot = this.tareas();

        const listaDestinoAntes = this.tareasDeColumna(columnaDestino.id);
        const nuevaLista = [...listaDestinoAntes];

        if (event.previousContainer === event.container) {
            moveItemInArray(nuevaLista, event.previousIndex, event.currentIndex);
        } else {
            nuevaLista.splice(event.currentIndex, 0, tarea);
        }

        // Actualización optimista: reordena localmente asignando órdenes provisionales.
        const actualizadas = snapshot.map((t) => {
            const idx = nuevaLista.findIndex((n) => n.id === t.id);
            if (idx === -1) return t;
            return { ...t, columnaId: columnaDestino.id, orden: idx };
        });
        this.tareas.set(actualizadas);

        this.tareasService.mover(this.proyectoId, tarea.id, {
            columnaDestinoId: columnaDestino.id,
            posicionDestino: event.currentIndex
        }).subscribe({
            next: (tareaActualizada) => this.upsertTarea(tareaActualizada),
            error: () => {
                // Reversión visible del movimiento si el servidor responde con error.
                this.tareas.set(snapshot);
                this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo mover la tarea. Se revirtió el cambio.' });
            }
        });
    }

    // ----- Columnas -----

    abrirNuevaColumna(): void {
        this.columnaEditando.set(null);
        this.columnaForm.reset({ nombre: '' });
        this.columnaDialogVisible.set(true);
    }

    abrirEdicionColumna(columna: ColumnaDto): void {
        this.columnaEditando.set(columna);
        this.columnaForm.reset({ nombre: columna.nombre });
        this.columnaDialogVisible.set(true);
    }

    guardarColumna(): void {
        if (this.columnaForm.invalid) {
            this.columnaForm.markAllAsTouched();
            return;
        }

        const nombre = this.columnaForm.getRawValue().nombre!;
        const editando = this.columnaEditando();
        const operacion = editando
            ? this.columnasService.actualizar(this.proyectoId, editando.id, { nombre })
            : this.columnasService.crear(this.proyectoId, { nombre });

        operacion.subscribe({
            next: (columna) => {
                const actuales = this.columnas();
                const index = actuales.findIndex((c) => c.id === columna.id);
                this.columnas.set(index === -1 ? [...actuales, columna] : actuales.map((c) => (c.id === columna.id ? columna : c)));
                this.columnaDialogVisible.set(false);
            },
            error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar la columna.' })
        });
    }

    confirmarEliminarColumna(columna: ColumnaDto): void {
        if (this.tareasDeColumna(columna.id).length > 0) {
            this.messageService.add({ severity: 'warn', summary: 'No permitido', detail: 'No se puede eliminar una columna que contiene tareas.' });
            return;
        }

        this.confirmationService.confirm({
            message: `¿Eliminar la columna "${columna.nombre}"?`,
            header: 'Confirmar eliminación',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.columnasService.eliminar(this.proyectoId, columna.id).subscribe({
                    next: () => this.columnas.set(this.columnas().filter((c) => c.id !== columna.id)),
                    error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar la columna.' })
                });
            }
        });
    }

    moverColumna(columna: ColumnaDto, direccion: -1 | 1): void {
        const actuales = [...this.columnas()];
        const index = actuales.findIndex((c) => c.id === columna.id);
        const nuevoIndex = index + direccion;
        if (nuevoIndex < 0 || nuevoIndex >= actuales.length) return;

        moveItemInArray(actuales, index, nuevoIndex);
        this.columnas.set(actuales);

        this.columnasService.reordenar(this.proyectoId, { columnaIdsEnOrden: actuales.map((c) => c.id) }).subscribe({
            error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo reordenar las columnas.' })
        });
    }

    // ----- Tareas -----

    abrirNuevaTarea(columna: ColumnaDto): void {
        this.tareaEditando.set(null);
        this.columnaDestinoNuevaTarea.set(columna.id);
        this.tareaForm.reset({ titulo: '', descripcion: '', prioridad: 'Media', responsableId: null });
        this.tareaDialogVisible.set(true);
    }

    abrirEdicionTarea(tarea: TareaDto): void {
        this.tareaEditando.set(tarea);
        this.tareaForm.reset({
            titulo: tarea.titulo,
            descripcion: tarea.descripcion ?? '',
            prioridad: tarea.prioridad,
            responsableId: tarea.responsableId ?? null
        });
        this.tareaDialogVisible.set(true);
    }

    guardarTarea(): void {
        if (this.tareaForm.invalid) {
            this.tareaForm.markAllAsTouched();
            return;
        }

        const valores = this.tareaForm.getRawValue();
        const editando = this.tareaEditando();

        const operacion = editando
            ? this.tareasService.actualizar(this.proyectoId, editando.id, {
                  titulo: valores.titulo!,
                  descripcion: valores.descripcion || null,
                  prioridad: valores.prioridad!,
                  responsableId: valores.responsableId
              })
            : this.tareasService.crear(this.proyectoId, {
                  titulo: valores.titulo!,
                  descripcion: valores.descripcion || null,
                  prioridad: valores.prioridad!,
                  responsableId: valores.responsableId,
                  columnaId: this.columnaDestinoNuevaTarea()!
              });

        operacion.subscribe({
            next: (tarea) => {
                this.upsertTarea(tarea);
                this.tareaDialogVisible.set(false);
            },
            error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo guardar la tarea.' })
        });
    }

    confirmarEliminarTarea(tarea: TareaDto): void {
        this.confirmationService.confirm({
            message: `¿Eliminar la tarea "${tarea.titulo}"?`,
            header: 'Confirmar eliminación',
            icon: 'pi pi-exclamation-triangle',
            accept: () => {
                this.tareasService.eliminar(this.proyectoId, tarea.id).subscribe({
                    next: () => this.eliminarTareaLocal(tarea.id),
                    error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo eliminar la tarea.' })
                });
            }
        });
    }

    severidadPrioridad(prioridad: Prioridad): 'success' | 'info' | 'warning' | 'danger' {
        switch (prioridad) {
            case 'Baja': return 'success';
            case 'Media': return 'info';
            case 'Alta': return 'warning';
            case 'Urgente': return 'danger';
        }
    }

    // ----- Reportes -----

    descargarPdf(): void {
        this.descargar(this.reportesService.descargarPdf(this.proyectoId));
    }

    descargarExcel(): void {
        this.descargar(this.reportesService.descargarExcel(this.proyectoId));
    }

    private descargar(observable: ReturnType<ReportesService['descargarPdf']>): void {
        observable.subscribe({
            next: (response) => {
                const nombreArchivo = this.extraerNombreArchivo(response.headers.get('content-disposition')) ?? 'reporte';
                const blob = response.body!;
                const url = window.URL.createObjectURL(blob);
                const enlace = document.createElement('a');
                enlace.href = url;
                enlace.download = nombreArchivo;
                enlace.click();
                window.URL.revokeObjectURL(url);
            },
            error: () => this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo generar el reporte.' })
        });
    }

    private extraerNombreArchivo(contentDisposition: string | null): string | null {
        if (!contentDisposition) return null;
        const match = /filename="?([^";]+)"?/i.exec(contentDisposition);
        return match ? match[1] : null;
    }

    volver(): void {
        this.router.navigate(['/proyectos']);
    }
}
