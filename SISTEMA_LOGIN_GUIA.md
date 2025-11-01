# Sistema de Login - PetsHome

## Resumen de Implementación

Se ha implementado un sistema de autenticación completo para el proyecto PetsHome utilizando ASP.NET Core 3.1 con autenticación por cookies y la infraestructura de base de datos existente.

---

## Archivos Creados

### 1. Capa Common (Entidades)
- `PetsHome.Common/Entities/Seguridad/PR_Seguridad_Usuarios_LoginResult.cs`
- `PetsHome.Common/Entities/Seguridad/PR_Seguridad_Usuarios_LoginInResult.cs`
- `PetsHome.Common/Entities/Seguridad/PR_Seguridad_Usuarios_LogoutResult.cs`

### 2. Capa Business (ViewModels y Servicios)
- `PetsHome.Business/Models/LoginViewModel.cs` - Modelo del formulario de login
- `PetsHome.Business/Models/UsuarioViewModel.cs` - Modelo del usuario autenticado
- `PetsHome.Business/Services/UsuarioService.cs` - Servicio de autenticación

### 3. Capa Logic (Repositorios)
- `PetsHome.Logic/Repositories/UsuarioRepository.cs` - Repositorio de usuarios

### 4. Capa UI (Controladores y Vistas)
**Controlador:**
- `PetsHome.UI/Controllers/AccountController.cs`

**Vistas:**
- `PetsHome.UI/Views/Account/Login.cshtml` - Página de login
- `PetsHome.UI/Views/Account/AccessDenied.cshtml` - Página de acceso denegado
- `PetsHome.UI/Views/Shared/_LoginLayout.cshtml` - Layout para páginas de autenticación
- `PetsHome.UI/Views/Shared/_UserInfo.cshtml` - Componente de información de usuario

**Estilos:**
- `PetsHome.UI/wwwroot/css/login.css` - Estilos personalizados para login

---

## Archivos Modificados

### 1. Configuración de Servicios
- `PetsHome.Business/ServiceConfiguration.cs`
  - Agregado: `services.AddScoped<UsuarioRepository>()`
  - Agregado: `services.AddScoped<UsuarioService>()`

### 2. Configuración de AutoMapper
- `PetsHome.Business/Extensions/MappingProfileExtensions.cs`
  - Agregados mapeos para `PR_Seguridad_Usuarios_LoginResult` y `UsuarioViewModel`

### 3. Configuración de Startup
- `PetsHome.UI/Startup.cs`
  - Agregada autenticación por cookies
  - Configurado `app.UseAuthentication()`
  - Cambiada ruta por defecto a `/Account/Login`

---

## Funcionalidades Implementadas

### 1. Login
- Formulario de login con validaciones
- Autenticación mediante procedimiento almacenado `UDP_Acce_tbUsuarios_Login`
- Hash de contraseñas con SHA256
- Soporte para "Recordarme" (sesión extendida a 30 días)
- Registro de último acceso en base de datos
- Claims personalizados (ID, Nombre, Rol, Empleado)

### 2. Logout
- Cierre de sesión en ASP.NET Core
- Actualización de estado en base de datos
- Registro de evento de logout
- Redirección a página de login

### 3. Seguridad
- Cookies HTTP-only
- Protección CSRF con ValidateAntiForgeryToken
- Expiración de sesión configurable (8 horas por defecto)
- Sliding expiration habilitado
- Validación de credenciales en base de datos

### 4. Interfaz de Usuario
- Diseño moderno y responsivo
- Gradiente de fondo
- Animaciones CSS
- Toggle para mostrar/ocultar contraseña
- Mensajes de error amigables
- Componente de información de usuario en navbar

---

## Cómo Usar el Sistema de Login

### 1. Primera Ejecución

Detener la aplicación si está corriendo y compilar el proyecto:

```bash
dotnet build PetsHome.sln
```

### 2. Ejecutar la Aplicación

```bash
cd PetsHome.UI
dotnet run
```

La aplicación ahora redirigirá a `/Account/Login` por defecto.

### 3. Credenciales de Prueba

Según los datos de la base de datos, existe un usuario:
- **Usuario:** `admin`
- **Contraseña:** (La que esté configurada en la base de datos con hash SHA256)

**IMPORTANTE:** Si la contraseña no funciona, es posible que necesites actualizar el hash en la base de datos.

### 4. Actualizar Contraseña de Admin (si es necesario)

Para generar un hash SHA256 de una contraseña (por ejemplo "admin123"):

```sql
-- Ejemplo: Actualizar contraseña del usuario admin
UPDATE [Seguridad].[tbUsuarios]
SET Usu_PasswordHash = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9'
WHERE usu_Id = 1;
-- Este hash corresponde a la contraseña: "admin123"
```

Puedes generar tu propio hash usando este código C#:

```csharp
using System.Security.Cryptography;
using System.Text;

string password = "tu_contraseña_aqui";
using (SHA256 sha256 = SHA256.Create())
{
    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    StringBuilder builder = new StringBuilder();
    foreach (byte b in bytes)
    {
        builder.Append(b.ToString("x2"));
    }
    Console.WriteLine(builder.ToString());
}
```

---

## Flujo de Autenticación

```
1. Usuario accede a cualquier página
   ↓
2. Middleware de autenticación verifica si está autenticado
   ↓
3. Si NO está autenticado → Redirige a /Account/Login
   ↓
4. Usuario ingresa credenciales
   ↓
5. AccountController.Login procesa el formulario
   ↓
6. UsuarioService.AuthenticateAsync valida credenciales
   ↓
7. UsuarioRepository.LoginAsync llama al procedimiento almacenado
   ↓
8. Si es válido:
   - Crea Claims del usuario
   - Inicia sesión con cookies
   - Registra login en BD (UDP_Acce_tbUsuarios_LoginIn)
   - Redirige a Home
   ↓
9. Usuario autenticado puede navegar por la aplicación
   ↓
10. Al hacer Logout:
    - Cierra sesión en ASP.NET Core
    - Actualiza estado en BD (UDP_Acce_tbUsuarios_Logout)
    - Redirige a Login
```

