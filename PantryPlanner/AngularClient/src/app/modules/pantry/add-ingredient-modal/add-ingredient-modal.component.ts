import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import Ingredient from '../../../data/models/Ingredient';

@Component({
  selector: 'pantry-add-ingredient-modal',
  templateUrl: './add-ingredient-modal.component.html',
  styleUrls: ['./add-ingredient-modal.component.css']
})
export class AddIngredientModalComponent {

  closeResult: string;

  @Input() ingredient: Ingredient;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
    console.log(this.ingredient);
  }

  close(): void {
    this.activeModal.close(this.ingredient);
  }

}
