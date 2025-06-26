// Función para inicializar tooltips de Bootstrap
function initTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Función para manejar el evento de impresión
function setupPrintButton() {
    document.getElementById('printBtn')?.addEventListener('click', function () {
        window.print();
    });
}

// Función para inicializar la aplicación
function initApp() {
    initTooltips();
    setupPrintButton();

    // Otras inicializaciones pueden ir aquí
}

// Ejecutar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', initApp);