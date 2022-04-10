import Ingredient from "./Ingredient";

export default class ListIngredient {
  id: number;
  kitchenListId: number;
  kitchenId: number;
  ingredientId: number;
  quantity?: number;
  sortOrder: number;
  isChecked: boolean;
  ingredient: Ingredient;
}

