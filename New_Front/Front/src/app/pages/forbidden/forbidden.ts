import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '../../shared/i18n/translate.pipe';

@Component({
  selector: 'app-forbidden',
  imports: [RouterLink, TranslatePipe],
  templateUrl: './forbidden.html',
  styleUrl: './forbidden.scss',
})
export class ForbiddenPage {}
