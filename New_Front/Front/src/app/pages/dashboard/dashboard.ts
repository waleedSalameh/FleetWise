import { TranslatePipe } from '../../shared/i18n/translate.pipe';
import { Component } from '@angular/core';

@Component({
  imports: [TranslatePipe, ],
  selector: 'app-dashboard',
  styleUrl: './dashboard.scss',
  templateUrl: './dashboard.html',
})
export class Dashboard {}
