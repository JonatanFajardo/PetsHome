// ============================================
// HOME.JS - PetsHome Dashboard
// ============================================

$(document).ready(function () {
    initializePage();
});

// Inicializar la página
function initializePage() {
    obtenerDatosClima();
}

// ============================================
// MÓDULO DE CLIMA
// ============================================

// Configuración de la API de Clima
const apiKey = '565c6073a5c841bd81385441242012';
const defaultLocation = 'Honduras';
const weatherUrl = `https://api.weatherapi.com/v1/current.json?key=${apiKey}&q=${defaultLocation}&aqi=no`;

// Elementos del DOM para el clima
const weatherElements = {
    location: $('#weather-location'),
    icon: $('#weather-icon'),
    temperature: $('#weather-temperature'),
    condition: $('#weather-condition'),
    feelsLike: $('#weather-feels-like'),
    humidity: $('#weather-humidity'),
    windSpeed: $('#weather-wind-speed'),
    windDirection: $('#weather-wind-direction'),
    visibility: $('#weather-visibility'),
    errorMessage: $('#weather-error-message')
};

// Función principal para obtener datos del clima
async function obtenerDatosClima() {
    try {
        const data = await $.ajax({
            url: weatherUrl,
            dataType: 'json',
            timeout: 10000 // Timeout de 10 segundos
        });

        if (data && data.current && data.location) {
            actualizarInterfazClima(data);
        } else {
            throw new Error('Datos incompletos de la API');
        }

    } catch (error) {
        console.error('Error al obtener datos del clima:', error);
        mostrarDatosClimaNoDisponibles();
    }
}

// Actualizar la interfaz con los datos del clima
function actualizarInterfazClima(data) {
    try {
        // Actualizar ubicación
        if (weatherElements.location.length) {
            weatherElements.location.text(`${data.location.name}, ${data.location.country}`);
        }

        // Actualizar icono del clima
        if (weatherElements.icon.length && data.current.condition.icon) {
            weatherElements.icon.attr('src', `https:${data.current.condition.icon}`);
            weatherElements.icon.attr('alt', data.current.condition.text);
            weatherElements.icon.show();
            // Ocultar el icono por defecto
            $('#weather-icon-default').hide();
        }

        // Actualizar temperatura
        if (weatherElements.temperature.length) {
            weatherElements.temperature.text(`${Math.round(data.current.temp_c)}°C`);
        }

        // Actualizar condición del clima
        if (weatherElements.condition.length) {
            weatherElements.condition.text(traducirCondicionClima(data.current.condition.text));
        }

        // Actualizar sensación térmica
        if (weatherElements.feelsLike.length) {
            weatherElements.feelsLike.text(`${Math.round(data.current.feelslike_c)}°C`);
        }

        // Actualizar humedad
        if (weatherElements.humidity.length) {
            weatherElements.humidity.text(`${data.current.humidity}%`);
        }

        // Actualizar velocidad del viento
        if (weatherElements.windSpeed.length) {
            weatherElements.windSpeed.text(`${data.current.wind_kph} km/h`);
        }

        // Actualizar dirección del viento
        if (weatherElements.windDirection.length) {
            weatherElements.windDirection.text(traducirDireccionViento(data.current.wind_dir));
        }

        // Actualizar visibilidad
        if (weatherElements.visibility.length) {
            weatherElements.visibility.text(`${data.current.vis_km} km`);
        }

        // Limpiar mensaje de error
        if (weatherElements.errorMessage.length) {
            weatherElements.errorMessage.text('').hide();
        }

        // Actualizar el mensaje contextual según el clima
        actualizarMensajeContextual(data.current);

    } catch (error) {
        console.error('Error al actualizar la interfaz del clima:', error);
        mostrarDatosClimaNoDisponibles();
    }
}

// Mostrar datos no disponibles en caso de error
function mostrarDatosClimaNoDisponibles() {
    if (weatherElements.location.length) {
        weatherElements.location.text(defaultLocation);
    }

    if (weatherElements.temperature.length) {
        weatherElements.temperature.text('--');
    }

    if (weatherElements.condition.length) {
        weatherElements.condition.text('Perfecto para pasear mascotas');
    }

    if (weatherElements.feelsLike.length) {
        weatherElements.feelsLike.text('--');
    }

    if (weatherElements.humidity.length) {
        weatherElements.humidity.text('--');
    }

    if (weatherElements.windSpeed.length) {
        weatherElements.windSpeed.text('--');
    }

    if (weatherElements.windDirection.length) {
        weatherElements.windDirection.text('--');
    }

    if (weatherElements.visibility.length) {
        weatherElements.visibility.text('--');
    }

    if (weatherElements.icon.length) {
        weatherElements.icon.hide();
    }

    // Mostrar el icono por defecto
    $('#weather-icon-default').show();

    if (weatherElements.errorMessage.length) {
        weatherElements.errorMessage.text('Datos del clima no disponibles').show();
    }
}

