import { Injectable, TemplateRef } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  toasts: any[] = [];

  show(textOrTpl: string | TemplateRef<any>, options: any = {}) {
    this.toasts.push({ textOrTpl, ...options });
  }

  remove(toast) {
    this.toasts = this.toasts.filter(t => t !== toast);
  }

  showStandard(msg: string) {
    this.show(msg, { classname: 'bg-info text-light', delay: 5000 });
  }

  showSuccess(msg: string) {
    this.show(msg, { classname: 'bg-success text-light', delay: 8000 });
  }

  showDanger(dangerTxtOrTpl: any) {
    this.show(dangerTxtOrTpl, { classname: 'bg-danger text-light', delay: 10000 });
  }
}
