#!/bin/bash
case "$GIT_COMMIT" in
    ba16a1633b33d616f6047a47e55dc0f3a23010ee) printf "%s" "feat(cita-medica): implementar módulo clínico con citas médicas, recetas y catálogos";;
    d93c67680694ad4ce0675b0a281a7ff6366c35e3) printf "%s" "feat(tratamiento): implementar vistas de detalle y formulario con dropdowns dinámicos";;
    *) cat;;
esac
