import { DOCUMENT } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { PrimeNG } from 'primeng/config';
import { firstValueFrom } from 'rxjs';

export type Language = 'en' | 'ar';
type Dictionary = Record<string, string>;

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly http = inject(HttpClient);
  private readonly document = inject(DOCUMENT);
  private readonly prime = inject(PrimeNG);
  private readonly defaultPrime = {
    ...this.prime.translation,
    aria: { ...this.prime.translation.aria },
  };
  private readonly dictionaries: Partial<Record<Language, Dictionary>> = {};
  private readonly englishKeys = new Map<string, string>();
  readonly language = signal<Language>('en');

  async initialize(): Promise<void> {
    const saved = this.readPreference();
    try {
      const [en, ar] = await Promise.allSettled([
        firstValueFrom(this.http.get<Dictionary>('/i18n/en.json')),
        firstValueFrom(this.http.get<Dictionary>('/i18n/ar.json')),
      ]);
      if (en.status === 'fulfilled') {
        this.dictionaries.en = en.value;
        for (const [key, value] of Object.entries(en.value))
          if (!this.englishKeys.has(value)) this.englishKeys.set(value, key);
      }
      if (ar.status === 'fulfilled') this.dictionaries.ar = ar.value;
    } catch {
      // Keep the original English UI available if the translation assets fail to load.
    }
    this.use(saved === 'ar' && !this.dictionaries.ar ? 'en' : saved);
  }

  use(language: Language): void {
    this.language.set(language);
    this.document.documentElement.lang = language;
    this.document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
    try {
      localStorage.setItem('fleetwise.language', language);
    } catch {
      /* Storage may be unavailable. */
    }
    this.prime.setTranslation(
      language === 'ar'
        ? {
            ...this.defaultPrime,
            ...this.arabicPrime,
            aria: { ...this.defaultPrime.aria, ...this.arabicPrime.aria },
          }
        : this.defaultPrime,
    );
  }

  text(key: string, params?: Record<string, string | number>): string {
    this.language();
    const resolvedKey =
      this.dictionaries.en?.[key] !== undefined ? key : (this.englishKeys.get(key) ?? key);
    const source =
      this.dictionaries[this.language()]?.[resolvedKey] ??
      this.dictionaries.en?.[resolvedKey] ??
      key;
    return source.replace(/\{\{(\w+)\}\}/g, (_, name: string) =>
      String(params?.[name] ?? `{{${name}}}`),
    );
  }

  knownText(value: string): string {
    if (this.language() === 'ar') {
      const oil =
        /^Are you sure you want to delete "(.+)"\? Oils with stock movements cannot be deleted\.$/.exec(
          value,
        );
      if (oil) return `هل أنت متأكد من حذف "${oil[1]}"؟ لا يمكن حذف الزيوت التي لها حركات مخزون.`;
      const part =
        /^Are you sure you want to delete "(.+)"\? Spare parts with stock movements cannot be deleted\.$/.exec(
          value,
        );
      if (part)
        return `هل أنت متأكد من حذف "${part[1]}"؟ لا يمكن حذف قطع الغيار التي لها حركات مخزون.`;
      const driver = /^Are you sure you want to delete (.+)\?$/.exec(value);
      if (driver) return `هل أنت متأكد من حذف ${driver[1]}؟`;
    }
    return this.text(value);
  }

  private readPreference(): Language {
    try {
      return localStorage.getItem('fleetwise.language') === 'ar' ? 'ar' : 'en';
    } catch {
      return 'en';
    }
  }

  private readonly englishPrime = {
    accept: 'Yes',
    reject: 'No',
    choose: 'Choose',
    upload: 'Upload',
    cancel: 'Cancel',
    today: 'Today',
    clear: 'Clear',
    weekHeader: 'Wk',
    dayNames: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],
    dayNamesShort: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    dayNamesMin: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
    monthNames: [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December',
    ],
    monthNamesShort: [
      'Jan',
      'Feb',
      'Mar',
      'Apr',
      'May',
      'Jun',
      'Jul',
      'Aug',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ],
    dateFormat: 'mm/dd/yy',
    firstDayOfWeek: 0,
    emptyMessage: 'No results found',
    emptyFilterMessage: 'No results found',
  };
  private readonly arabicPrime = {
    accept: 'نعم',
    reject: 'لا',
    choose: 'اختيار',
    upload: 'رفع',
    cancel: 'إلغاء',
    startsWith: 'يبدأ بـ',
    contains: 'يحتوي على',
    notContains: 'لا يحتوي على',
    endsWith: 'ينتهي بـ',
    equals: 'يساوي',
    notEquals: 'لا يساوي',
    noFilter: 'بلا تصفية',
    clear: 'مسح',
    apply: 'تطبيق',
    matchAll: 'مطابقة الكل',
    matchAny: 'مطابقة أي',
    addRule: 'إضافة قاعدة',
    removeRule: 'إزالة قاعدة',
    before: 'قبل',
    after: 'بعد',
    dateIs: 'التاريخ يساوي',
    dateIsNot: 'التاريخ لا يساوي',
    dateBefore: 'التاريخ قبل',
    dateAfter: 'التاريخ بعد',
    today: 'اليوم',
    weekHeader: 'أسب',
    dayNames: ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'],
    dayNamesShort: ['أحد', 'إثن', 'ثلا', 'أرب', 'خمي', 'جمع', 'سبت'],
    dayNamesMin: ['ح', 'ن', 'ث', 'ر', 'خ', 'ج', 'س'],
    monthNames: [
      'كانون الثاني',
      'شباط',
      'آذار',
      'نيسان',
      'أيار',
      'حزيران',
      'تموز',
      'آب',
      'أيلول',
      'تشرين الأول',
      'تشرين الثاني',
      'كانون الأول',
    ],
    monthNamesShort: [
      'ك2',
      'شباط',
      'آذار',
      'نيسان',
      'أيار',
      'حزيران',
      'تموز',
      'آب',
      'أيلول',
      'ت1',
      'ت2',
      'ك1',
    ],
    dateFormat: 'dd/mm/yy',
    firstDayOfWeek: 6,
    emptyMessage: 'لا توجد نتائج',
    emptyFilterMessage: 'لا توجد نتائج',
    emptySearchMessage: 'لا توجد نتائج',
    searchMessage: 'نتائج البحث متاحة',
    selectionMessage: 'تم اختيار {0} عناصر',
    emptySelectionMessage: 'لم يُحدد أي عنصر',
    fileChosenMessage: 'ملفات',
    noFileChosenMessage: 'لم يُحدد ملف',
    chooseYear: 'اختر السنة',
    chooseMonth: 'اختر الشهر',
    chooseDate: 'اختر التاريخ',
    prevDecade: 'العقد السابق',
    nextDecade: 'العقد التالي',
    prevYear: 'السنة السابقة',
    nextYear: 'السنة التالية',
    prevMonth: 'الشهر السابق',
    nextMonth: 'الشهر التالي',
    prevHour: 'الساعة السابقة',
    nextHour: 'الساعة التالية',
    prevMinute: 'الدقيقة السابقة',
    nextMinute: 'الدقيقة التالية',
    prevSecond: 'الثانية السابقة',
    nextSecond: 'الثانية التالية',
    am: 'ص',
    pm: 'م',
    weak: 'ضعيفة',
    medium: 'متوسطة',
    strong: 'قوية',
    passwordPrompt: 'أدخل كلمة المرور',
    aria: {
      trueLabel: 'نعم',
      falseLabel: 'لا',
      nullLabel: 'غير محدد',
      star: 'نجمة واحدة',
      stars: '{star} نجوم',
      close: 'إغلاق',
      previous: 'السابق',
      next: 'التالي',
      navigation: 'التنقل',
      scrollTop: 'العودة إلى الأعلى',
      firstPageLabel: 'الصفحة الأولى',
      lastPageLabel: 'الصفحة الأخيرة',
      nextPageLabel: 'الصفحة التالية',
      prevPageLabel: 'الصفحة السابقة',
      rowsPerPageLabel: 'صفوف لكل صفحة',
      selectAll: 'تحديد الكل',
      unselectAll: 'إلغاء تحديد الكل',
      filter: 'تصفية',
      showFilterMenu: 'إظهار قائمة التصفية',
      hideFilterMenu: 'إخفاء قائمة التصفية',
      editRow: 'تعديل الصف',
      saveEdit: 'حفظ التعديل',
      cancelEdit: 'إلغاء التعديل',
      filterOperator: 'عامل التصفية',
      filterConstraint: 'شرط التصفية',
      moveTop: 'نقل إلى الأعلى',
      moveUp: 'نقل للأعلى',
      moveDown: 'نقل للأسفل',
      moveBottom: 'نقل إلى الأسفل',
      moveToTarget: 'نقل إلى الهدف',
      moveToSource: 'نقل إلى المصدر',
      moveAllToTarget: 'نقل الكل إلى الهدف',
      moveAllToSource: 'نقل الكل إلى المصدر',
      selectRow: 'تحديد الصف',
      unselectRow: 'إلغاء تحديد الصف',
      expandRow: 'توسيع الصف',
      collapseRow: 'طي الصف',
      expand: 'توسيع',
      collapse: 'طي',
      previousPageLabel: 'الصفحة السابقة',
      jumpToPageDropdownLabel: 'الانتقال إلى صفحة من القائمة',
      jumpToPageInputLabel: 'الانتقال إلى رقم صفحة',
      listView: 'عرض القائمة',
      gridView: 'عرض الشبكة',
      zoomIn: 'تكبير',
      zoomOut: 'تصغير',
      browseFiles: 'استعراض الملفات',
      maximizeLabel: 'تكبير النافذة',
      minimizeLabel: 'تصغير النافذة',
    },
  };
}
