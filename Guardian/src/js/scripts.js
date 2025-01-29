/*!
    * Start Bootstrap - SB Admin v7.0.7 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2023 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
    // 
// Scripts
// 

window.addEventListener('DOMContentLoaded', event => {

    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (sidebarToggle) {
        // Esta función verifica si la pantalla es de un tamaño pequeño
        const isMobile = checkIfMobile();
    
        // Si es móvil, esconde la barra lateral al cargar la página
        if (isMobile) {
            document.body.classList.remove('sb-sidenav-toggled');
        } 
    
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }
    
     // Función para determinar si la navegación es desde un dispositivo móvil
     function checkIfMobile() {
        if (navigator.userAgent.match(/iPhone/i)   || navigator.userAgent.match(/iPad/i)  || navigator.userAgent.match(/Android/i)) 
            return true;
        return false;
    }
});
