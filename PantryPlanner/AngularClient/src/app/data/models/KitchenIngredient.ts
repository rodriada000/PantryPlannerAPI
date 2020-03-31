import Ingredient from './Ingredient';
import Category from './Category';

export default class KitchenIngredient {
  kitchenIngredientId: number;
  ingredientId: number;
  kitchenId: number;
  addedByKitchenUserId: number;
  categoryId: number;
  lastUpdated: Date;
  quantity: number;
  note: string;
  ingredient: Ingredient;
  category: Category;
}

