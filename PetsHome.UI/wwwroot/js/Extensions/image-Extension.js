
function previewFile() {
    //extensiones de las imagenes
    var extensionesPermitidas = /(.png|.jpg|.jpge)$/i;
    //se pasa el 'img' a una variable
    var preview = document.getElementById('img');
    //se para el archivo seleccionado a una variable
    var file = document.getElementById('file').files[0];
    //se obtiene la direccion de la imagen
    var archivoRuta = document.getElementById('file').value;

    //validacion de la extension de la imagen
    if (!extensionesPermitidas.exec(archivoRuta)) {
        alert('Extension del archivo no valida');
        document.getElementById('file').value = '';
        return;
    }
    if (!validarImagen(file, 2048)) {
        alert("La imagen ha superado el tamaño limite.");
        return;
    }

    //El FileReader permite leer los ficheros almacenados en el cliente
    var reader = new FileReader();

    //onloadend: es un EventHandler que representa el código cuando el progreso se ha detenido en la carga de un recurso.
    reader.onloadend = function () {
        //se accede al src del <img> y se agrega la direccion
        preview.src = reader.result;
    }

    if (file) {
        //es usado para leer el contenido del especificado File
        reader.readAsDataURL(file);
    } else {
        preview.src = "";
    }


}

/**
 * Valida si la imagen cumple con el tamaño especificado en Kilobytes
 * @param { File } file
 * @param { int } maxSize
 */
function validarImagen(file, maxSize) {
    var fileSize = file.size;
    var sizekiloByte = parseInt(fileSize / 1024);
    if (sizekiloByte > maxSize) {
        return false;
    }
    return true;
}