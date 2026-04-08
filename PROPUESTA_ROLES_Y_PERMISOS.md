# Propuesta de Roles, Pantallas y Permisos - PetsHome

## Analisis de Roles Actuales

Los 6 roles existentes son:

| # | Rol | Justificacion |
|---|-----|---------------|
| 1 | **Administrador** | Acceso total al sistema, incluyendo seguridad (usuarios, roles). Indispensable. |
| 2 | **Director** | Supervisa el refugio a nivel estrategico: reportes, empleados, adopciones, eventos. No gestiona seguridad. |
| 3 | **Supervisor** | Gestiona operaciones del dia a dia: inventario, recepciones, empleados, voluntarios, adopciones. |
| 4 | **Veterinario** | Enfocado en salud animal: citas medicas, recetas, tratamientos, historial medico, catalogos veterinarios. |
| 5 | **Cuidador** | Personal de contacto directo con animales: consulta mascotas, registra solicitudes, asiste en adopciones. |
| 6 | **Usuario Basico** | Acceso minimo: solo consulta informacion general (mascotas, eventos). Ideal para voluntarios o pasantes. |

### Veredicto: Los 6 roles son adecuados para este proyecto

Cada rol tiene un proposito claro y pantallas exclusivas que lo justifican. No hay roles redundantes ni pantallas huerfanas.

---

## Inventario Completo de Pantallas del Sistema

Extraido del sidebar y controllers del proyecto:

| # | Pantalla (pan_Descripcion) | Grupo Sidebar | Controller |
|---|---------------------------|---------------|------------|
| 1 | Listado de empleados | Cuenta | EmpleadoController |
| 2 | Listado de voluntarios | Cuenta | VoluntarioController |
| 3 | Listado de recepciones | Inventario | RecepcionMercanciaController |
| 4 | Listado de items | Inventario | ItemController |
| 5 | Listado de cargos | Administracion | EmpleadosCargoController |
| 6 | Listado de refugios | Administracion | RefugioController |
| 7 | Listado de localidades | Administracion | LocalidadController |
| 8 | Listado de categorias | Administracion | CategoriaController |
| 9 | Listado de eventos | Administracion | EventoController |
| 10 | Listado de mascotas | Adopcion | MascotaController + HistorialMedicoController |
| 11 | Listado de adopciones | Adopcion | AdopcionController |
| 12 | Listado de solicitudes | Adopcion | SolicitudController |
| 13 | Listado de citas medicas | Medicamento | CitaMedicaController |
| 14 | Listado de recetas | Medicamento | RecetaController |
| 15 | Listado de tratamientos | Medicamento | TratamientoController |
| 16 | Listado de vacunas | Medicamento | VacunaController |
| 17 | Listado de procedencias | Medicamento | ProcedenciaController |
| 18 | Listado de razas | Medicamento | RazaController |
| 19 | Listado de tipos de consulta | Medicamento | TipoConsultaController |
| 20 | Listado de gravedades | Medicamento | GravedadController |
| 21 | Listado de tipos de medicamento | Medicamento | TipoMedicamentoController |
| 22 | Listado de vias de administracion | Medicamento | ViaAdministracionController |
| 23 | Listado de tipos de parasito | Medicamento | TipoParasitoController |
| 24 | Listado de tipos de esterilizacion | Medicamento | TipoEsterilizacionController |
| 25 | Listado de usuarios | Seguridad | UsuariosController |
| 26 | Listado de roles | Seguridad | RolesController |

**Total: 26 pantallas**

---

## Matriz de Permisos CRUD por Rol

Leyenda: **C** = Consultar, **I** = Insertar, **E** = Editar, **D** = Eliminar

### Grupo: Cuenta

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de empleados | CIED | CIE | CIE | - | - | - |
| Listado de voluntarios | CIED | CIE | CIE | - | C | - |

### Grupo: Inventario

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de recepciones | CIED | C | CIED | - | - | - |
| Listado de items | CIED | C | CIED | C | - | - |

### Grupo: Administracion

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de cargos | CIED | C | CIE | - | - | - |
| Listado de refugios | CIED | CIE | C | - | - | - |
| Listado de localidades | CIED | C | C | - | - | - |
| Listado de categorias | CIED | C | CIE | - | - | - |
| Listado de eventos | CIED | CIE | CIE | - | C | C |

### Grupo: Adopcion

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de mascotas | CIED | CIE | CIE | C | C | C |
| Listado de adopciones | CIED | CIE | CIE | - | C | - |
| Listado de solicitudes | CIED | CIE | CIE | - | CI | C |

### Grupo: Medicamento (Veterinaria)

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de citas medicas | CIED | C | - | CIED | - | - |
| Listado de recetas | CIED | C | - | CIED | - | - |
| Listado de tratamientos | CIED | C | - | CIED | - | - |
| Listado de vacunas | CIED | - | - | CIED | - | - |
| Listado de procedencias | CIED | - | - | CIE | - | - |
| Listado de razas | CIED | - | - | CIE | - | - |
| Listado de tipos de consulta | CIED | - | - | CIE | - | - |
| Listado de gravedades | CIED | - | - | CIE | - | - |
| Listado de tipos de medicamento | CIED | - | - | CIE | - | - |
| Listado de vias de administracion | CIED | - | - | CIE | - | - |
| Listado de tipos de parasito | CIED | - | - | CIE | - | - |
| Listado de tipos de esterilizacion | CIED | - | - | CIE | - | - |

