import { ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideServiceWorker } from '@angular/service-worker';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { ConnectionServiceOptions, ConnectionServiceOptionsToken } from 'ngx-connection-service';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es-MX';

// Registrar el locale español
registerLocaleData(localeEs);

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideHttpClient(withInterceptorsFromDi()), 
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    provideServiceWorker('ngsw-worker.js', 
      {
        enabled: !isDevMode(),
        registrationStrategy: 'registerWhenStable:30000'
      }),
      {
        provide: ConnectionServiceOptionsToken,
        useValue: {
          heartbeatUrl: 'assets/ping.json', // URL que verifica el estado de conexión.
          heartbeatInterval: 30000, // Intervalo en milisegundos. Aquí lo configuramos a 20 segundos.
          heartbeatRetryInterval: 20000,
          requestMethod: 'get', // Método de la petición HTTP para el heartbeat.
        } as ConnectionServiceOptions
      } 
      ,
    provideAnimationsAsync(),
    { provide: LOCALE_ID, useValue: 'es-MX' } 
  ]
};