// Actualizar mensaje contextual según las condiciones climáticas
function actualizarMensajeContextual(currentWeather) {
    const temp = currentWeather.temp_c;
    const conditionCode = currentWeather.condition.code;
    let mensaje = 'Perfecto para pasear mascotas';

    // Condiciones de temperatura
    if (temp < 10) {
        mensaje = 'Hace frío, abriga a tus mascotas';
    } else if (temp > 30) {
        mensaje = 'Hace calor, mantén hidratadas a tus mascotas';
    } else if (temp >= 18 && temp <= 25) {
        mensaje = 'Clima ideal para actividades al aire libre';
    }

    // Condiciones de clima (lluvia, nieve, etc.)
    // Códigos de WeatherAPI: https://www.weatherapi.com/docs/weather_conditions.json
    const rainyConditions = [1063, 1180, 1183, 1186, 1189, 1192, 1195, 1240, 1243, 1246];
    const snowConditions = [1066, 1210, 1213, 1216, 1219, 1222, 1225, 1255, 1258];
    const stormConditions = [1087, 1273, 1276, 1279, 1282];

    if (rainyConditions.includes(conditionCode)) {
        mensaje = 'Está lloviendo, cuidado al salir con tus mascotas';
    } else if (snowConditions.includes(conditionCode)) {
        mensaje = 'Nevando, protege a tus mascotas del frío';
    } else if (stormConditions.includes(conditionCode)) {
        mensaje = 'Tormenta eléctrica, mantén a tus mascotas dentro';
    }

    if (weatherElements.condition.length) {
        weatherElements.condition.text(mensaje);
    }
}

// Traducir condición del clima a español
function traducirCondicionClima(condition) {
    const traducciones = {
        'Sunny': 'Soleado',
        'Clear': 'Despejado',
        'Partly cloudy': 'Parcialmente nublado',
        'Cloudy': 'Nublado',
        'Overcast': 'Cubierto',
        'Mist': 'Neblina',
        'Patchy rain possible': 'Posible lluvia dispersa',
        'Patchy snow possible': 'Posible nieve dispersa',
        'Patchy sleet possible': 'Posible aguanieve dispersa',
        'Patchy freezing drizzle possible': 'Posible llovizna congelada',
        'Thundery outbreaks possible': 'Posibles tormentas',
        'Blowing snow': 'Nieve con viento',
        'Blizzard': 'Ventisca',
        'Fog': 'Niebla',
        'Freezing fog': 'Niebla congelada',
        'Patchy light drizzle': 'Llovizna ligera dispersa',
        'Light drizzle': 'Llovizna ligera',
        'Freezing drizzle': 'Llovizna congelada',
        'Heavy freezing drizzle': 'Llovizna congelada intensa',
        'Patchy light rain': 'Lluvia ligera dispersa',
        'Light rain': 'Lluvia ligera',
        'Moderate rain at times': 'Lluvia moderada a intervalos',
        'Moderate rain': 'Lluvia moderada',
        'Heavy rain at times': 'Lluvia intensa a intervalos',
        'Heavy rain': 'Lluvia intensa',
        'Light freezing rain': 'Lluvia congelada ligera',
        'Moderate or heavy freezing rain': 'Lluvia congelada moderada o intensa',
        'Light sleet': 'Aguanieve ligera',
        'Moderate or heavy sleet': 'Aguanieve moderada o intensa',
        'Patchy light snow': 'Nieve ligera dispersa',
        'Light snow': 'Nieve ligera',
        'Patchy moderate snow': 'Nieve moderada dispersa',
        'Moderate snow': 'Nieve moderada',
        'Patchy heavy snow': 'Nieve intensa dispersa',
        'Heavy snow': 'Nieve intensa',
        'Ice pellets': 'Granizo',
        'Light rain shower': 'Lluvia ligera intermitente',
        'Moderate or heavy rain shower': 'Lluvia moderada o intensa intermitente',
        'Torrential rain shower': 'Lluvia torrencial',
        'Light sleet showers': 'Aguanieve ligera intermitente',
        'Moderate or heavy sleet showers': 'Aguanieve moderada o intensa',
        'Light snow showers': 'Nevada ligera intermitente',
        'Moderate or heavy snow showers': 'Nevada moderada o intensa',
        'Light showers of ice pellets': 'Granizo ligero intermitente',
        'Moderate or heavy showers of ice pellets': 'Granizo moderado o intenso',
        'Patchy light rain with thunder': 'Lluvia ligera dispersa con truenos',
        'Moderate or heavy rain with thunder': 'Lluvia moderada o intensa con truenos',
        'Patchy light snow with thunder': 'Nieve ligera dispersa con truenos',
        'Moderate or heavy snow with thunder': 'Nieve moderada o intensa con truenos'
    };

    return traducciones[condition] || condition;
}

// Traducir dirección del viento a español
function traducirDireccionViento(direction) {
    const traducciones = {
        'N': 'Norte',
        'NNE': 'Norte-Noreste',
        'NE': 'Noreste',
        'ENE': 'Este-Noreste',
        'E': 'Este',
        'ESE': 'Este-Sureste',
        'SE': 'Sureste',
        'SSE': 'Sur-Sureste',
        'S': 'Sur',
        'SSW': 'Sur-Suroeste',
        'SW': 'Suroeste',
        'WSW': 'Oeste-Suroeste',
        'W': 'Oeste',
        'WNW': 'Oeste-Noroeste',
        'NW': 'Noroeste',
        'NNW': 'Norte-Noroeste'
    };

    return traducciones[direction] || direction;
}

// Actualizar clima cada 30 minutos
setInterval(obtenerDatosClima, 30 * 60 * 1000);