### Grupo: Seguridad

| Pantalla | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|----------|:---:|:---:|:---:|:---:|:---:|:---:|
| Listado de usuarios | CIED | - | - | - | - | - |
| Listado de roles | CIED | - | - | - | - | - |

---

## Resumen: Cantidad de Pantallas por Rol

| Rol | Pantallas con acceso | Pantallas exclusivas |
|-----|:---:|---|
| **Administrador** | 26 de 26 | Seguridad (usuarios, roles) con CRUD completo |
| **Director** | 16 de 26 | Ninguna exclusiva, pero ve reportes/consulta de areas medicas |
| **Supervisor** | 14 de 26 | Inventario (recepciones, items) con CRUD completo |
| **Veterinario** | 14 de 26 | Catalogos veterinarios (9 pantallas exclusivas) |
| **Cuidador** | 6 de 26 | Ninguna exclusiva, pero es el unico rol bajo que puede crear solicitudes |
| **Usuario Basico** | 3 de 26 | Acceso minimo, solo consulta |

---

## Visibilidad del Sidebar por Rol

| Grupo Sidebar | Administrador | Director | Supervisor | Veterinario | Cuidador | Usuario Basico |
|---------------|:---:|:---:|:---:|:---:|:---:|:---:|
| Cuenta | SI | SI | SI | - | SI (solo voluntarios) | - |
| Inventario | SI | SI | SI | SI (solo items) | - | - |
| Administracion | SI | SI | SI | - | SI (solo eventos) | SI (solo eventos) |
| Adopcion | SI | SI | SI | SI (solo mascotas) | SI | SI (solo mascotas y solicitudes) |
| Medicamento | SI | SI (parcial) | - | SI | - | - |
| Seguridad | SI | - | - | - | - | - |

---

## Justificacion de Cada Rol

### 1. Administrador
- **Proposito:** Control total del sistema.
- **Diferenciador:** Unico rol con acceso a Seguridad (usuarios y roles). Puede eliminar registros en todas las pantallas.

### 2. Director
- **Proposito:** Vision ejecutiva del refugio.
- **Diferenciador:** Acceso de consulta a areas medicas y financieras sin poder modificar. Gestiona refugios, empleados y adopciones.

### 3. Supervisor
- **Proposito:** Operaciones del dia a dia.
- **Diferenciador:** Control total de inventario (recepciones + items). Gestiona empleados, voluntarios, adopciones y catalogos administrativos. No ve area medica.

### 4. Veterinario
- **Proposito:** Salud animal.
- **Diferenciador:** Unico rol (aparte de Admin) con CRUD en citas, recetas, tratamientos y todos los catalogos medicos. Puede consultar mascotas e items pero no modificarlos.

### 5. Cuidador
- **Proposito:** Atencion directa a los animales.
- **Diferenciador:** Puede ver mascotas, adopciones, voluntarios y eventos. Puede crear solicitudes de adopcion. No accede a areas administrativas ni medicas.

### 6. Usuario Basico
- **Proposito:** Acceso minimo para voluntarios, pasantes o personal nuevo.
- **Diferenciador:** Solo consulta mascotas, eventos y solicitudes. No puede crear ni editar nada excepto el Home.

---

## Datos para Insertar en Base de Datos

Si las pantallas aun no existen en la tabla `tbPantallas`, aqui estan los INSERT sugeridos:

```sql
-- Grupo: Cuenta
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de empleados', 'Cuenta', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de voluntarios', 'Cuenta', 1);

-- Grupo: Inventario
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de recepciones', 'Inventario', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de items', 'Inventario', 1);

-- Grupo: Administracion
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de cargos', 'Administracion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de refugios', 'Administracion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de localidades', 'Administracion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de categorias', 'Administracion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de eventos', 'Administracion', 1);

-- Grupo: Adopcion
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de mascotas', 'Adopcion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de adopciones', 'Adopcion', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de solicitudes', 'Adopcion', 1);

-- Grupo: Medicamento
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de citas medicas', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de recetas', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de tratamientos', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de vacunas', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de procedencias', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de razas', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de tipos de consulta', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de gravedades', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de tipos de medicamento', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de vias de administracion', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de tipos de parasito', 'Medicamento', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de tipos de esterilizacion', 'Medicamento', 1);

-- Grupo: Seguridad
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de usuarios', 'Seguridad', 1);
INSERT INTO tbPantallas (pan_Descripcion, pan_Grupo, pan_EsActivo) VALUES ('Listado de roles', 'Seguridad', 1);
```

> **Nota:** Estos INSERT son de referencia. Verifica si ya existen en tu base de datos antes de ejecutarlos.
