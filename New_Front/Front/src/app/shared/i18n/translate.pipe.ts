import { Pipe, PipeTransform, inject } from '@angular/core';
import { LanguageService } from './language.service';

@Pipe({ name: 'translate', pure: false })
export class TranslatePipe implements PipeTransform {
  private readonly language = inject(LanguageService);
  transform(
    key: string | number | null | undefined,
    params?: Record<string, string | number>,
  ): string {
    return key === null || key === undefined ? '' : this.language.text(String(key), params);
  }
}
