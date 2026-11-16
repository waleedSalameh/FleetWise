import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component, inject } from '@angular/core';
import { Language, LanguageService } from '../../../shared/i18n/language.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './topbar.html',
  styleUrl: './topbar.scss',
})
export class Topbar {
  readonly language = inject(LanguageService);
  change(value: string): void {
    if (value === 'en' || value === 'ar') this.language.use(value as Language);
  }
}
