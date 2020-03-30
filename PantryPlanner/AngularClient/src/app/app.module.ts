import { NgModule, Injector } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { createCustomElement } from '@angular/elements';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import ApiService from './shared/services/api.service';
import { KitchenNavComponent } from './shared/components/kitchenNav/kitchenNav.component';

@NgModule({
  declarations: [
    AppComponent,
    KitchenNavComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbModule
  ],
  providers: [ApiService],
  entryComponents: [KitchenNavComponent]
})
export class AppModule {
  constructor(private injector: Injector) { }

  ngDoBootstrap() {
    const AppElement = createCustomElement(KitchenNavComponent, { injector: this.injector });
    customElements.define('kitchen-nav', AppElement);
  }
}

//platformBrowserDynamic().bootstrapModule(AppModule);
