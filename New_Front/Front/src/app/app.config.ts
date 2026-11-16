import { ApplicationConfig, inject, provideAppInitializer } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { LanguageService } from './shared/i18n/language.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    provideAppInitializer(() => inject(LanguageService).initialize()),
    MessageService,

    provideAnimationsAsync(),

    providePrimeNG({
      theme: {
        preset: Aura,
        options: {
          darkModeSelector: false
        }
      },
      license: 'eyJpZCI6IjIxZjZlNjlhLTJmZjMtNDMxOS1hZWUxLTA4ZjNkMTUwODFkOCIsInByb2R1Y3QiOiJwcmltZXVpIiwidGllciI6ImNvbW11bml0eSIsInR5cGUiOiJkZXYiLCJpYXQiOjE3OTM4ODA4NjgsImV4cCI6MTgyNTQxNjg2OH0.f7ZTOZmp4644-DwMnXyai8qMfu4C9bM_tS8KrImxdXOnROKLgcTKwETBu1dZ2RfCrJ37-FdaODJpWxpGOKXKCQ'
    })
  ]
};
