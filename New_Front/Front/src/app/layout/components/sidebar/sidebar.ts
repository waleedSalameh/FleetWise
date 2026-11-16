import { TranslatePipe } from '../../../shared/i18n/translate.pipe';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [TranslatePipe, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {

  // القوائم الرئيسية
  openMenu: string | null = null;

  // القوائم الفرعية
  openSubMenu: string | null = null;

  // فتح وإغلاق القوائم الرئيسية
  toggleMenu(menu: string) {
    this.openMenu = this.openMenu === menu ? null : menu;

    // إغلاق القائمة الفرعية عند تغيير القائمة الرئيسية
    this.openSubMenu = null;
  }

  // فتح وإغلاق القوائم الفرعية
  toggleSubMenu(menu: string) {
    this.openSubMenu = this.openSubMenu === menu ? null : menu;
  }
}
