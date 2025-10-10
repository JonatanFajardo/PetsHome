# Guía de Migración: De Inventarios a Recepciones de Mercancía

## Resumen de Cambios

Este documento detalla todos los cambios necesarios para migrar del esquema de inventario anterior al nuevo esquema de recepciones y salidas.

## 1. Cambios en Base de Datos

### Tablas Renombradas
- `tbInventarios` → `tbRecepcionesMercancia`
- `tbInventariosDetalles` → `tbRecepcionesDetalles`

### Campos Renombrados

#### En tbRecepcionesMercancia (anteriormente tbInventarios):
- `inv_Id` → `recep_Id`
- `inv_Descripcion` → `recep_Descripcion`
- `inv_Fecha` → `recep_Fecha`
- `inv_EsEliminado` → `recep_EsEliminado`
- `inv_UsuarioCrea` → `recep_UsuarioCrea`
- `inv_FechaCrea` → `recep_FechaCrea`
- `inv_UsuarioModifica` → `recep_UsuarioModifica`
- `inv_FechaModifica` → `recep_FechaModifica`

#### En tbRecepcionesDetalles (anteriormente tbInventariosDetalles):
- `invdet_Id` → `recdet_Id`
- `inv_Id` → `recep_Id`
- `invdet_Existencia` → `recdet_Cantidad`
- `invdet_EsEliminado` → `recdet_EsEliminado`
- `invdet_UsuarioCrea` → `recdet_UsuarioCrea`
- `invdet_FechaCrea` → `recdet_FechaCrea`
- `invdet_UsuarioModifica` → `recdet_UsuarioModifica`
- `invdet_FechaModifica` → `recdet_FechaModifica`

### Campos Nuevos

#### En tbRecepcionesMercancia:
- `recep_TipoRecepcion` (string) - Tipo de recepción
- `recep_OrigenId` (int?) - ID del origen
- `recep_NumeroDocumento` (string) - Número de documento

#### En tbRecepcionesDetalles:
- `recdet_PrecioUnitario` (decimal) - Precio unitario
- `recdet_FechaVencimiento` (DateTime?) - Fecha de vencimiento
- `recdet_NumeroLote` (string) - Número de lote

### Campos Eliminados
- `invdet_Stock` (eliminado de tbRecepcionesDetalles)

### Nuevas Tablas
- `tbExistencias` - Control de stock actual
- `tbSalidas` - Registro de salidas
- `tbSalidasDetalles` - Detalles de salidas
- `tbMovimientos` - Historial de movimientos

## 2. Archivos Creados

### Entidades (PetsHome.Common/Entities/Inventario/)
- ✅ `tbRecepcionesMercancia.cs`
- ✅ `tbRecepcionesDetalles.cs`
- ✅ `tbExistencias.cs`
- ✅ `tbSalidas.cs`
- ✅ `tbSalidasDetalles.cs`
- ✅ `tbMovimientos.cs`

### Interfaces de Repositorio (PetsHome.Logic/Interfaces/Especific/)
- ✅ `IRecepcionesMercanciaRepository.cs`
- ✅ `IRecepcionesDetallesRepository.cs`
- ✅ `IExistenciasRepository.cs`
- ✅ `ISalidasRepository.cs`
- ✅ `ISalidasDetallesRepository.cs`
- ✅ `IMovimientosRepository.cs`

### Repositorios (PetsHome.Logic/Repositories/)
- ✅ `RecepcionesMercanciaRepository.cs`
- ✅ `ExistenciasRepository.cs`
- ✅ `SalidasRepository.cs`

### ViewModels (PetsHome.Business/Models/)
- ✅ `RecepcionMercanciaViewModel.cs` (ya existía)
- ✅ `SalidaViewModel.cs` (ya existía)

### Servicios (PetsHome.Business/Services/)
- ✅ `RecepcionMercanciaService.cs`
- ✅ `SalidaService.cs`

### Controladores (PetsHome.UI/Controllers/parciales/)
- ✅ `RecepcionMercanciaController.cs`
- ✅ `SalidaController.cs`

## 3. Archivos que Requieren Modificación

### 3.1 Archivos de Configuración

#### ServiceConfiguration.cs
```csharp
// Agregar nuevos servicios y repositorios
services.AddScoped<RecepcionesMercanciaRepository>();
services.AddScoped<ExistenciasRepository>();
services.AddScoped<SalidasRepository>();
services.AddScoped<RecepcionMercanciaService>();
services.AddScoped<SalidaService>();
```

#### MappingProfileExtensions.cs
```csharp
// Agregar mapeos para las nuevas entidades
CreateMap<tbRecepcionesMercancia, RecepcionMercanciaViewModel>().ReverseMap();
CreateMap<tbSalidas, SalidaViewModel>().ReverseMap();
CreateMap<tbExistencias, ExistenciaViewModel>().ReverseMap();
```

### 3.2 Archivos que Referencian las Tablas Antiguas

Los siguientes archivos contienen referencias a las tablas antiguas y necesitan actualización:

#### PetsHome.Logic/Repositories/
- `InventarioRepository.cs` - **DEPRECAR o MIGRAR**
- `InventariosDetalleRepository.cs` - **DEPRECAR o MIGRAR**

#### PetsHome.Business/Services/
- `InventarioService.cs` - **DEPRECAR o MIGRAR**
- `InventariosDetalleService.cs` - **DEPRECAR o MIGRAR**

