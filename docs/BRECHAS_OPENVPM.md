
---

## Ficha del paciente: OpenVPM vs PetsHome (2026-10-29)

**Regla:** la ficha (expediente) y la consulta (visita activa) son DOS pantallas distintas, como en OpenVPM. Ficha = historia completa del animal (varias visitas, read-mostly, muchas pestañas). Consulta = herramienta de trabajo de UNA visita (SOAP, vitales, cargos, recetas). Si la ficha se "parece" a la consulta es porque está incompleta.

**OpenVPM — ficha del paciente** (`patients/[id]/page.tsx`): pestañas Overview · Medical Records · Documents (Photos/Consents/Documents/Lab reports) · Appointments · Weight History · Vitals · Vaccinations (+certificado) · Invoices.
**OpenVPM — consulta** (`encounters/[appointmentId]/page.tsx`): SOAP · Vitals · Charges/Payment · Prescriptions · Diagnosis/Discharge · Treatment plan.

**Comparación de pestañas de la FICHA:**

| Pestaña (OpenVPM)        | PetsHome (al 2026-10-29) |
|--------------------------|--------------------------|
| Overview / Resumen       | ✅ |
| Registros médicos        | ◐ (Historial/Citas) |
| Citas                    | ✅ |
| Peso (con gráfica)       | ❌ falta |
| Signos vitales           | ❌ falta |
| Vacunas (+certificado)   | ✅ (certificado agregado) |
| Facturas                 | ❌ falta |
| Documentos / Laboratorio | ❌ falta |

**Pendiente:** enriquecer la ficha de PetsHome con Facturas · Signos vitales · Peso (gráfica) · Laboratorio · Documentos, leyendo datos que ya existen, para que sea el expediente completo como OpenVPM. (Documentos requiere almacenamiento de archivos → puede diferirse.)
