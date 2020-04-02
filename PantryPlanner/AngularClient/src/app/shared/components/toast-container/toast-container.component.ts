import { Component, TemplateRef } from '@angular/core';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  templateUrl: './toast-container.component.html',
  host: { '[class.ngb-toasts]': 'true' }
})
export class ToastContainerComponent {
  constructor(public toastService: ToastService) { }

  isTemplate(toast) {
    return toast.textOrTpl instanceof TemplateRef;
  }
}

