import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SearchIngredientsComponent } from './search-ingredients/search-ingredients.component';
import { AddIngredientModalComponent } from './add-ingredient-modal/add-ingredient-modal.component';
import { NgbModule, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';


@NgModule({
  declarations: [
    SearchIngredientsComponent,
    AddIngredientModalComponent
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
    SearchIngredientsComponent
  ]
})
export class PantryModule {

}
