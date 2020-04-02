import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Ingredient from '../../../data/models/Ingredient';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { ToastService } from '../../../shared/services/toast.service';

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

  constructor(
    public activeModal: NgbActiveModal,
    public apiService: KitchenIngredientApi,
    public activeKitchen: ActiveKitchenService,
    public toastService: ToastService) { }

  ngOnInit() {
    console.log(this.ingredient);
    this.quantity = 1;
    this.notes = "";
  }

  confirmAdd(): void {

    const toAdd: KitchenIngredient = this.apiService.createEmpty(this.ingredient, this.activeKitchen.getActiveKitchenId());
    toAdd.note = this.notes;
    toAdd.quantity = this.quantity;

    if (toAdd.kitchenId === 0) {
      this.toastService.showDanger("Cannot add to kitchen - kitchen id is 0");
      return;
    }

    this.apiService.addIngredientToKitchen(toAdd).subscribe(data => {
      this.apiService.setAddedIngredient(data);
      this.activeModal.close(data);
      this.toastService.showSuccess("Added " + this.ingredient.name);
    },
      resp => {
        this.toastService.showDanger(resp.error);
        this.apiService.setAddedIngredient(null);
        this.activeModal.close(null);
      },
    );

  }

  close(): void {
    this.activeModal.close(null);
  }

}
