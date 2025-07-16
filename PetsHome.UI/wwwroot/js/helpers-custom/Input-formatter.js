class InputFormatter {
    constructor() {
        this.formatters = {
            identidad: {
                pattern: /^[0-9]{4}-[0-9]{4}-[0-9]{5}$/,
                maxLength: 13,
                format: this.formatIdentidad.bind(this),
                placeholder: "0000-0000-00000",
                errorMessage: "Formato inválido. Use: 0000-0000-00000"
            },
            telefono: {
                pattern: /^[0-9]{4}-[0-9]{4}$/,
                maxLength: 8,
                format: this.formatTelefono.bind(this),
                placeholder: "0000-0000",
                errorMessage: "Formato inválido. Use: 0000-0000"
            }
        };
    }

    // Inicializar todos los campos formateados
    init() {
        document.addEventListener('DOMContentLoaded', () => {
            this.initializeField('identidadInput', 'identidad');
            this.initializeField('telefonoInput', 'telefono');
        });
    }

    // Inicializar un campo específico
    initializeField(inputId, formatterType) {
        const input = document.getElementById(inputId);
        if (!input) return;

        const formatter = this.formatters[formatterType];
        if (!formatter) return;

        // Formatear valor inicial si existe (model binding)
        if (input.value && input.value.trim() !== '') {
            formatter.format(input);
        }

        // Agregar eventos
        this.addEventListeners(input, formatter);
    }

    // Agregar event listeners
    addEventListeners(input, formatter) {
        const events = ['input', 'paste', 'blur', 'focus'];
        
        events.forEach(eventType => {
            input.addEventListener(eventType, (e) => {
                if (eventType === 'paste') {
                    setTimeout(() => formatter.format(e.target), 10);
                } else {
                    formatter.format(e.target);
                }
            });
        });
    }

    // Formatear identidad: 0000-0000-00000
    formatIdentidad(input) {
        let value = input.value.replace(/\D/g, '');
        
        if (value.length > 13) {
            value = value.substring(0, 13);
        }
        
        let formattedValue = '';
        
        if (value.length > 0) {
            formattedValue = value.substring(0, 4);
            
            if (value.length > 4) {
                formattedValue += '-' + value.substring(4, 8);
                
                if (value.length > 8) {
                    formattedValue += '-' + value.substring(8, 13);
                }
            }
        }
        
        if (input.value !== formattedValue) {
            input.value = formattedValue;
        }
        
        //this.validateFormat(input, 'identidad');
    }

    // Formatear teléfono: 0000-0000
    formatTelefono(input) {
        let value = input.value.replace(/\D/g, '');
        
        if (value.length > 8) {
            value = value.substring(0, 8);
        }
        
        let formattedValue = '';
        
        if (value.length > 0) {
            formattedValue = value.substring(0, 4);
            
            if (value.length > 4) {
                formattedValue += '-' + value.substring(4, 8);
            }
        }
        
        if (input.value !== formattedValue) {
            input.value = formattedValue;
        }
        
        //this.validateFormat(input, 'telefono');
    }

    // Validar formato
    validateFormat(input, formatterType) {
        const formatter = this.formatters[formatterType];
        const value = input.value;
        
        // Buscar elemento de validación
        const validationElement = this.findValidationElement(input);
        
        if (validationElement) {
            if (value.length > 0 && !formatter.pattern.test(value)) {
                validationElement.textContent = formatter.errorMessage;
                validationElement.style.display = 'block';
                validationElement.classList.add('text-danger');
            } else {
                validationElement.textContent = '';
                validationElement.style.display = 'none';
            }
        }
    }

    // Encontrar elemento de validación
    findValidationElement(input) {
        const fieldName = input.getAttribute('asp-for') || input.name;
        
        return document.querySelector(`span[data-valmsg-for="${fieldName}"]`) ||
               document.querySelector(`span[asp-validation-for="${fieldName}"]`) ||
               document.getElementById(`validation-message-${fieldName}`);
    }

    // Métodos públicos para uso externo
    
    // Obtener valor sin formato
    getUnformattedValue(inputId) {
        const input = document.getElementById(inputId);
        return input ? input.value.replace(/\D/g, '') : '';
    }

    // Establecer valor y formatear
    setValue(inputId, value, formatterType) {
        const input = document.getElementById(inputId);
        const formatter = this.formatters[formatterType];
        
        if (input && formatter) {
            input.value = value;
            formatter.format(input);
        }
    }

    // Limpiar campo
    clearField(inputId) {
        const input = document.getElementById(inputId);
        if (input) {
            input.value = '';
            input.focus();
            
            // Limpiar validación
            const validationElement = this.findValidationElement(input);
            if (validationElement) {
                validationElement.textContent = '';
                validationElement.style.display = 'none';
            }
        }
    }

    // Forzar formateo
    forceFormat(inputId, formatterType) {
        const input = document.getElementById(inputId);
        const formatter = this.formatters[formatterType];
        
        if (input && formatter) {
            formatter.format(input);
        }
    }

    // Agregar nuevo formateador
    addFormatter(name, config) {
        this.formatters[name] = {
            pattern: config.pattern,
            maxLength: config.maxLength,
            format: config.format.bind(this),
            placeholder: config.placeholder,
            errorMessage: config.errorMessage
        };
    }

    // Validar todos los campos
    validateAllFields() {
        const results = {};
        
        Object.keys(this.formatters).forEach(type => {
            const input = document.getElementById(`${type}Input`);
            if (input) {
                const formatter = this.formatters[type];
                const isValid = formatter.pattern.test(input.value);
                results[type] = {
                    value: input.value,
                    isValid: isValid,
                    unformatted: this.getUnformattedValue(`${type}Input`)
                };
            }
        });
        
        return results;
    }
}

// Instanciar y inicializar
const inputFormatter = new InputFormatter();
inputFormatter.init();

// Funciones globales para compatibilidad (opcional)
function formatearIdentidad(input) {
    inputFormatter.formatters.identidad.format(input);
}

function formatearTelefono(input) {
    inputFormatter.formatters.telefono.format(input);
}

function obtenerValorSinFormato(inputId) {
    return inputFormatter.getUnformattedValue(inputId);
}

function establecerValor(inputId, value, type) {
    inputFormatter.setValue(inputId, value, type);
}

function limpiarCampo(inputId) {
    inputFormatter.clearField(inputId);
}

// Ejemplo de uso:
// inputFormatter.setValue('identidadInput', '1234567890123', 'identidad');
// console.log(inputFormatter.validateAllFields());
// inputFormatter.clearField('telefonoInput');