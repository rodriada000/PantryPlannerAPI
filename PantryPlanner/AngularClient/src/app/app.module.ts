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
import { PantryComponent } from './modules/pantry/pantry.component';
import { ManageUsersComponent } from './modules/pantry/manage-users/manage-users.component';


import { RecipeModule } from './modules/recipe/recipe.module';
import { RecipeComponent } from './modules/recipe/recipe.component';
import { GroceryListComponent } from './modules/grocery-list/grocery-list.component';
import { ManageListComponent } from './modules/grocery-list/manage-list/manage-list.component';

@NgModule({
  declarations: [
    AppComponent,
    KitchenNavComponent,
    ToastContainerComponent,
    GroceryListComponent,
    ManageListComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    NgbModule,
    PantryModule,
    RecipeModule
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


    const pantryElem = createCustomElement(PantryComponent, { injector: this.injector });
    customElements.define('pantry-root', pantryElem);

    const searchElement = createCustomElement(SearchIngredientsComponent, { injector: this.injector });
    customElements.define('pantry-search-ingredients', searchElement);

    const myIngredElement = createCustomElement(MyIngredientsComponent, { injector: this.injector });
    customElements.define('pantry-my-ingredients', myIngredElement);

    const manageUElement = createCustomElement(ManageUsersComponent, { injector: this.injector });
    customElements.define('pantry-manage-users', manageUElement);


    const recipesElem = createCustomElement(RecipeComponent, { injector: this.injector });
    customElements.define('recipe-root', recipesElem);

    const groceryListElem = createCustomElement(GroceryListComponent, { injector: this.injector });
    customElements.define('app-grocery-list', groceryListElem);
  }
}

