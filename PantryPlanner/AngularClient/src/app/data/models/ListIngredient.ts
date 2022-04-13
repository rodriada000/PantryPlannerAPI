import Category from "./Category";
import Ingredient from "./Ingredient";

export default class ListIngredient {
  id: number;
  kitchenListId: number;
  kitchenId: number;
  ingredientId: number;
  quantity?: number;
  sortOrder: number;
  isChecked: boolean;
  note: string;
  categoryId?: number;
  ingredient: Ingredient;
  category: Category;
}

