
// CitaMedica mejorado con mejor control de errores
var CitaMedica = (function () {
    var obj = {};

    obj.datatable = function (Direction) {
        // Validación de parámetros
        if (!Direction) {
            console.error('CitaMedica.datatable: Parámetro Direction es requerido');
            return false;
        }

        if (!Direction.urlList) {
            console.error('CitaMedica.datatable: Direction.urlList es requerido');
            return false;
        }

        $(function () {
            try {
                console.log("Inicializando CitaMedica datatable");

                // Configuración de headers
                var header = [
                    //{ FieldName: "medic_Id" },
                    { FieldName: "masc_Nombre" },
                    { FieldName: "medic_FechaConsulta"},
                    { FieldName: "medic_TipoConsulta"},
                    { FieldName: "medic_Diagnostico"},
                    { FieldName: "medic_Peso"},
                    { FieldName: "medic_Temperatura"},
                    { FieldName: "medic_ProximaCita"},
                ];

                // Validar que header tenga elementos
                if (!header || header.length === 0) {
                    throw new Error('La configuración de header está vacía');
                }

                console.log("URL de lista:", Direction.urlList);
                console.log("Configuración de header:", header);

                // Verificar que datatable existe y tiene el método init
                if (typeof datatable === 'undefined') {
                    throw new Error('El objeto datatable no está definido');
                }

                if (typeof datatable.init !== 'function') {
                    throw new Error('datatable.init no es una función');
                }

                // Inicializar datatable con timeout para manejo de carga
                setTimeout(function () {
                    try {
                        datatable.init(Direction, header);
                        console.log("DataTable inicializado correctamente");
                    } catch (initError) {
                        console.error("Error en datatable.init (timeout):", initError);

                        // Intentar mostrar mensaje de error al usuario
                        if (typeof Swal !== 'undefined') {
                            Swal.fire({
                                icon: 'error',
                                title: 'Error',
                                text: 'No se pudo cargar la tabla de datos'
                            });
                        } else {
                            alert('Error: No se pudo cargar la tabla de datos');
                        }
                    }
                }, 100);

            } catch (error) {
                console.error("ERROR en CitaMedica.datatable:", error);

                // Log detallado del error
                console.error("Stack trace:", error.stack);
                console.error("Direction object:", Direction);

                // Mostrar error al usuario
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error de Configuración',
                        text: 'Hubo un problema al configurar la tabla: ' + error.message
                    });
                } else if (typeof toastr !== 'undefined') {
                    toastr.error('Error al configurar la tabla: ' + error.message);
                } else {
                    alert('Error: ' + error.message);
                }

                return false;
            }
        });

        return true;
    };

    // Método adicional para validar configuración
    obj.validateConfig = function (Direction, header) {
        var errors = [];

        if (!Direction) {
            errors.push('Direction es requerido');
        } else {
            if (!Direction.urlList) errors.push('Direction.urlList es requerido');
        }

        if (!header || !Array.isArray(header)) {
            errors.push('Header debe ser un array');
        } else if (header.length === 0) {
            errors.push('Header no puede estar vacío');
        }

        return {
            isValid: errors.length === 0,
            errors: errors
        };
    };

    return obj;
}());