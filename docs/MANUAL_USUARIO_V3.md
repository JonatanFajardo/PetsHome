# Manual de usuario — Novedades PetsHome

> Guía rápida de las funciones nuevas, explicadas de forma sencilla.
> Para cada pantalla: **qué hace** y **para qué le sirve a cada rol**.

---

## 1. Banderas rojas de la mascota (alergias y padecimientos)

**Qué es:** un aviso visible en la parte de arriba de la ficha de la mascota y en la pantalla de consulta, que muestra a qué es **alérgica** la mascota y qué **enfermedades crónicas** tiene.

**Para qué sirve:** que el veterinario vea de inmediato, sin buscar, si la mascota tiene una alergia o un padecimiento importante **antes de recetar o tratar**. Evita darle un medicamento que le haga daño.

**Cómo se usa:**
- Al abrir la ficha o atender una consulta, el aviso aparece solo.
- Con **"+ Agregar"** se registra una alergia (sustancia, reacción y qué tan grave: leve / media / alta) o un padecimiento (nombre, estado y desde cuándo).
- La **✕** de cada etiqueta la quita de la lista si se registró por error (queda guardada como corregida, no se borra del historial).

**Dónde:** ficha del Perfil Médico · pantalla En Consulta.

| Rol | Le sirve para |
|-----|---------------|
| Veterinario | Ver el riesgo al instante antes de recetar |
| Cualquiera que abra la ficha | Conocer alergias/crónicos de la mascota |

---

## 2. Cobros pendientes (caja)

**Qué es:** una lista de los **medicamentos entregados en consulta a clientes** que todavía **no se han cobrado**.

**Para qué sirve:** separar el trabajo del veterinario (entregar el medicamento) del trabajo de caja (cobrarlo). El veterinario atiende sin frenarse; el cobro queda pendiente para que recepción/caja lo facture después.

**Cómo se usa:**
- Entra a **Cobros pendientes** (menú Clínica).
- Verás, por cliente, lo entregado y su monto.
- **"Cobrar cliente"** genera la factura con todo lo pendiente de ese cliente (ya con el impuesto incluido).
- **"Condonar"** marca un cargo como cortesía (hay que escribir el motivo).

**Importante:** el precio que se cobra es el que tenía el producto **el día que se entregó** (aunque después cambie el precio en el catálogo).

| Rol | Le sirve para |
|-----|---------------|
| Recepción / Caja | Cobrar lo que el veterinario entregó |
| Administrador | Supervisar lo pendiente y autorizar cortesías |

---

## 3. Tablero del día (sala de espera visual)

**Qué es:** un tablero con **tres columnas** — *En espera*, *En atención* y *Finalizada* — donde cada mascota del día aparece como una tarjeta.

**Para qué sirve:** ver de un vistazo quién está en la clínica y en qué fase va, y mover a los pacientes conforme avanzan.

**Cómo se usa:**
- Entra a **Tablero del día** (menú Recepción).
- Cada tarjeta muestra la mascota, el dueño (o si es interno del refugio), el doctor y cuánto tiempo lleva.
- **Arrastra** una tarjeta de una columna a otra para cambiar su estado (de "En espera" a "En atención", etc.).
- Se actualiza solo cada pocos segundos.

| Rol | Le sirve para |
|-----|---------------|
| Recepción | Gestionar la fila del día y mover pacientes |
| Veterinario | Ver a quién sigue atender |
| Administrador | Supervisar el flujo de la clínica |

---

## 4. Agenda / Calendario con vista por rol

**Qué es:** el calendario de citas, que ahora **muestra a cada quien lo que le corresponde** según su rol.

**Para qué sirve:** que el veterinario no se distraiga con citas que no son suyas, y que recepción/administración puedan ver todo o concentrarse en un doctor.

**Cómo funciona según el rol:**

```
/CitaMedica/Calendario

  · Como VETERINARIO      →  ve SOLO sus citas (y las que aún no tienen
                             doctor asignado). No ve las de otros doctores.

  · Como ADMIN / RECEPCIÓN →  ve TODAS las citas, y arriba del calendario
                             tiene un selector "Doctor" para filtrar y ver
                             solo las de un veterinario en particular.
```

| Rol | Qué ve en la agenda |
|-----|---------------------|
| Veterinario | Solo sus citas + las sin asignar |
| Recepción | Todas, con opción de filtrar por doctor |
| Administrador / Supervisor | Todas, con opción de filtrar por doctor |
| Otros roles | No aplica |

**Cómo se usa el filtro (recepción/admin):** en el panel lateral del calendario, elige un doctor en el selector **"Doctor"** para ver solo su agenda, o "Todos los doctores" para ver la clínica completa.

---

## 5. Consulta en formato SOAP (nota clínica ordenada)

**Qué es:** la pantalla **En Consulta** ahora está organizada en cuatro bloques que siguen el orden natural de una revisión médica. Son las iniciales **SOAP**, el estándar que usan las clínicas veterinarias para dejar la historia clínica ordenada.

**Para qué sirve:** que el veterinario capture la consulta en el mismo orden en que piensa (primero escucha, luego examina, luego concluye y al final decide el tratamiento). La nota queda clara y completa, y nada importante se queda sin anotar.

**Los cuatro bloques:**

```
S · Subjetivo   →  Lo que cuenta el dueño: el motivo de la visita y
                   qué ha notado en la mascota. (tipo de consulta + motivo)

O · Objetivo    →  Lo que mide el veterinario: los signos vitales.
                   peso · temperatura · frecuencia cardíaca · frecuencia
                   respiratoria · color de mucosas · llenado capilar ·
                   escala de dolor · condición corporal.

A · Evaluación  →  La conclusión del veterinario: el diagnóstico.

P · Plan        →  Qué se va a hacer: medicamentos recetados, vacuna
                   aplicada y la próxima cita de seguimiento.
```

**Signos vitales ampliados (bloque Objetivo):** además del peso y la temperatura que ya existían, ahora se pueden registrar frecuencia cardíaca, frecuencia respiratoria, color de mucosas, tiempo de llenado capilar, escala de dolor (0 a 10) y condición corporal (1 a 9). **Todos son opcionales**: se llena lo que se haya medido.

**Aviso de rango:** si un valor se sale de lo normal (por ejemplo, una temperatura muy alta), el campo se marca en **ámbar** con una nota breve. Es solo un **aviso** para que el veterinario lo revise; **no impide guardar** la consulta.

**Qué NO cambió:** sigue estando el **aviso de banderas rojas** (alergias y padecimientos) a la derecha, el **buscador de medicamentos**, la casilla **"Entregar del inventario"** y el botón **Finalizar consulta**. Solo se reordenó la pantalla y se agregaron más signos vitales.

**Dónde:** pantalla **En Consulta**.

| Rol | Le sirve para |
|-----|---------------|
| Veterinario | Capturar la consulta ordenada y registrar más signos vitales |
| Quien lea el historial después | Entender rápido qué pasó en cada visita |

---

## Preguntas frecuentes

**¿Por qué el veterinario no ve las citas de otros?**
Para que cada uno se enfoque en su propia agenda. Recepción y administración sí ven todo.

**¿Por qué un cobro muestra un precio distinto al del catálogo actual?**
Porque se cobra el precio que tenía el producto cuando se entregó, no el de hoy. Así el histórico es justo.

**¿Qué pasa si registro una alergia por error?**
Usa la ✕ para quitarla; queda marcada como corregida, sin perder el registro.

**¿Los cambios de permisos se aplican al instante?**
No: si se cambian permisos de un usuario, debe **cerrar sesión y volver a entrar** para que tomen efecto.
