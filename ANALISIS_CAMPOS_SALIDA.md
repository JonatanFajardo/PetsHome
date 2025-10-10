# Análisis de Campos - Pantalla Create de Salida

## ❌ **Problemas Identificados**

El `SalidaViewModel.cs` actual **NO COINCIDE** con la estructura de base de datos definida para `tbSalidas`.

## 📊 **Comparación: BD vs ViewModel Actual**

### **Campos en tbSalidas (Base de Datos)**
```csharp
public int sal_Id { get; set; }
public string sal_Descripcion { get; set; }
public DateTime sal_Fecha { get; set; }
public string sal_TipoSalida { get; set; }
public int? sal_DestinoId { get; set; }
public string sal_NumeroDocumento { get; set; }
public int refg_Id { get; set; }
public bool sal_EsEliminado { get; set; }
public int sal_UsuarioCrea { get; set; }
public DateTime sal_FechaCrea { get; set; }
public int? sal_UsuarioModifica { get; set; }
public DateTime? sal_FechaModifica { get; set; }
```

### **Campos en SalidaViewModel Actual (INCORRECTO)**
```csharp
public int sal_Id { get; set; }               // ✅ CORRECTO
public int? item_Id { get; set; }             // ❌ NO EXISTE EN BD
public string itm_Descripcion { get; set; }  // ❌ NO EXISTE EN BD
public DateTime? sal_Fecha { get; set; }      // ✅ CORRECTO
public int? sal_Cantidad { get; set; }        // ❌ NO EXISTE EN BD (va en SalidasDetalles)
public bool? esEliminado { get; set; }        // ❌ NOMBRE INCORRECTO
public DateTime fechaCreacion { get; set; }   // ❌ NOMBRE INCORRECTO
public DateTime? fechaModificacion { get; set; } // ❌ NOMBRE INCORRECTO
public int refg_Id { get; set; }              // ✅ CORRECTO
```

### **Campos FALTANTES en ViewModel**
```csharp
❌ sal_Descripcion
❌ sal_TipoSalida  
❌ sal_DestinoId
❌ sal_NumeroDocumento
❌ sal_EsEliminado
❌ sal_UsuarioCrea
❌ sal_FechaCrea
❌ sal_UsuarioModifica
❌ sal_FechaModifica
```

## 🔧 **Campos en Vista Create.cshtml**

### **Campos Usados en la Vista:**
- ✅ `sal_Fecha` - CORRECTO
- ✅ `sal_TipoSalida` - CORRECTO  
- ✅ `sal_NumeroDocumento` - CORRECTO
- ✅ `refg_Id` - CORRECTO
- ✅ `sal_DestinoId` - CORRECTO
- ✅ `sal_Descripcion` - CORRECTO

### **Problema Principal:**
La vista usa campos correctos, pero el `SalidaViewModel` **NO LOS TIENE DEFINIDOS**.

## ✅ **Solución Requerida**

Necesito **REEMPLAZAR** completamente el `SalidaViewModel.cs` actual con la versión que creé anteriormente que SÍ coincide con la base de datos.

## 📋 **Estructura Correcta Requerida**

```csharp
public partial class SalidaViewModel
{
    public int sal_Id { get; set; }
    public string sal_Descripcion { get; set; }
    public DateTime? sal_Fecha { get; set; }
    public string sal_TipoSalida { get; set; }
    public int? sal_DestinoId { get; set; }
    public string sal_NumeroDocumento { get; set; }
    public int refg_Id { get; set; }
    public string? refg_Nombre { get; set; }
    
    // Campos de auditoría
    public int? sal_UsuarioCrea { get; set; }
    public string? sal_NombreUsuarioCrea { get; set; }
    public DateTime sal_FechaCrea { get; set; }
    public int? sal_UsuarioModifica { get; set; }
    public string? sal_NombreUsuarioModifica { get; set; }
    public DateTime? sal_FechaModifica { get; set; }
    
    public bool isEdit => sal_Id > 0;
}
```

## 🚨 **Acción Inmediata Requerida**

1. ✅ Reemplazar `SalidaViewModel.cs` con la versión correcta
2. ✅ Verificar que todos los campos en Create.cshtml existan en el nuevo ViewModel
3. ✅ Actualizar mapeos de AutoMapper si es necesario

---

**Estado**: ❌ **CRÍTICO** - ViewModel actual incompatible con BD y Vista