import { NgModule, Injector, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { createCustomElement } from '@angular/elements';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { ToastService } from './shared/services/toast.service';

import { AppComponent } from './app.component';
import { KitchenNavComponent } from './shared/components/kitchenNav/kitchenNav.component';

import { PantryModule } from './modules/pantry/pantry.module';
import { SearchIngredientsComponent } from './modules/pantry/search-ingredients/search-ingredients.component';
import { MyIngredientsComponent } from './modules/pantry/my-ingredients/my-ingredients.component';
import { ToastContainerComponent } from './shared/components/toast-container/toast-container.component';

@NgModule({
  declarations: [
    AppComponent,
    KitchenNavComponent,
    ToastContainerComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    PantryModule
  ],
  providers: [
    ToastService
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: []
})
export class AppModule {
  constructor(private injector: Injector) { }

  ngDoBootstrap() {
    const navElement = createCustomElement(KitchenNavComponent, { injector: this.injector });
    customElements.define('kitchen-nav', navElement);

    const toastElement = createCustomElement(ToastContainerComponent, { injector: this.injector });
    customElements.define('app-toast', toastElement);

    const searchElement = createCustomElement(SearchIngredientsComponent, { injector: this.injector });
    customElements.define('pantry-search-ingredients', searchElement);

    const myIngredElement = createCustomElement(MyIngredientsComponent, { injector: this.injector });
    customElements.define('pantry-my-ingredients', myIngredElement);
  }
}

