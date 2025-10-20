# PetsHome

Sistema de gestión para refugios de animales que permite administrar mascotas, adopciones, voluntarios, empleados, eventos y más.

## Tecnologías

- **Backend**: ASP.NET Core 3.1 MVC
- **Base de datos**: SQL Server
- **ORM**: Entity Framework Core 5.0.11 + Dapper 2.0.90
- **Frontend**: Razor Views, JavaScript, DataTables
- **Logging**: Serilog

## Arquitectura del Proyecto

El proyecto sigue una arquitectura en capas:

```
PetsHome/
├── PetsHome.UI/              # Capa de presentación (MVC)
├── PetsHome.Business/        # Lógica de negocio y servicios
├── PetsHome.Logic/           # Capa de lógica intermedia
├── PetsHome.DataAccess/      # Acceso a datos (EF Core + Dapper)
└── PetsHome.Common/          # Modelos y entidades comunes
```

## Funcionalidades Principales

- **Gestión de Mascotas**: Registro, seguimiento y actualización de información de mascotas
- **Adopciones**: Control del proceso de adopción de mascotas
- **Refugios**: Administración de refugios y localidades
- **Empleados y Voluntarios**: Gestión de personal
- **Eventos**: Organización de eventos relacionados con el refugio
- **Historial Médico**: Seguimiento de vacunas y atención veterinaria
- **Inventario**: Control de items y recursos del refugio
- **Solicitudes**: Gestión de solicitudes de adopción

## Requisitos Previos

- [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet/3.1) o superior
- SQL Server 2016 o superior
- Visual Studio 2019/2022 (recomendado) o VS Code

## Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/JonatanFajardo/PetsHome.git
   cd PetsHome
   ```

2. **Configurar la base de datos**

   Editar `PetsHome.UI/appsettings.json` y configurar la cadena de conexión:
   ```json
   "ConnectionStrings": {
     "PetsHomeConnectionString": "Data source=TU_SERVIDOR;Initial Catalog=PETSHOMEDB;Integrated Security=True;Encrypt=False"
   }
   ```

3. **Crear la base de datos**

   Ejecutar el script SQL para crear la base de datos `PETSHOMEDB` con todas las tablas necesarias.

4. **Configurar la ruta de imágenes**

   Crear la carpeta para almacenar imágenes de mascotas (por defecto: `C:\PetsHome_Files\Mascotas`)
   o cambiar la ruta en `appsettings.json`:
   ```json
   "Filepath": {
     "pathMascotaImage": "C:\\TU_RUTA\\Mascotas"
   }
   ```

5. **Restaurar dependencias**
   ```bash
   dotnet restore
   ```

6. **Compilar el proyecto**
   ```bash
   dotnet build
   ```

7. **Ejecutar la aplicación**
   ```bash
   cd PetsHome.UI
   dotnet run
   ```

8. **Acceder a la aplicación**

   Abrir el navegador en `https://localhost:5001`

## Estructura de Base de Datos

El sistema gestiona las siguientes entidades principales:

- Mascotas (Raza, Categoría, Procedencia)
- Adopciones y Solicitudes
- Refugios y Localidades
- Empleados (con cargos)
- Voluntarios
- Eventos
- Historial Médico y Vacunas
- Inventario e Items

## Configuración de Logs

Los logs se generan automáticamente en la carpeta `Logs/` con rotación diaria.
Configurar niveles de log en `appsettings.json` bajo la sección `Serilog`.

## Desarrollo

### Ejecutar en modo desarrollo

```bash
dotnet run --environment Development
```

### Agregar migraciones (si se usa EF Migrations)

```bash
dotnet ef migrations add NombreMigracion --project PetsHome.DataAccess
dotnet ef database update --project PetsHome.DataAccess
```

## Contribuir

1. Fork el proyecto
2. Crear una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abrir un Pull Request

## Autor

**Jonatan Fajardo** - [GitHub](https://github.com/JonatanFajardo)

## Licencia

Este proyecto es privado y su uso está restringido a fines educativos o autorizados por el autor.

---

**Nota**: Este proyecto fue iniciado el 7 de febrero de 2022.
