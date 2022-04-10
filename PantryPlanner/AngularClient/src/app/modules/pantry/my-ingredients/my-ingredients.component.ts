import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { Subscription } from 'rxjs';
import { ToastService } from '../../../shared/services/toast.service';
import { PantryPageService } from '../pantry-page.service';

@Component({
  selector: 'pantry-my-ingredients',
  templateUrl: './my-ingredients.component.html',
  styleUrls: ['./my-ingredients.component.css']
})
export class MyIngredientsComponent implements OnInit, OnDestroy {

  public isFilterVisible: boolean;

  public isLoading: boolean;
  public isSaving: boolean;
  public hoveredIndex: number;
  public myIngredients: Array<KitchenIngredient> = [];
  public filteredIngredients: Array<KitchenIngredient> = [];
  public isEditing: boolean;
  public selectedSort: string = "";
  public selectedSortOrder: number = 1;
  public filterText: string = "";

  private kitchenId: Subscription;
  private itemAddedSub: Subscription;
  private pageSelection: Subscription;
  private origIngredient: KitchenIngredient;

  constructor(public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService, public toasts: ToastService, private cdRef:ChangeDetectorRef, private pageService: PantryPageService) { }
  
  ngOnInit(): void {
    this.isLoading = false;
    this.hoveredIndex = -1;
    this.isFilterVisible = false;
    this.filterText = "";

    // refresh list of ingredients in pantry after user changes kitchen
    this.kitchenId = this.activeKitchen.observableKitchenId.subscribe(id => {
      if (id === 0) {
        this.myIngredients = [];
      } else {
        this.refreshIngredients();
      }
    });

    this.pageSelection = this.pageService.observableSelectedPages.subscribe(p => {
      if (p !== null && p !== undefined) {
        this.filterText = "";
        this.doFilter();
        this.isFilterVisible = this.pageService.isSearchPantryPageSelected;
      }
    })

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.apiService.observableAddedIngredient.subscribe(newIngredient => {
      if (newIngredient !== null && newIngredient !== undefined) {
        this.myIngredients.push(newIngredient);
        this.sortBy(this.selectedSort, false);
        this.doFilter();
      }
    });
  }


  refreshIngredients(): void {
    this.isLoading = true;

    this.apiService.getIngredientsForKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.myIngredients = data;
      this.doFilter();
      this.sortBy('name')
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  sortBy(field: string, toggleSort: boolean = true): void {
    field = field.toLowerCase();

    if (toggleSort) {
      if (this.selectedSort.toLowerCase() === field) {
        // toggle sort asc/desc when clicking same field already sorted by
        this.selectedSortOrder *= -1;
      } else {
        this.selectedSortOrder = 1; // set back to A->Z
      }
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

  doFilter() {
    if (this.myIngredients === null || this.myIngredients === undefined) {
      this.filteredIngredients = [];
      return;
    }

    this.filteredIngredients = this.myIngredients.filter(p => this.filterText === "" || (p.ingredient.name.toLowerCase().includes(this.filterText.toLowerCase())));

    if (this.hoveredIndex >= this.filteredIngredients.length) {
      this.hoveredIndex = -1;
    }
  }

  toggleSortOrder() {
    this.sortBy(this.selectedSort);
  }

  setSelected(index: number, $event) {
    if (this.hoveredIndex != index) {
      if (this.isEditing) {
        this.cancelEdit(this.myIngredients[this.hoveredIndex], $event);
        this.isEditing = false;
      }
      this.hoveredIndex = index;
    }
  }

  unselect(index: number, $event) {
    if (index == this.hoveredIndex) {
      this.hoveredIndex = -1;
      $event.preventDefault();
      $event.stopPropagation();
    }
  }

  removeFromKitchen(ingredient: KitchenIngredient, index: number, $event): void {
    if (this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.apiService.removeKitchenIngredient(ingredient).subscribe(data => {
      const i:number = this.myIngredients.findIndex(i => i.kitchenIngredientId == ingredient.kitchenIngredientId);
      this.myIngredients.splice(i, 1);
      this.filteredIngredients.splice(index, 1);
      this.toasts.showSuccess("Removed " + data.ingredient.name);
      this.isSaving = false;

    },
      error => {
        this.toasts.showDanger("Could not remove - " + error.error);
        this.isSaving = false;
      });

    $event.preventDefault();
    $event.stopPropagation();
  }

  editIngredient(ingredient: KitchenIngredient, $event): void {
    this.isEditing = true;
    this.origIngredient = { ...ingredient };
    $event.preventDefault();
    $event.stopPropagation();
  }

  quickEditQty(ingredient: KitchenIngredient, qtyToAdd: number) {
    if (this.isSaving) {
      return;
    }

    if (ingredient.quantity + qtyToAdd <= 0) {
      return; // cant have 0 or negative qty
    }

    ingredient.quantity += qtyToAdd;
    this.saveEdit(ingredient, false);
  }

  saveEdit(ingredient: KitchenIngredient, showToast: boolean = true): void {
    if (this.isSaving) {
      return;
    }
    
    if (ingredient.quantity <= 0) {
      return; // cant have 0 or negative qty
    }

    this.isSaving = true;

    this.apiService.updateKitchenIngredient(ingredient).subscribe(data => {
      if (showToast) {
        this.toasts.showSuccess("Updated " + ingredient.ingredient.name);
      }
      this.isEditing = false;
      this.isSaving = false;
    },
      error => {
        this.toasts.showDanger("Could not update - " + error.error);
        this.isEditing = false;
        this.isSaving = false;
      });
  }

  cancelEdit(ingredient: KitchenIngredient, $event): void {
    ingredient.note = this.origIngredient.note;
    ingredient.quantity = this.origIngredient.quantity;
    this.origIngredient = null;
    this.isEditing = false;
    $event.preventDefault();
    $event.stopPropagation();
  }

  addToGroceryList(ingredient: KitchenIngredient, $event): void {
    $event.preventDefault();
    $event.stopPropagation();
  }


  ngOnDestroy(): void {
    this.kitchenId?.unsubscribe();
    this.itemAddedSub?.unsubscribe();
    this.pageSelection?.unsubscribe();
  }

}
