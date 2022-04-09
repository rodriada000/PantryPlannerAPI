import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RecipeComponent } from './recipe.component';
import { CreateRecipeComponent } from './create-recipe/create-recipe.component';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';



@NgModule({
  declarations: [RecipeComponent, CreateRecipeComponent],
  imports: [
    CommonModule,
    FormsModule,
    NgbModule,
    NgSelectModule
  ],
  exports: [RecipeComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class RecipeModule { }
