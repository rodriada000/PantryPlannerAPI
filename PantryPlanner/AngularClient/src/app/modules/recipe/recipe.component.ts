import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'recipe-root',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.css']
})
export class RecipeComponent implements OnInit {

  public isMyRecipesPageSelected: boolean;
  public isExplorePageSelected: boolean;
  public isCreateRecipeSelected: boolean;
  public showSideMenu: boolean;

  constructor() { }

  ngOnInit(): void {
    this.showSideMenu = true;
    this.switchToMyRecipes();
  }

  switchToMyRecipes(): void {
    this.isMyRecipesPageSelected = true;
    this.isExplorePageSelected = false;
    this.isCreateRecipeSelected = false;
  }

  switchToExploreRecipes(): void {
    this.isMyRecipesPageSelected = false;
    this.isExplorePageSelected = true;
    this.isCreateRecipeSelected = false;
  }

  switchToCreateRecipe(): void {
    this.isMyRecipesPageSelected = false;
    this.isExplorePageSelected = false;
    this.isCreateRecipeSelected = true;
  }


}
