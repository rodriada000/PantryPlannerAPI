import RecipeIngredient from './RecipeIngredient';
import RecipeStep from './RecipeStep';

export default class Recipe {
  recipeId: number;
  createdByUserId: string;
  recipeUrl: string;
  name: string;
  description: string;
  prepTime: number;
  cookTime: number;
  servingSize: string;
  dateCreated: Date;
  isPublic: boolean;

  createdByUsername: string;
  ingredients: Array<RecipeIngredient>;
  steps: Array<RecipeStep>;

}
