import { NgModule, Injector } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { createCustomElement } from '@angular/elements';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { KitchenNavComponent } from './shared/components/kitchenNav/kitchenNav.component';

import { PantryModule } from './modules/pantry/pantry.module';
import { SearchIngredientsComponent } from './modules/pantry/search-ingredients/search-ingredients.component';

@NgModule({
  declarations: [
    AppComponent,
    KitchenNavComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    PantryModule
  ],
  entryComponents: []
})
export class AppModule {
  constructor(private injector: Injector) { }

  ngDoBootstrap() {
    const navElement = createCustomElement(KitchenNavComponent, { injector: this.injector });
    customElements.define('kitchen-nav', navElement);

    const searchElement = createCustomElement(SearchIngredientsComponent, { injector: this.injector });
    customElements.define('pantry-search-ingredients', searchElement);
  }
}