#### PetsHome.UI/Controllers/parciales/
- `InventarioController.cs` - **DEPRECAR o MIGRAR**
- `InventarioDetalleController.cs` - **DEPRECAR o MIGRAR**

#### PetsHome.Common/Entities/Inventario/
- `tbInventarios.cs` - **MANTENER para compatibilidad**
- `tbInventariosDetalles.cs` - **MANTENER para compatibilidad**

### 3.3 Procedimientos Almacenados Afectados

Estos procedimientos necesitan ser actualizados en la base de datos:

#### Existentes (necesitan migración):
- `[Inventario].[PR_Inventario_Inventarios_List]` → `[Inventario].[PR_Inventario_RecepcionesMercancia_List]`
- `[Inventario].[PR_Inventario_Inventarios_Find]` → `[Inventario].[PR_Inventario_RecepcionesMercancia_Find]`
- `[Inventario].[PR_Inventario_Inventarios_Detail]` → `[Inventario].[PR_Inventario_RecepcionesMercancia_Detail]`
- `[Inventario].[PR_Inventario_Inventarios_Insert]` → `[Inventario].[PR_Inventario_RecepcionesMercancia_Insert]`
- `[Inventario].[PR_Inventario_Inventarios_Update]` → `[Inventario].[PR_Inventario_RecepcionesMercancia_Update]`
- `[General].[PR_General_Inventarios_Delete]` → `[General].[PR_General_RecepcionesMercancia_Delete]`

#### Nuevos (necesitan creación):
- `[Inventario].[PR_Inventario_Salidas_List]`
- `[Inventario].[PR_Inventario_Salidas_Find]`
- `[Inventario].[PR_Inventario_Salidas_Detail]`
- `[Inventario].[PR_Inventario_Salidas_Insert]`
- `[Inventario].[PR_Inventario_Salidas_Update]`
- `[General].[PR_General_Salidas_Delete]`
- `[Inventario].[PR_Inventario_Existencias_List]`
- `[Inventario].[PR_Inventario_Existencias_Find]`
- `[Inventario].[PR_Inventario_Existencias_Detail]`
- `[Inventario].[PR_Inventario_Existencias_Insert]`
- `[Inventario].[PR_Inventario_Existencias_Update]`
- `[Inventario].[PR_Inventario_Existencias_GetByItemRefugio]`
- `[Inventario].[PR_Inventario_Existencias_UpdateStock]`
- `[General].[PR_General_Existencias_Delete]`

## 4. Estrategia de Migración Recomendada

### Fase 1: Preparación
1. ✅ Crear todas las nuevas entidades, repositorios y servicios
2. ✅ Crear controladores para las nuevas funcionalidades
3. Actualizar la configuración de dependencias
4. Crear/actualizar procedimientos almacenados

### Fase 2: Migración de Datos
1. Ejecutar scripts de migración de datos de `tbInventarios` a `tbRecepcionesMercancia`
2. Migrar datos de `tbInventariosDetalles` a `tbRecepcionesDetalles`
3. Crear registros iniciales en `tbExistencias` basados en inventarios actuales

### Fase 3: Transición
1. Mantener ambos sistemas funcionando en paralelo
2. Dirigir nuevas operaciones al sistema de recepciones/salidas
3. Migrar operaciones existentes gradualmente

### Fase 4: Finalización
1. Deprecar controladores y servicios antiguos
2. Mantener entidades antiguas para compatibilidad histórica
3. Actualizar interfaces de usuario

## 5. Consideraciones Importantes

### Compatibilidad hacia atrás
- Mantener las entidades `tbInventarios` y `tbInventariosDetalles` para consultas históricas
- Crear vistas que mapeen los datos antiguos al nuevo formato si es necesario

### Migración de datos
- Los datos existentes en `tbInventarios` deben migrar a `tbRecepcionesMercancia`
- `tbInventariosDetalles.invdet_Existencia` → `tbRecepcionesDetalles.recdet_Cantidad`
- `tbInventariosDetalles.invdet_Stock` se debe usar para crear registros en `tbExistencias`

### Nuevas funcionalidades
- Control de stock en tiempo real a través de `tbExistencias`
- Trazabilidad completa con `tbMovimientos`
- Gestión de salidas con `tbSalidas` y `tbSalidasDetalles`

## 6. Scripts de Base de Datos Requeridos

### Creación de tablas
```sql
-- Scripts para crear las nuevas tablas con sus índices y constraints
-- (Los scripts específicos deberían ejecutarse en el orden correcto)
```

### Migración de datos
```sql
-- Scripts para migrar datos existentes
-- Scripts para crear registros iniciales en tbExistencias
```

### Creación de procedimientos almacenados
```sql
-- Los 11 nuevos procedimientos almacenados mencionados
```

### Creación de vistas
```sql
-- Las 4 nuevas vistas para facilitar consultas
```

### Triggers
```sql
-- Los 2 triggers para mantenimiento automático de existencias
```

## 7. Testing

### Puntos críticos a probar
1. Migración de datos existentes
2. Funcionalidad de recepciones con actualización de existencias
3. Funcionalidad de salidas con validación de stock
4. Integridad referencial entre las nuevas tablas
5. Performance de las consultas con las nuevas estructuras

---

**Estado del Proyecto:** ✅ Código C# completado - Pendiente implementación en base de datos y pruebas