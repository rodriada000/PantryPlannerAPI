import { Component, OnInit, OnDestroy } from '@angular/core';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { Subscription } from 'rxjs';
import { isNullOrUndefined } from 'util';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'pantry-my-ingredients',
  templateUrl: './my-ingredients.component.html',
  styleUrls: ['./my-ingredients.component.css']
})
export class MyIngredientsComponent implements OnInit, OnDestroy {

  public isLoading: boolean;
  public hoveredIndex: number;
  public myIngredients: Array<KitchenIngredient>;
  public isEditing: boolean;
  public selectedSort: string = "";
  public selectedSortOrder: number = 1;




  private kitchenId: Subscription;
  private itemAddedSub: Subscription;
  private origIngredient: KitchenIngredient;

  constructor(public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService, public toasts: ToastService) { }

  ngOnInit(): void {
    this.isLoading = false;
    this.hoveredIndex = -1;

    // refresh list of ingredients in pantry after user changes kitchen
    this.kitchenId = this.activeKitchen.observableKitchenId.subscribe(id => {
      if (id === 0) {
        this.myIngredients = [];
      } else {
        this.refreshIngredients();
      }
    });

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.apiService.observableAddedIngredient.subscribe(newIngredient => {
      if (!isNullOrUndefined(newIngredient)) {
        this.myIngredients.push(newIngredient);
      }
    });
  }

  refreshIngredients(): void {
    this.isLoading = true;

    this.apiService.getIngredientsForKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.myIngredients = data;
      this.sortBy('name')
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  sortBy(field: string): void {
    field = field.toLowerCase();
    if (this.selectedSort.toLowerCase() === field) {
      // toggle sort asc/desc when clicking same field already sorted by
      this.selectedSortOrder *= -1;
    } else {
      this.selectedSortOrder = 1; // set back to A->Z
    }

    this.myIngredients = this.myIngredients.sort((a, b) => {
      let valA: string = a.ingredient.name;
      let valB: string = b.ingredient.name;
      this.selectedSort = "Name";

      if (field == 'category') {
        this.selectedSort = "Category";
        valA = a.ingredient.categoryName;
        valB = a.ingredient.categoryName;
      }

      if (valA.toLowerCase() > valB.toLowerCase()) {
        return 1 * this.selectedSortOrder;
      } else if (valA.toLowerCase() < valB.toLowerCase()) {
        return -1 * this.selectedSortOrder;
      }

      if (field == 'category') {
        if (a.ingredient.name.toLowerCase() > b.ingredient.name.toLowerCase()) {
          return 1 * this.selectedSortOrder;
        } else if (a.ingredient.name.toLowerCase() < b.ingredient.name.toLowerCase()) {
          return -1 * this.selectedSortOrder;
        }
      }

      return 0;
    });
  }

  toggleSortOrder() {
    this.sortBy(this.selectedSort);
  }

  setSelected(index: number) {
    if (this.hoveredIndex != index && this.isEditing) {
      this.cancelEdit(this.myIngredients[this.hoveredIndex]);
      this.isEditing = false;
    }

    this.hoveredIndex = index;
  }

  removeFromKitchen(ingredient: KitchenIngredient, index: number): void {
    this.apiService.removeKitchenIngredient(ingredient).subscribe(data => {
      this.myIngredients.splice(index, 1);
      this.toasts.showSuccess("Removed " + data.ingredient.name);
    },
      error => { this.toasts.showDanger("Could not remove - " + error.error); });
  }

  editIngredient(ingredient: KitchenIngredient): void {
    this.isEditing = true;
    this.origIngredient = { ...ingredient };
  }

  saveEdit(ingredient: KitchenIngredient): void {
    this.apiService.updateKitchenIngredient(ingredient).subscribe(data => {
      this.toasts.showSuccess("Updated " + ingredient.ingredient.name);
      this.isEditing = false;
    },
      error => {
        this.toasts.showDanger("Could not update - " + error.error);
        this.isEditing = false;
      });
  }

  cancelEdit(ingredient: KitchenIngredient): void {
    ingredient.note = this.origIngredient.note;
    ingredient.quantity = this.origIngredient.quantity;
    this.origIngredient = null;
    this.isEditing = false;
  }


  ngOnDestroy(): void {
    this.kitchenId.unsubscribe();
    this.itemAddedSub.unsubscribe();
  }

}
