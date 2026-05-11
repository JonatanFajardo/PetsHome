# Sistema de Login - PetsHome ✅

## Inicio Rápido

### Credenciales de Prueba
```
Usuario: admin
Contraseña: admin123
```

### Pasos para Ejecutar

1. **Detener la aplicación** si está corriendo actualmente

2. **Compilar el proyecto:**
   ```bash
   cd C:\Users\movie\Desktop\PetsHome
   dotnet build PetsHome.sln
   ```

3. **Ejecutar la aplicación:**
   ```bash
   cd PetsHome.UI
   dotnet run
   ```

4. **Abrir el navegador:**
   - La aplicación redirigirá automáticamente a `/Account/Login`
   - Ingresar las credenciales de prueba
   - ¡Listo! El sistema está funcionando

---

## Archivos Importantes

### Documentación Completa
📄 **[SISTEMA_LOGIN_GUIA.md](./SISTEMA_LOGIN_GUIA.md)** - Guía completa de implementación

### Scripts de Base de Datos
📄 **[crear_usuario_prueba.sql](./crear_usuario_prueba.sql)** - Script para crear/actualizar usuario de prueba

---

## Estructura Implementada

```
✅ Entidades (Common Layer)
   - PR_Seguridad_Usuarios_LoginResult.cs
   - PR_Seguridad_Usuarios_LoginInResult.cs
   - PR_Seguridad_Usuarios_LogoutResult.cs

✅ ViewModels (Business Layer)
   - LoginViewModel.cs
   - UsuarioViewModel.cs

✅ Servicios (Business Layer)
   - UsuarioService.cs

✅ Repositorios (Logic Layer)
   - UsuarioRepository.cs

✅ Controladores (UI Layer)
   - AccountController.cs

✅ Vistas (UI Layer)
   - Views/Account/Login.cshtml
   - Views/Account/AccessDenied.cshtml
   - Views/Shared/_LoginLayout.cshtml
   - Views/Shared/_UserInfo.cshtml

✅ Estilos
   - wwwroot/css/login.css

✅ Configuración
   - Startup.cs (Autenticación habilitada)
   - ServiceConfiguration.cs (Servicios registrados)
   - MappingProfileExtensions.cs (Mapeos agregados)
```

---

## Características

- ✅ Autenticación por cookies
- ✅ Hash de contraseñas (SHA256)
- ✅ Validación contra base de datos
- ✅ Registro de sesiones (login/logout)
- ✅ Opción "Recordarme"
- ✅ Diseño moderno y responsivo
- ✅ Protección CSRF
- ✅ Sistema de roles integrado
- ✅ Claims personalizados
- ✅ Páginas de error personalizadas

---

## Próximos Pasos (Opcional)

1. Proteger controladores con `[Authorize]`
2. Implementar recuperación de contraseña
3. Agregar gestión de usuarios (CRUD)
4. Implementar cambio de contraseña
5. Mejorar seguridad con BCrypt

---

## Solución Rápida de Problemas

### No puedo compilar
- Cerrar la aplicación si está corriendo
- Ejecutar: `dotnet clean && dotnet build`

### Las credenciales no funcionan
- Ejecutar el script: `crear_usuario_prueba.sql`
- Verificar que el hash coincida con "admin123"

### No veo los estilos
- Verificar que existe: `wwwroot/css/login.css`
- Limpiar caché del navegador (Ctrl+F5)

---

## Contacto

Para más información, revisar la documentación completa en:
**[SISTEMA_LOGIN_GUIA.md](./SISTEMA_LOGIN_GUIA.md)**

---

**Estado:** ✅ Implementación Completa
**Fecha:** Octubre 2025
**Versión:** 1.0
