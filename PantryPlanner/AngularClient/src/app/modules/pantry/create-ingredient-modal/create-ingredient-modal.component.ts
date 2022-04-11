import { Component, Input, OnInit } from '@angular/core';
import Category from '../../../data/models/Category';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastService } from '../../../shared/services/toast.service';
import Ingredient from '../../../data/models/Ingredient';
import IngredientApi from '../../../data/services/ingredientApi.service';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';

@Component({
  selector: 'app-create-ingredient-modal',
  templateUrl: './create-ingredient-modal.component.html',
  styleUrls: ['./create-ingredient-modal.component.css']
})
export class CreateIngredientModalComponent implements OnInit {

  public addMode: string = "Pantry"; // "Pantry" or "GroceryList"
  public activeList: KitchenList;
  
  public name: string;
  public description: string;
  public selectedCategoryId: number;
  public isPublic: boolean;
  public isAddToPantry: boolean;
  public quantity: number;
  public notes: string;
  public isAdding: boolean;

  public categories: Array<Category>;

  constructor(
    private activeModal: NgbActiveModal,
    private toastService: ToastService,
    private apiService: IngredientApi,
    private listIngredientService: ListIngredientApiService,
    private kitchenIngredientApi: KitchenIngredientApi,
    private activeKitchen: ActiveKitchenService) { }

  ngOnInit(): void {
    
    this.name = this.name ?? "";
    this.description = "";
    this.isPublic = true;
    this.isAddToPantry = true;
    this.isAdding = false;
    this.categories = [];
    this.selectedCategoryId = -1;
    this.quantity = 1;
    this.notes = "";

    this.apiService.getIngredientCategories().subscribe(
      data => {
        this.categories = data;
      },
      error => {
        this.toastService.showDanger("Failed to load categories. re-open this modal to try again - " + error.error);
      });

  }

  confirmAdd(): void {

    if (this.name === "") {
      this.toastService.showDanger("Name is required.");
      return;
    }

    if (this.selectedCategoryId === null || this.selectedCategoryId === undefined || this.selectedCategoryId <= 0) {
      this.toastService.showDanger("Category is required.");
      return;
    }

    this.isAdding = true;

    const toAdd: Ingredient = new Ingredient();
    toAdd.name = this.name;
    toAdd.description = this.description;
    toAdd.categoryId = this.selectedCategoryId;
    toAdd.isPublic = this.isPublic;

    this.apiService.addIngredient(toAdd).subscribe(
      data => {
        this.toastService.showSuccess("Successfully created ingredient - " + toAdd.name);
        this.isAdding = false;
        this.activeModal.close(data);

        if (this.isAddToPantry) {
          if (this.addMode === 'Pantry') {
            this.addToPantry(data);
          } else {
            this.addToGroceryList(data);
          }
        }

      },
      error => {
        this.toastService.showDanger("Could not create ingredient - " + error.error);
        this.isAdding = false;
      });
  }

  private addToGroceryList(x: Ingredient) {
    const toAdd: ListIngredient = this.listIngredientService.createEmpty(x, this.activeList);

    if (toAdd.kitchenId === 0 || toAdd.kitchenListId === 0) {
      this.toastService.showDanger("Cannot add to list - id is 0");
      return;
    }

    this.listIngredientService.addIngredientToList(toAdd).subscribe(data => {
      this.listIngredientService.setAddedIngredient(data);
      this.toastService.showSuccess("Added " + x.name);
    },
      resp => {
        this.toastService.showDanger(resp.error);
        this.listIngredientService.setAddedIngredient(null);
      },
    );
  }

  private addToPantry(toAdd: Ingredient) {
    const toPantry: KitchenIngredient = this.kitchenIngredientApi.createEmpty(toAdd, this.activeKitchen.getActiveKitchenId());
    toPantry.quantity = this.quantity;
    toPantry.note = this.notes;

    this.kitchenIngredientApi.addIngredientToKitchen(toPantry).subscribe(
      data => {
        this.kitchenIngredientApi.setAddedIngredient(data);
        this.toastService.showSuccess("Added to pantry - " + toAdd.name);
      },
      error => {
        this.toastService.showDanger("Could not add " + toAdd.name + " to pantry - " + error.error);
      });
  }

  close(): void {
    this.activeModal.close(null);
  }

}
