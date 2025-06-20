import { ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideServiceWorker } from '@angular/service-worker';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { ConnectionServiceOptions, ConnectionServiceOptionsToken } from 'ngx-connection-service';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es-MX';
import { provideToastr } from 'ngx-toastr';
import { environment } from './environments/enviroment';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { importProvidersFrom } from '@angular/core';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

// Registrar el locale español
registerLocaleData(localeEs);

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

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
    provideAnimations(),
    { provide: LOCALE_ID, useValue: 'es-MX' } ,
    provideToastr({
      timeOut: environment.notificationsDisplayTimeSeconds,
      positionClass: 'toast-bottom-center',
      preventDuplicates: true,
      closeButton: true
    }), provideAnimationsAsync(),
    
        importProvidersFrom(
      TranslateModule.forRoot({
        defaultLanguage: 'es',
        loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
        }
      })
    )
  ]
};
