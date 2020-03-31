import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import IngredientApi from '../../data/services/ingredientApi.service';
import { ActiveKitchenService } from '../../shared/services/active-kitchen.service';
import { SearchIngredientsComponent } from './search-ingredients/search-ingredients.component';


@NgModule({
  declarations: [
    SearchIngredientsComponent
  ],
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    SearchIngredientsComponent
  ]
})
export class PantryModule {

}
