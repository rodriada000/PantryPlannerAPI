import { Component, OnInit } from '@angular/core';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';

@Component({
  selector: 'pantry-my-ingredients',
  templateUrl: './my-ingredients.component.html',
  styleUrls: ['./my-ingredients.component.css']
})
export class MyIngredientsComponent implements OnInit {

  myIngredients: Array<KitchenIngredient>;

  constructor(public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService) { }

  ngOnInit(): void {

    this.apiService.getIngredientsForKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.myIngredients = data;
    });
  }

}
