import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SearchIngredientsComponent } from './search-ingredients/search-ingredients.component';
import { AddIngredientModalComponent } from './add-ingredient-modal/add-ingredient-modal.component';
import { MyIngredientsComponent } from './my-ingredients/my-ingredients.component';


@NgModule({
  declarations: [
    SearchIngredientsComponent,
    AddIngredientModalComponent,
    MyIngredientsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgbModule
  ],
  providers: [
    NgbActiveModal
  ],
  exports: [
    SearchIngredientsComponent,
    MyIngredientsComponent
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PantryModule {

}
