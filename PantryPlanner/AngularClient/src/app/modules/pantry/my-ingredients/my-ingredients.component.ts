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

  private kitchenId: Subscription;
  private itemAddedSub: Subscription;

  constructor(public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService, public toasts: ToastService) { }

  ngOnInit(): void {
    this.isLoading = false;
    this.hoveredIndex = -1;

    // refresh list of ingredients in pantry after user changes kitchen
    this.kitchenId = this.activeKitchen.observableKitchenId.subscribe(id => {
      this.refreshIngredients();
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
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  removeFromKitchen(ingredient: KitchenIngredient, index: number): void {
    this.apiService.removeKitchenIngredient(ingredient).subscribe(data => {
      this.myIngredients.splice(index, 1);
      this.toasts.showSuccess("Removed " + data.ingredient.name);
    },
      error => { this.toasts.showDanger("Could not remove - " + error.error); });
  }


  ngOnDestroy(): void {
    this.kitchenId.unsubscribe();
    this.itemAddedSub.unsubscribe();
  }

}
