import { Component, OnInit, OnDestroy } from '@angular/core';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { Subscription } from 'rxjs';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'pantry-my-ingredients',
  templateUrl: './my-ingredients.component.html',
  styleUrls: ['./my-ingredients.component.css']
})
export class MyIngredientsComponent implements OnInit, OnDestroy {
 
  public myIngredients: Array<KitchenIngredient>;
  private kitchenId: Subscription;
  private itemAddedSub: Subscription;

  constructor(public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService) { }

  ngOnInit(): void {
    // refresh list of ingredients in pantry after user changes kitchen
    this.kitchenId = this.activeKitchen.observableKitchenId.subscribe(id => { this.refreshIngredients(); });

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.apiService.observableAddedIngredient.subscribe(newIngredient => {
      if (!isNullOrUndefined(newIngredient)) {
        this.myIngredients.push(newIngredient);
      }
    });
  }

  refreshIngredients(): void {
    this.apiService.getIngredientsForKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.myIngredients = data;
    });
  }

  ngOnDestroy(): void {
    this.kitchenId.unsubscribe();
    this.itemAddedSub.unsubscribe();
  }

}
