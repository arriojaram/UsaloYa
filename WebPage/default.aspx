<!DOCTYPE html>
<html lang="es">

<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>UsaloYa - Herramientas de Software Innovadoras</title>
  
  <!-- Favicon -->
  <link rel="icon" type="image/png" href="Images/Logo_200_194.png">
  <link rel="shortcut icon" type="image/png" href="Images/Logo_200_194.png">
  <link rel="apple-touch-icon" sizes="180x180" href="Images/Logo_200_194.png">
  <link rel="icon" type="image/png" sizes="32x32" href="Images/Logo_200_194.png">
  <link rel="icon" type="image/png" sizes="16x16" href="Images/Logo_200_194.png">
  <meta name="msapplication-TileImage" content="Images/Logo_200_194.png">
  <meta name="theme-color" content="#667eea">
  
  <!-- Bootstrap CSS -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet">
  <!-- Google Fonts -->
  <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600;700;800&display=swap"
    rel="stylesheet">
  <!-- Custom Styles -->
  <link href="styles.css" rel="stylesheet">
</head>

<body>

  <!-- Navbar -->
  <nav class="navbar navbar-expand-lg fixed-top">
    <div class="container">
      <a class="navbar-brand d-flex align-items-center" href="#">
        <img src="Images/Logo_200_194.jpg" alt="UsaloYa Logo" class="navbar-logo d-block d-md-none"
          style="height: 40px; width: auto;">
        <img src="Images/Logo_661_640.png" alt="UsaloYa Logo" class="navbar-logo d-none d-md-block"
          style="height: 50px; width: auto;">
        <span class="fw-bold ms-2">UsaloYa</span>
      </a>
      <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#navMenu">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navMenu">
        <ul class="navbar-nav ms-auto">
          <li class="nav-item"><a class="nav-link" href="#about">Sobre Nosotros</a></li>
          <li class="nav-item"><a class="nav-link" href="#features">Guardian POS</a></li>
          <li class="nav-item"><a class="nav-link" href="#projects">Casos de Uso</a></li>
          <li class="nav-item"><a class="nav-link" href="#contact">Contacto</a></li>
        </ul>
      </div>
    </div>
  </nav>

  <!-- Hero -->
  <header class="hero text-center text-white">
    <div class="container hero-content">
      <div class="animate-fadeInUp">
        <h1 class="mt-2 mb-2">Transformamos tu Negocio</h1>
        <h1 class="mb-4">al Instante</h1>
        <p class="lead mb-5">Te proveemos herramientas de software innovadoras que impulsan el crecimiento de tu
          negocio, te permiten organizar de manera efectiva tus ventas y control de inventarios y que adem&aacute;s
          puedes
          comenzar a utilizar &#161;Ya&#33;</p>
        <div class="d-flex flex-column flex-md-row gap-3 justify-content-center align-items-center">
          <a href="#guardian" class="btn-hero animate-pulse">
            <i class="bi bi-rocket-takeoff me-2"></i>Descubre Guardian POS
          </a>
          <a href="#contact" class="btn-hero">
            <i class="bi bi-chat-dots me-2"></i>Habla con Nosotros
          </a>
        </div>
      </div>

      <!-- Floating Elements -->
      <div class="hero-floating-icon top-left animate-float">
        <i class="bi bi-gear"></i>
      </div>
      <div class="hero-floating-icon top-right animate-float">
        <i class="bi bi-graph-up"></i>
      </div>
      <div class="hero-floating-icon bottom-left animate-float">
        <i class="bi bi-shield-check"></i>
      </div>
    </div>
  </header>

  <main>
    <!-- About -->
    <section id="about" class="section bg-gradient-light text-center">
      <div class="container">
        <div class="animate-fadeInUp">
          <h2 class="section-title">&iquest;Por qu&eacute; elegir UsaloYa?</h2>
          <div class="row justify-content-center">
            <div class="col-lg-8">
              <p class="lead mb-4">Te ofrecemos las mejores herramientas de software que puedes usar al instante para
                comenzar a ordenar tus ventas, sin rollos ni subscripciones raras, incluso funcionan cuando no hay
                internet ni datos.
              </p>
              <p>Actual&iacute;zate ahora mismo y comienza a usar nuestro sistema punto de venta Guardian de manera
                completamente gratutia</p>
            </div>
          </div>

          <div class="row g-4">
            <div class="col-lg-4 col-md-6">
              <div class="card h-100 p-4"
                style="background: linear-gradient(135deg, rgba(16, 185, 129, 0.05) 0%, rgba(5, 150, 105, 0.05) 100%); border: 2px solid rgba(16, 185, 129, 0.1); transition: all 0.3s ease;">
                <div class="text-center mb-4">
                  <div class="card-icon success mb-3">
                    <i class="bi bi-shop"></i>
                  </div>
                  <h5 class="fw-bold text-gradient">Tienditas</h5>
                </div>
                <p class="card-text mb-4">
                  Perfecto para hacer cuentas r&aacute;pidas, realizar ventas y generar reportes. Sistema intuitivo y
                  confiable para modernizar operaciones.
                </p>
                <div class="mt-auto">
                  <div class="d-flex align-items-center mb-2">
                    <i class="bi bi-check-circle-fill text-success me-2"></i>
                    <small class="fw-medium">Cobros r&aacute;pidos</small>
                  </div>
                  <div class="d-flex align-items-center mb-2">
                    <i class="bi bi-check-circle-fill text-success me-2"></i>
                    <small class="fw-medium">Control de inventarios</small>
                  </div>
                  <div class="d-flex align-items-center">
                    <i class="bi bi-check-circle-fill text-success me-2"></i>
                    <small class="fw-medium">F&aacute;cil implementaci&oacute;n</small>
                  </div>

                </div>
              </div>
            </div>
            <div class="col-lg-4 col-md-6">
              <div class="card h-100 p-4"
                style="background: linear-gradient(135deg, rgba(102, 126, 234, 0.05) 0%, rgba(118, 75, 162, 0.05) 100%); border: 2px solid rgba(102, 126, 234, 0.1); transition: all 0.3s ease;">
                <div class="text-center mb-4">
                  <div class="card-icon primary mb-3">
                    <i class="bi bi-building"></i>
                  </div>
                  <h5 class="fw-bold text-gradient">Peque&ntilde;as Empresas</h5>
                </div>
                <p class="card-text mb-4">
                  Sistema completo y sencillo para peque&ntilde;as empresas, con control de inventario, gesti&oacute;n
                  de ventas, clientes y reportes financieros b&aacute;sicos.
                </p>
                <div class="mt-auto">
                  <div class="text-start mb-2">
                    <i class="bi bi-check-circle-fill text-primary me-2"></i>
                    <small class="fw-medium">Gesti&oacute;n integral de ventas y clientes</small>
                  </div>
                  <div class="text-start mb-2">
                    <i class="bi bi-check-circle-fill text-primary me-2"></i>
                    <small class="fw-medium">Reportes financieros</small>
                  </div>
                  <div class="text-start">
                    <i class="bi bi-check-circle-fill text-primary me-2"></i>
                    <small class="fw-medium">Reportes de ventas detallados</small>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-lg-4 col-md-6">
              <div class="card h-100 p-4"
                style="background: linear-gradient(135deg, rgba(245, 158, 11, 0.05) 0%, rgba(217, 119, 6, 0.05) 100%); border: 2px solid rgba(245, 158, 11, 0.1); transition: all 0.3s ease;">
                <div class="text-center mb-4">
                  <div class="card-icon warning mb-3">
                    <i class="bi bi-briefcase"></i>
                  </div>
                  <h5 class="fw-bold text-gradient">Asesoria con Sentido</h5>
                </div>
                <p class="card-text mb-4">
                  Asesoramiento para negocios que quieren transformar e impulsar sus operaciones con un sistema de
                  inventario y ventas.
                </p>
                <div class="mt-auto">
                  <div class="d-flex align-items-center mb-2">
                    <i class="bi bi-check-circle-fill text-warning me-2"></i>
                    <small class="fw-medium">Asesoramiento personalizado</small>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="mt-5">
            <h3 class="text-gradient fw-bold">&iexcl;Actual&iacute;zate y &uacute;salo ya!</h3>
            <!-- Strategic CTA Button #1 -->
            <div class="text-center mt-4">
              <a href="https://g.usaloya.xyz/forms-navigator/register" target="_blank" rel="noopener noreferrer"
                class="btn-cta"
                style="background: var(--usalo-gradient-primary); border-color: var(--usalo-primary); font-size: 1rem; padding: 0.8rem 2rem;">
                <i class="bi bi-rocket-takeoff me-2"></i>Prueba Guardian Gratis
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Features -->
    <section id="features" class="section">
      <div class="container">
        <div class="text-center mb-5">
          <h2 class="section-title">Caracter&iacute;sticas de Guardian POS</h2>
          <p class="lead">El sistema de punto de venta gratuito m&aacute;s f&aacute;cil de usar del mercado.</p>
          <p class="lead">Completamente a tu dispoci&oacute;n para iniciar la transformaci&oacute;n de tu negocio,
            organizar tus ventas, manejar tus inventario y tomar el control de tu negocio.</p>
        </div>

        <!-- Main Features - Optimized Layout for Key Features -->
        <div class="row g-5 mb-5">
          <!-- Featured Hero Card -->
          <div class="col-12">
            <div class="row justify-content-center">
              <div class="col-lg-10 col-md-11">
                <div class="card border-0 shadow-lg feature-hero-card">
                  <div class="card-body text-center p-5">
                    <div class="card-icon primary mx-auto mb-4" style="width: 100px; height: 100px; font-size: 2.5rem;">
                      <i class="bi bi-code-slash"></i>
                    </div>
                    <h3 class="fw-bold mb-4">Funcionalidad Sin L&iacute;mites</h3>
                    <p class="lead mb-4">Funciona en cualquier dispositivo: computadora, laptop celular o tablet.</p>
                    <div class="row g-3 mt-4">
                      <div class="col-md-4">
                        <div class="d-flex justify-content-center">
                          <i class="bi bi-check-circle-fill text-success me-2"></i>
                          <small class="fw-medium">Sin instalaci&oacute;n previa</small>
                        </div>
                      </div>
                      <div class="col-md-4">
                        <div class="d-flex justify-content-center">
                          <i class="bi bi-check-circle-fill text-success me-2"></i>
                          <small class="fw-medium">Interfaz sencilla</small>
                        </div>
                      </div>
                      <div class="col-md-4">
                        <div class="d-flex justify-content-center">
                          <i class="bi bi-check-circle-fill text-success me-2"></i>
                          <small class="fw-medium">Sin problemas de acceso</small>
                        </div>
                      </div>
                    </div>
                    <div class="mt-4">
                     
                      <span class="badge-gradient-primary px-4 py-2" style="font-size: 0.9rem;">
                        <i class="bi bi-stars me-2"></i>
                        Caracter&iacute;sticas Estrella
                        <i class="bi bi-stars ms-2"></i>
                      </span>
                      
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <!-- Hero Features - Aligned Layout -->
          <div id="features2" class="row g-4 my-5">
            <div class="col-lg-4 col-md-6">
              <div class="feature-highlight text-center animate-fadeInUp" style="animation-delay: 0.2s;">
                <div class="card-icon info mb-3 mx-auto">
                  <i class="bi bi-bricks"></i>
                </div>
                <h5 class="fw-bold mb-3">Multiplataforma</h5>
                <p class="mb-0">Vende desde cualquier dispositivo: computadora, tablet o smartphone y no pierdas
                  ning&uacute;n detalle de tus ventas.</p>
              </div>
            </div>

            <div class="col-lg-4 col-md-6">
              <div class="feature-highlight text-center animate-fadeInUp" style="animation-delay: 0.4s;">
                <div class="card-icon warning mb-3 mx-auto">
                  <i class="bi bi-cash-stack"></i>
                </div>
                <h5 class="fw-bold mb-3">Control Total</h5>
                <p class="mb-0">Controla inventario y ventas en tiempo real, deja de preocuparte por los detalles si no
                  hay internet.</p>
              </div>
            </div>

            <div class="col-lg-4 col-md-6">
              <div class="feature-highlight text-center animate-fadeInUp" style="animation-delay: 0.6s;">
                <div class="card-icon rose mb-3 mx-auto">
                  <i class="bi bi-award"></i>
                </div>
                <h5 class="fw-bold mb-3">Garant&iacute;a</h5>
                <p class="mb-1">
                  Lo mejor de lo mejor en sistemas de punto de venta que vas a encontrar. 
                </p>
                <p>
                  &iquest;Sigues pagando por sistemas caros, complejos y que necesitan internet para funcionar?
                </p>
              </div>
            </div>
          </div>
        </div>
        <!-- Strategic CTA Button #2 -->
        <div class="text-center mt-5 mb-4">
          <p class="lead mb-3">&iquest;Ya est&aacute;s listo para revolucionar tu negocio?</p>
          <a href="https://g.usaloya.xyz/forms-navigator/register" target="_blank" rel="noopener noreferrer"
            class="btn-cta"
            style="background: var(--usalo-gradient-success); border-color: var(--usalo-success); font-size: 1rem; padding: 0.8rem 2rem;">
            &#128640; Comenzar Ahora - Gratis
          </a>
        </div>
    </section>

    <!-- Guardian Solution -->
    <section id="guardian" class="section guardian-section">
      <div class="container">


        <!-- Main Content -->
        <div class="text-center mb-5">
          <h3 class="text-gradient fw-bold mb-4" style="font-size: 2.2rem;">Transforma tu Negocio con Guardian</h3>
          <div class="row justify-content-center">
            <div class="col-lg-8">
              <p class="lead">Guardian es un sistema de Punto de Venta moderno y sencillo de usar, dise&ntilde;ado para
                adaptarse a tu necesidades y las de tu negocio. Funciona desde cualquier dispositivo en cualquier parte, sin depender de una
                conexi&oacute;n constante a Internet.</p>
            </div>
          </div>
        </div>

        <!-- Key Features -->
        <div class="row g-4 mb-5">
          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <img src="images/product-stock.png" class="img-fluid mb-3 cursor-pointer" alt="Control de Inventario"
                  style="max-height: 200px; cursor: pointer;" data-bs-toggle="modal" data-bs-target="#imageModal1">
                <h5 class="card-title">Tu Inventario Controlado</h5>
                <p class="card-text">Registra compras e ingresos, gestiona tu inventario y
                  almacenes, administra todas tus sucursales.</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <img src="images/sales-report.png" class="img-fluid mb-3 cursor-pointer" alt="Reportes de Ventas"
                  style="max-height: 200px; cursor: pointer;" data-bs-toggle="modal" data-bs-target="#imageModal2">
                <h5 class="card-title">Reportes en Tiempo Real</h5>
                <p class="card-text">Accede a an&aacute;lisis completos de tus ventas y tendencias para tomar decisiones
                  informadas en cualquier momento.</p>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="card h-100 border-0 shadow-sm">
              <div class="card-body text-center p-4">
                <img src="images/main-menu.png" class="img-fluid mb-3 cursor-pointer" alt="Interfaz Intuitiva"
                  style="max-height: 200px; cursor: pointer;" data-bs-toggle="modal" data-bs-target="#imageModal3">
                <h5 class="card-title">Interfaz Intuitiva</h5>
                <p class="card-text">Dise&ntilde;o moderno y f&aacute;cil de usar que permite una r&aacute;pida
                  adaptaci&oacute;n de tu equipo sin necesidad de capacitaci&oacute;n intensa.</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Image Preview Modals -->
        <div class="modal fade" id="imageModal1" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content bg-dark">
              <div class="modal-header border-0">
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"
                  aria-label="Close"></button>
              </div>
              <div class="modal-body text-center p-0">
                <img src="images/product-stock.png" class="img-fluid" alt="Control de Inventario">
              </div>
            </div>
          </div>
        </div>

        <div class="modal fade" id="imageModal2" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content bg-dark">
              <div class="modal-header border-0">
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"
                  aria-label="Close"></button>
              </div>
              <div class="modal-body text-center p-0">
                <img src="images/sales-report.png" class="img-fluid" alt="Reportes de Ventas">
              </div>
            </div>
          </div>
        </div>

        <div class="modal fade" id="imageModal3" tabindex="-1" aria-hidden="true">
          <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content bg-dark">
              <div class="modal-header border-0">
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"
                  aria-label="Close"></button>
              </div>
              <div class="modal-body text-center p-0">
                <img src="images/main-menu.png" class="img-fluid" alt="Interfaz Intuitiva">
              </div>
            </div>
          </div>
        </div>


      </div>
      <div class="text-center mt-5">
        <a href="https://g.usaloya.xyz/forms-navigator/register" target="_blank" rel="noopener noreferrer"
          class="btn-cta animate-pulse"
          style="background: var(--usalo-gradient-success); border-color: var(--usalo-success); font-size: 1.2rem; padding: 1.2rem 3rem;">
          <i class="bi bi-rocket-takeoff me-2"></i> Probar Guardian AHORA - GRATIS
        </a>
        <div class="mt-3">
          <p>
            &#x274C; Sin compromisos</p>
          <p>
            &#x274C; Sin tarjeta de cr&eacute;dito</p>
          <p><i class="bi bi-whatsapp text-success"></i>
            <a class="ms-1" href="https://wa.me/5212225725212" target="_blank" rel="noopener noreferrer">
              Cont&aacute;ctanos si tienes dudas
            </a>
          </p>
        </div>
      </div>
    </section>

    <!-- Projects -->
    <section id="projects" class="section">
      <div class="container">
        <div class="text-center mb-5">
          <h2 class="section-title">Casos de Uso Exitosos</h2>
          <p class="lead">Guardian se adapta perfectamente a diferentes tipos de negocios</p>
        </div>
        <!-- Success Stories Carousel -->
        <div id="successStoriesCarousel" class="carousel slide" data-bs-ride="carousel" data-bs-interval="6000">
          <!-- Carousel Indicators -->
          <div class="carousel-indicators">
            <button type="button" data-bs-target="#successStoriesCarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
            <button type="button" data-bs-target="#successStoriesCarousel" data-bs-slide-to="1" aria-label="Slide 2"></button>
            <button type="button" data-bs-target="#successStoriesCarousel" data-bs-slide-to="2" aria-label="Slide 3"></button>
            <button type="button" data-bs-target="#successStoriesCarousel" data-bs-slide-to="3" aria-label="Slide 4"></button>
          </div>

          <!-- Carousel Items -->
          <div class="carousel-inner">
            <!-- Story 1: Don Tomas -->
            <div class="carousel-item active">
              <div class="row justify-content-center">
                <div class="col-lg-8 col-md-10">
                  <div class="success-story-content">
                    <div class="story-header mb-4">
                      <div class="story-image-circle mb-3">
                        <img src="images/Customer_Tomas.png" alt="Don Tomas - Ferretería ALDOS" class="customer-image">
                      </div>
                      <h4 class="story-title">Don Tomas - Ferreter&iacute;a &quot;ALDOS&quot;</h4>
                      <div class="story-rating mb-3">
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                      </div>
                    </div>
                    <blockquote class="story-quote">
                      Antes llevaba todo en una libreta&hellip; y siempre terminaba con m&aacute;s clavos que martillos y sin saber por qu&eacute;. Desde que uso &iexcl;Usalo Ya! s&eacute; exactamente cu&aacute;nto vendo y qu&eacute; necesito pedir. Hasta mis clientes notan que ahora los atiendo m&aacute;s r&aacute;pido. Lo mejor&hellip; &iexcl;es que no me cost&oacute; un dineral y lo aprend&iacute; a usar en un d&iacute;a!
                    </blockquote>
                    <div class="story-badge">
                      <span class="badge bg-success">
                        <i class="bi bi-check-circle me-1"></i>
                        Historia Verificada
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Story 2: Lucia -->
            <div class="carousel-item">
              <div class="row justify-content-center">
                <div class="col-lg-8 col-md-10">
                  <div class="success-story-content">
                    <div class="story-header mb-4">
                      <div class="story-image-circle mb-3">
                        <img src="images/Customer_SnMiguel.jpg" alt="Lucia - Tienda de Colchas y Cobertores San miguel" class="customer-image">
                      </div>
                      <h4 class="story-title">Lucia - Tienda de Colchas y Cobertores &quot;San miguel&quot;</h4>
                      <div class="story-rating mb-3">
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                      </div>
                    </div>
                    <blockquote class="story-quote">
                      Con tantos tipos de colchas y dise&ntilde;os, siempre me confund&iacute;a al dar precios y me costaba saber qu&eacute; modelos se vend&iacute;an m&aacute;s. &iexcl;Usalo Ya! me cambi&oacute; la jugada. Ahora con un par de clics s&eacute; qu&eacute; colores vuelan m&aacute;s y puedo planear mis compras. Mis clientes felices&hellip; y yo duermo tranquila.
                    </blockquote>
                    <div class="story-badge">
                      <span class="badge bg-success">
                        <i class="bi bi-check-circle me-1"></i>
                        Historia Verificada
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Story 3: Rutulo -->
            <div class="carousel-item">
              <div class="row justify-content-center">
                <div class="col-lg-8 col-md-10">
                  <div class="success-story-content">
                    <div class="story-header mb-4">
                      <div class="story-image-circle mb-3">
                        <img src="images/Customer_MiTiendita.jpg" alt="Rutulo - Abarrotes El semáforo: Parada obligatoria" class="customer-image">
                      </div>
                      <h4 class="story-title">Rutulo - Abarrotes &quot;El sem&aacute;foro: Parada obligatoria &quot;</h4>
                      <div class="story-rating mb-3">
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                      </div>
                    </div>
                    <blockquote class="story-quote">
                      Yo pens&eacute; que eso de los sistemas era para tiendas grandes. Pero cuando prob&eacute; &iexcl;Usalo Ya! me di cuenta que tambi&eacute;n era para m&iacute;. Ya no me falta refresco los fines de semana, no se me pasan las cuentas por cobrar y puedo ver todo desde el celular. Mis hijos dicen que ahora s&iacute; soy <strong>tienda moderna</strong>.
                    </blockquote>
                    <div class="story-badge">
                      <span class="badge bg-success">
                        <i class="bi bi-check-circle me-1"></i>
                        Historia Verificada
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Story 4: Martin -->
            <div class="carousel-item">
              <div class="row justify-content-center">
                <div class="col-lg-8 col-md-10">
                  <div class="success-story-content">
                    <div class="story-header mb-4">
                      <div class="story-image-circle mb-3">
                        <img src="images/Customer_Vaquero.png" alt="Martin - Ferretería El vaquero" class="customer-image">
                      </div>
                      <h4 class="story-title">Martin - Ferreter&iacute;a &quot;El vaquero&quot;</h4>
                      <div class="story-rating mb-3">
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                        <i class="bi bi-star-fill text-warning"></i>
                      </div>
                    </div>
                    <blockquote class="story-quote">
                      En temporada  la  ferre era un caos: filas largas, precios que olvidaba y ventas que no apuntaba. Desde que uso &iexcl;Usalo Ya! las ventas fluyen, los clientes no esperan y hasta me sobra tiempo para tomarme un caf&eacute;. &iexcl;Y eso no pasaba antes!
                    </blockquote>
                    <div class="story-badge">
                      <span class="badge bg-success">
                        <i class="bi bi-check-circle me-1"></i>
                        Historia Verificada
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Carousel Controls -->
          <button class="carousel-control-prev" type="button" data-bs-target="#successStoriesCarousel" data-bs-slide="prev">
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
            <span class="visually-hidden">Previous</span>
          </button>
          <button class="carousel-control-next" type="button" data-bs-target="#successStoriesCarousel" data-bs-slide="next">
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
            <span class="visually-hidden">Next</span>
          </button>
        </div>
        <!-- Strategic CTA Button #3 -->
        <div class="text-center mt-5">
          <div class="p-4"
            style="background: linear-gradient(135deg, rgba(102, 126, 234, 0.05) 0%, rgba(118, 75, 162, 0.05) 100%); border-radius: 20px; border: 2px solid rgba(102, 126, 234, 0.1);">
            <h5 class="fw-bold mb-3">&iquest;Tu negocio necesita esto?</h5>
            <p class="mb-3">&Uacute;nete a los negocios que ya est&aacute;n transformando sus operaciones</p>
            <a href="https://g.usaloya.xyz/forms-navigator/register" target="_blank" rel="noopener noreferrer"
              class="btn-cta"
              style="background: var(--usalo-gradient-warning); border-color: var(--usalo-warning); font-size: 1rem; padding: 0.8rem 2rem;">
              <i class="bi bi-star me-2"></i>Probar Sin Costo
            </a>
          </div>
        </div>
      </div>
    </section>

    <!-- Contact CTA -->
    <section id="contact" class="section cta-section text-white">
      <div class="container position-relative">
        <div class="row justify-content-center">
          <div class="col-lg-8 text-center">
            <div class="animate-fadeInUp">
              <h2 class="fw-bold mb-4" style="font-size: 2.5rem;">&iquest;Ya est&aacute;s listo para Transformar tu
                Negocio?</h2>
              <p class="lead mb-5" style="font-size: 1.3rem; opacity: 0.9;">&Uacute;nete a nosotros, y revoluciona tus
                operaciones con Guardian</p>

              <div class="row g-3 justify-content-center mb-5">
                <div class="col-md-4">
                  <div class="cta-feature-card text-center">
                    <i class="bi bi-clock cta-feature-icon"></i>
                    <h6 class="fw-bold mb-1">Implementaci&oacute;n R&aacute;pida</h6>
                    <small style="opacity: 0.8;">En menos de 1 hora</small>
                  </div>
                </div>
                <div class="col-md-4">
                  <div class="cta-feature-card text-center">
                    <i class="bi bi-headset cta-feature-icon"></i>
                    <h6 class="fw-bold mb-1">Soporte Continuo</h6>
                    <small style="opacity: 0.8;">Asistencia completa</small>
                  </div>
                </div>
                <div class="col-md-4">
                  <div class="cta-feature-card text-center">
                    <i class="bi bi-shield-check cta-feature-icon"></i>
                    <h6 class="fw-bold mb-1">100% Seguro</h6>
                    <small style="opacity: 0.8;">Datos protegidos</small>
                  </div>
                </div>
              </div>

              <div class="d-flex flex-column flex-md-row gap-3 justify-content-center align-items-center">
                <a href="mailto:soporte@usaloya.xyz" class="btn-cta email">
                  <i class="bi bi-envelope me-2"></i>Escr&iacute;benos por Email
                </a>
                <a href="https://wa.me/5212225725212" class="btn-cta whatsapp animate-pulse">
                  <i class="bi bi-whatsapp me-2"></i>WhatsApp Informaci&oacute;n
                </a>
                <a href="https://wa.me/5212225725212" class="btn-cta whatsapp">
                  <i class="bi bi-headset me-2"></i>WhatsApp Soporte
                </a>
              </div>

              <div class="mt-4">
                <p style="opacity: 0.8; font-size: 1.1rem;"><strong>UsaloYa es gratis</strong>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>

  <!-- Footer -->
  <footer class="text-center">
    <div class="container py-4">
      <div class="row align-items-center">
        <div class="col-md-4 text-md-start text-center mb-3 mb-md-0">
          <div class="d-flex align-items-center justify-content-center justify-content-md-start">
            <img src="Images/Logo_200_194.jpg" alt="UsaloYa Logo" class="footer-logo d-block d-md-none"
              style="height: 35px; width: auto; margin-right: 12px;">
            <img src="Images/Logo_661_640.jpg" alt="UsaloYa Logo" class="footer-logo d-none d-md-block"
              style="height: 40px; width: auto; margin-right: 12px;">
            <span class="fw-bold" style="font-size: 1.2rem;">UsaloYa</span>
          </div>
        </div>
        <div class="col-md-4 text-center mb-3 mb-md-0">
          <p class="mb-0" style="opacity: 0.8;">Transformando negocios desde 2020</p>
        </div>
        <div class="col-md-4 text-md-end text-center">
          <p class="mb-0" style="opacity: 0.8;">&copy; 2025 UsaloYa. Todos los derechos reservados.</p>
        </div>
      </div>
    </div>
  </footer>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
</body>

</html>