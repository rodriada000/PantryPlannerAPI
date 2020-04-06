import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipeComponent } from './recipe.component';



@NgModule({
  declarations: [RecipeComponent],
  imports: [
    CommonModule
  ],
  exports: [RecipeComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class RecipeModule { }
