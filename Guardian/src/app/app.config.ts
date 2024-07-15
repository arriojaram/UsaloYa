import { ApplicationConfig, isDevMode } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideServiceWorker } from '@angular/service-worker';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { LoadingInterceptor } from './interceptors/loading.interceptor';

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
    provideAnimationsAsync()
  ]
};
