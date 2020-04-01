import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Ingredient from '../../../data/models/Ingredient';
import IngredientApi from '../../../data/services/ingredientApi.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';

@Component({
  selector: 'pantry-add-ingredient-modal',
  templateUrl: './add-ingredient-modal.component.html',
  styleUrls: ['./add-ingredient-modal.component.css']
})
export class AddIngredientModalComponent {

  closeResult: string;

  public quantity: number;
  public notes: string;

  @Input() ingredient: Ingredient;

  constructor(public activeModal: NgbActiveModal, public apiService: KitchenIngredientApi, public activeKitchen: ActiveKitchenService) { }

  ngOnInit() {
    console.log(this.ingredient);
    this.quantity = 1;
    this.notes = "";
  }

  confirmAdd(): void {

    const toAdd: KitchenIngredient = new KitchenIngredient();
    toAdd.ingredientId = this.ingredient.ingredientId;
    toAdd.categoryId = this.ingredient.categoryId;
    toAdd.kitchenId = this.activeKitchen.getActiveKitchenId();
    toAdd.note = this.notes;
    toAdd.quantity = this.quantity;

    if (toAdd.kitchenId === 0) {
      // TODO: show error dialog
      console.error("kitchen id is 0");
      return;
    }

    console.log("adding ingredient ...");

    this.apiService.addIngredientToKitchen(toAdd).subscribe(data => {
      this.apiService.setAddedIngredient(data);
      this.activeModal.close(data);
    },
      error => {
        console.error(error);
        this.apiService.setAddedIngredient(null);
        this.activeModal.close(null);
      },
    );

  }

  close(): void {
    this.activeModal.close(null);
  }

}
