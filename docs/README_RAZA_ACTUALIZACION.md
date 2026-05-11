# Actualización de la Tabla Razas - Instrucciones

## Resumen
Este documento describe los pasos necesarios para agregar los nuevos campos a la tabla `Refugio.tbRazas` en la base de datos PETSHOMEDB.

## Nuevos Campos Agregados
1. **raza_Tamano** (VARCHAR(20)) - Tamaño de la raza: pequeño, mediano, grande, gigante
2. **raza_TipoAnimal** (VARCHAR(50)) - Tipo de animal: perro, gato, ave, conejo, roedor, reptil, otro
3. **raza_TipoPelaje** (VARCHAR(30)) - Tipo de pelaje: corto, largo, rizado, sin pelo, semilargo
4. **raza_ImagenUrl** (VARCHAR(500)) - URL de imagen representativa de la raza

## Pasos para Aplicar los Cambios

### 1. Ejecutar el Script de ALTER TABLE
Este script agrega las nuevas columnas a la tabla existente.

```sql
-- Ejecutar el archivo: ALTER_TABLE_tbRazas_AddNewFields.sql
-- Conexión: DESKTOP-06VA2CI
-- Base de datos: PETSHOMEDB
```

**Comando desde SQL Server Management Studio:**
1. Conectarse a: `DESKTOP-06VA2CI`
2. Seleccionar base de datos: `PETSHOMEDB`
3. Abrir el archivo: `Scripts\ALTER_TABLE_tbRazas_AddNewFields.sql`
4. Ejecutar el script (F5)

**Comando desde sqlcmd:**
```bash
sqlcmd -S DESKTOP-06VA2CI -d PETSHOMEDB -i "Scripts\ALTER_TABLE_tbRazas_AddNewFields.sql"
```

### 2. Ejecutar el Script de Stored Procedures
Este script actualiza los stored procedures para manejar los nuevos campos.

```sql
-- Ejecutar el archivo: SP_Refugio_Razas_Insert_Update.sql
```

**Stored Procedures Actualizados:**
- `[Refugio].[PR_Refugio_Razas_Insert]` - INSERT con nuevos campos
- `[Refugio].[PR_Refugio_Razas_Update]` - UPDATE con nuevos campos
- `[Refugio].[PR_Refugio_Razas_List]` - SELECT para listado
- `[Refugio].[PR_Refugio_Razas_Find]` - SELECT por ID para edición
- `[Refugio].[PR_Refugio_Razas_Detail]` - SELECT detallado con auditoría

**Comando desde SQL Server Management Studio:**
1. Conectarse a: `DESKTOP-06VA2CI`
2. Seleccionar base de datos: `PETSHOMEDB`
3. Abrir el archivo: `Scripts\SP_Refugio_Razas_Insert_Update.sql`
4. Ejecutar el script (F5)

**Comando desde sqlcmd:**
```bash
sqlcmd -S DESKTOP-06VA2CI -d PETSHOMEDB -i "Scripts\SP_Refugio_Razas_Insert_Update.sql"
```

### 3. Compilar la Solución
Después de ejecutar los scripts SQL, compilar la solución para verificar que no hay errores.

```bash
dotnet build PetsHome.sln
```

### 4. Ejecutar la Aplicación
```bash
dotnet run --project PetsHome.UI/PetsHome.UI.csproj
```

## Archivos Modificados en el Código

### Capa Common (Entidades)
- `PetsHome.Common/Entities/Refugio/tbRazas.cs` - Entidad principal
- `PetsHome.Common/Entities/Refugio/PR_Refugio_Razas_ListResult.cs` - Resultado de listado
- `PetsHome.Common/Entities/Refugio/PR_Refugio_Razas_FindResult.cs` - Resultado de búsqueda
- `PetsHome.Common/Entities/Refugio/PR_Refugio_Razas_DetailResult.cs` - Resultado de detalle

### Capa Logic (Repositorio)
- `PetsHome.Logic/Repositories/RazaRepository.cs` - Métodos AddAsync y EditAsync actualizados

### Capa Business (ViewModels)
- `PetsHome.Business/Models/RazaViewModel.cs` - ViewModel con validaciones

### Capa UI (Interfaz de Usuario)
- `PetsHome.UI/Views/Catalogo/Raza/Index.cshtml` - Vista actualizada con nuevos campos
- `PetsHome.UI/wwwroot/js/pages/raza.js` - DataTable con nuevas columnas

## Verificación

### Verificar Campos en la Base de Datos
```sql
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'Refugio'
    AND TABLE_NAME = 'tbRazas'
ORDER BY ORDINAL_POSITION
```

### Verificar Stored Procedures
```sql
-- Verificar que los SP se crearon correctamente
SELECT
    ROUTINE_NAME,
    CREATED,
    LAST_ALTERED
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'Refugio'
    AND ROUTINE_NAME LIKE '%Razas%'
ORDER BY ROUTINE_NAME
```

## Funcionalidad en la Interfaz

Una vez aplicados todos los cambios, la pantalla de Razas incluirá:

1. **Formulario de Creación/Edición:**
   - Campo de Descripción (obligatorio)
   - Selector de Tipo de Animal (Perro, Gato, Ave, Conejo, Roedor, Reptil, Otro)
   - Selector de Tamaño (Pequeño, Mediano, Grande, Gigante)
   - Selector de Tipo de Pelaje (Corto, Largo, Rizado, Sin pelo, Semilargo)
   - Campo de URL de Imagen

2. **Tabla de Listado:**
   - Columna ID
   - Columna Descripción
   - Columna Tipo Animal
   - Columna Tamaño
   - Columna Tipo Pelaje
   - Columna Acciones (Editar, Eliminar)

## Notas Importantes

- Los nuevos campos son opcionales (NULL permitido)
- El campo `raza_Descripcion` sigue siendo obligatorio
- La validación de espacios en blanco al inicio se mantiene
- Los campos de auditoría (usuario creación/modificación, fechas) se mantienen sin cambios
- La imagen se almacena como URL, no como archivo binario

## Rollback (En caso de necesitar revertir)

Si necesita revertir los cambios:

```sql
-- Revertir columnas agregadas
ALTER TABLE [Refugio].[tbRazas] DROP COLUMN raza_Tamano
ALTER TABLE [Refugio].[tbRazas] DROP COLUMN raza_TipoAnimal
ALTER TABLE [Refugio].[tbRazas] DROP COLUMN raza_TipoPelaje
ALTER TABLE [Refugio].[tbRazas] DROP COLUMN raza_ImagenUrl
```

Luego restaurar los stored procedures originales desde un backup.

## Soporte

Para cualquier problema durante la actualización, verificar:
1. Conexión a la base de datos
2. Permisos del usuario en SQL Server
3. Logs de la aplicación en `Logs/mascota-log-.txt`
4. Errores de compilación con `dotnet build`