---

## Integración con Vistas Existentes

### Agregar Información de Usuario en el Layout

En cualquier vista que use `_Layout.cshtml`, puedes incluir el componente de usuario en el navbar:

```html
@Html.Partial("~/Views/Shared/_UserInfo.cshtml")
```

### Proteger Controladores con [Authorize]

Para requerir autenticación en un controlador o acción:

```csharp
[Authorize] // Requiere usuario autenticado
public class MascotaController : BaseController
{
    // ...
}

[Authorize(Roles = "Administrador")] // Requiere rol específico
public IActionResult Delete(int id)
{
    // ...
}
```

### Permitir Acceso Anónimo

Para permitir acceso sin autenticación:

```csharp
[AllowAnonymous]
public IActionResult PublicPage()
{
    // ...
}
```

---

## Acceso a Información del Usuario Autenticado

### En Controladores

```csharp
// Obtener ID del usuario
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

// Obtener nombre de usuario
var username = User.Identity.Name;

// Obtener nombre completo
var nombreCompleto = User.FindFirst(ClaimTypes.GivenName)?.Value;

// Obtener rol
var rol = User.FindFirst(ClaimTypes.Role)?.Value;

// Verificar si está autenticado
if (User.Identity.IsAuthenticated)
{
    // ...
}

// Verificar rol
if (User.IsInRole("Administrador"))
{
    // ...
}
```

### En Vistas Razor

```csharp
@if (User.Identity.IsAuthenticated)
{
    <p>Bienvenido, @User.Identity.Name</p>

    var nombreCompleto = User.FindFirst(ClaimTypes.GivenName)?.Value;
    <p>@nombreCompleto</p>
}
```

---

## Personalización

### Cambiar Tiempo de Expiración de Sesión

En `Startup.cs`, modifica:

```csharp
options.ExpireTimeSpan = TimeSpan.FromHours(8); // Cambiar a lo deseado
```

### Cambiar Rutas de Login/Logout

En `Startup.cs`, modifica:

```csharp
options.LoginPath = "/Account/Login"; // Ruta de login
options.LogoutPath = "/Account/Logout"; // Ruta de logout
options.AccessDeniedPath = "/Account/AccessDenied"; // Acceso denegado
```

### Mejorar Seguridad del Hash de Contraseñas

**Recomendación:** El hash SHA256 actual es funcional pero no es el más seguro para contraseñas. Para producción, considera usar BCrypt o Argon2:

```bash
dotnet add package BCrypt.Net-Next
```

```csharp
// En UsuarioService.cs
private string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}

private bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

---

## Próximos Pasos (Opcional)

1. **Recuperación de Contraseña**
   - Usar procedimiento `PR_Seguridad_Usuarios_RecuperarContra`
   - Implementar envío de emails

2. **Cambio de Contraseña**
   - Usar procedimiento `PR_Seguridad_Usuarios_UpdatePassword`

3. **Gestión de Usuarios**
   - CRUD completo de usuarios
   - Asignación de roles

4. **Two-Factor Authentication (2FA)**
   - Implementar autenticación de dos factores

5. **Registro de Usuarios Nuevos**
   - Formulario de registro
   - Validación de emails únicos

6. **Gestión de Permisos Granular**
   - Usar las tablas `tbPermisos`, `tbModulos`, `tbPantallas`
   - Implementar autorización basada en permisos

---

## Solución de Problemas

### Error: "Usuario o contraseña incorrectos"

1. Verificar que el usuario existe en `[Seguridad].[tbUsuarios]`
2. Verificar que `Usu_EsActivo = 1`
3. Verificar que `Usu_Suspendido = 0`
4. Verificar que `Usu_EsEliminado = 0`
5. Verificar que el empleado asociado tiene `emp_EsActivo = 1`
6. Verificar que tiene al menos un rol activo
7. Verificar que el hash de la contraseña coincide

### Error al compilar

1. Cerrar la aplicación si está corriendo
2. Limpiar la solución: `dotnet clean`
3. Restaurar paquetes: `dotnet restore`
4. Compilar: `dotnet build`

### La página de login no tiene estilos

1. Verificar que existe `wwwroot/css/login.css`
2. Verificar que el archivo CSS se está sirviendo correctamente
3. Revisar la consola del navegador para errores 404

---

## Estructura de Base de Datos Utilizada

### Tablas Principales
- `[Seguridad].[tbUsuarios]` - Información de usuarios
- `[Seguridad].[tbRoles]` - Roles del sistema
- `[Seguridad].[tbRolesUsuarios]` - Relación usuarios-roles
- `[Refugio].[tbEmpleados]` - Empleados vinculados a usuarios
- `[General].[tbPersonas]` - Información personal

### Procedimientos Almacenados
- `[Seguridad].[UDP_Acce_tbUsuarios_Login]` - Validar credenciales
- `[Seguridad].[UDP_Acce_tbUsuarios_LoginIn]` - Marcar como logueado
- `[Seguridad].[UDP_Acce_tbUsuarios_Logout]` - Marcar como deslogueado

---

## Contacto y Soporte

Para cualquier duda o problema con el sistema de login, revisar:
1. Este documento
2. Comentarios en el código fuente
3. Logs de la aplicación en `Logs/mascota-log-*.txt`

---

**Fecha de Implementación:** Octubre 2025
**Versión del Sistema:** 1.0
**Tecnología:** ASP.NET Core 3.1 MVC + SQL Server
