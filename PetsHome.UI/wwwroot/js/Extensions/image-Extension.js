// ============================================
// IMAGE UPLOAD - Mejoras con Drag & Drop
// ============================================

$(document).ready(function () {
    initializeImageUpload();
});

// Inicializar funcionalidad de carga de imagen
function initializeImageUpload() {
    const uploadZone = document.getElementById('uploadZone');
    const fileInput = document.getElementById('file');
    const preview = document.getElementById('img');
    const previewContainer = document.getElementById('previewContainer');
    const btnRemoveImage = document.getElementById('btnRemoveImage');

    // Verificar si los elementos existen
    if (!uploadZone || !fileInput) return;

    // Eventos de drag & drop
    uploadZone.addEventListener('dragover', handleDragOver);
    uploadZone.addEventListener('dragleave', handleDragLeave);
    uploadZone.addEventListener('drop', handleDrop);

    // Evento de clic en la zona de carga
    uploadZone.addEventListener('click', function (e) {
        if (e.target !== fileInput) {
            fileInput.click();
        }
    });

    // Botón eliminar imagen
    if (btnRemoveImage) {
        btnRemoveImage.addEventListener('click', function () {
            removeImage();
        });
    }

    // Verificar si ya hay una imagen precargada
    if (preview && preview.src && preview.src !== '' && preview.src !== window.location.href && !preview.src.includes('undefined')) {
        // Imagen precargada desde el servidor: ocultar upload, mostrar preview
        uploadZone.classList.add('hide');
        if (previewContainer) previewContainer.classList.remove('hide');
        if (btnRemoveImage) btnRemoveImage.style.display = 'flex';
    }
}

// Manejar evento dragover
function handleDragOver(e) {
    e.preventDefault();
    e.stopPropagation();
    this.classList.add('dragover');
}

// Manejar evento dragleave
function handleDragLeave(e) {
    e.preventDefault();
    e.stopPropagation();
    this.classList.remove('dragover');
}

// Manejar evento drop
function handleDrop(e) {
    e.preventDefault();
    e.stopPropagation();
    this.classList.remove('dragover');

    const files = e.dataTransfer.files;
    if (files.length > 0) {
        const fileInput = document.getElementById('file');
        fileInput.files = files;
        previewFile();
    }
}

// Función mejorada de vista previa
function previewFile() {
    // Extensiones permitidas
    const extensionesPermitidas = /(.png|.jpg|.jpeg|.gif)$/i;
    const preview = document.getElementById('img');
    const file = document.getElementById('file').files[0];
    const archivoRuta = document.getElementById('file').value;
    const uploadZone = document.getElementById('uploadZone');

    // Validación de extensión
    if (!extensionesPermitidas.exec(archivoRuta)) {
        showNotification('Extensión del archivo no válida. Use JPG, PNG o GIF.', 'error');
        document.getElementById('file').value = '';
        uploadZone.classList.add('error');
        setTimeout(() => uploadZone.classList.remove('error'), 2000);
        return;
    }

    // Validación de tamaño (2MB)
    if (!validarImagen(file, 2048)) {
        showNotification('La imagen ha superado el tamaño límite de 2 MB.', 'error');
        document.getElementById('file').value = '';
        uploadZone.classList.add('error');
        setTimeout(() => uploadZone.classList.remove('error'), 2000);
        return;
    }

    // Leer archivo
    const reader = new FileReader();

    reader.onloadstart = function () {
        uploadZone.classList.add('uploading');
    };

    reader.onloadend = function () {
        uploadZone.classList.remove('uploading');

        if (preview) {
            const previewContainer = document.getElementById('previewContainer');
            const btnRemoveImage = document.getElementById('btnRemoveImage');

            uploadZone.classList.add('hide');
            if (previewContainer) previewContainer.classList.remove('hide');

            preview.src = reader.result;

            setTimeout(() => {
                preview.classList.add('show');
                if (btnRemoveImage) btnRemoveImage.style.display = 'flex';
            }, 50);
        }

        uploadZone.classList.add('success');
        setTimeout(() => uploadZone.classList.remove('success'), 1500);
    };

    reader.onerror = function () {
        uploadZone.classList.remove('uploading');
        showNotification('Error al cargar la imagen. Intente nuevamente.', 'error');
        uploadZone.classList.add('error');
        setTimeout(() => uploadZone.classList.remove('error'), 2000);
    };

    if (file) {
        reader.readAsDataURL(file);
    } else {
        preview.src = "";
        hidePreview();
    }
}

// Mostrar vista previa
function showPreview(imageSrc) {
    const preview = document.getElementById('img');
    if (preview) {
        preview.src = imageSrc;
    }
}

// Ocultar vista previa
function hidePreview() {
    const preview = document.getElementById('img');
    const uploadZone = document.getElementById('uploadZone');
    const previewContainer = document.getElementById('previewContainer');
    const btnRemoveImage = document.getElementById('btnRemoveImage');

    if (preview) {
        preview.classList.remove('show');
        previewContainer.classList.add('hide');
        uploadZone.classList.remove('hide');

        setTimeout(() => {
            preview.src = '';
            preview.removeAttribute('src');
            if (btnRemoveImage) btnRemoveImage.style.display = 'none';
        }, 300);
    }
}

// Eliminar imagen
function removeImage() {
    const fileInput = document.getElementById('file');
    const uploadZone = document.getElementById('uploadZone');

    if (fileInput) {
        fileInput.value = '';
    }

    if (uploadZone) {
        uploadZone.classList.remove('has-file', 'success');
    }

    hidePreview();
    showNotification('Imagen eliminada correctamente.', 'info');
}

/**
 * Valida si la imagen cumple con el tamaño especificado en Kilobytes
 * @param { File } file
 * @param { int } maxSize - Tamaño máximo en KB
 */
function validarImagen(file, maxSize) {
    if (!file) return false;

    const fileSize = file.size;
    const sizekiloByte = parseInt(fileSize / 1024);

    return sizekiloByte <= maxSize;
}

/**
 * Mostrar notificación (compatible con toastr si está disponible)
 * @param { string } message
 * @param { string } type - 'success', 'error', 'info', 'warning'
 */
function showNotification(message, type) {
    if (typeof toastr !== 'undefined') {
        toastr[type](message);
    } else {
        alert(message);
    }
}