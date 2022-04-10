import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import IngredientApi from '../../../data/services/ingredientApi.service';
import Ingredient from '../../../data/models/Ingredient';
import { AddIngredientModalComponent } from '../add-ingredient-modal/add-ingredient-modal.component';
import KitchenIngredient from '../../../data/models/KitchenIngredient';
import { ToastService } from '../../../shared/services/toast.service';
import KitchenIngredientApi from '../../../data/services/kitchenIngredientApi.service';
import { CreateIngredientModalComponent } from '../create-ingredient-modal/create-ingredient-modal.component';

@Component({
  selector: 'pantry-search-ingredients',
  templateUrl: './search-ingredients.component.html',
  styleUrls: ['./search-ingredients.component.css']
})
export class SearchIngredientsComponent implements OnInit {

  public isSearching: boolean;
  public searchFailed: boolean;
  public searchText: string;
  public searchResults: Array<Ingredient>


  constructor(
    private apiService: IngredientApi,
    private kitchenIngredientApi: KitchenIngredientApi,
    private modalService: NgbModal,
    private activeKitchen: ActiveKitchenService,
    private toastService: ToastService) { }

  ngOnInit(): void {
    this.searchText = "";
    this.searchResults = [];
    this.isSearching = false;
    this.searchFailed = false;
  }

  doSearch = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      tap(() => this.isSearching = true),
      switchMap(term => term === "" || term.length < 2 ? of([]) :
        this.apiService.getIngredientsByName(term).pipe(
          tap(() => this.searchFailed = false),
          map(r => r.slice(0, 20)),
          map(r => {
            const createMissing: Ingredient = new Ingredient();
            createMissing.name = "Create Missing Ingredient";
            createMissing.categoryName = "CreateMissing";
            r.push(createMissing);

            return r;
          }),
          catchError(() => {
            this.searchFailed = true;
            return of([]);
          }))
      ),
      tap(() => {
        this.isSearching = false;
      })
    )

  isCreateMissingDropdownItem(dropdownItem: Ingredient): boolean {
    return !isNullOrUndefined(dropdownItem) && dropdownItem.categoryName === "CreateMissing";
  }

  // don't keep the selected input just clear it out once added
  // adds the ingredient to kitchen with quantity 1 and no notes
  quickAdd = (x: Ingredient) => {

    if (this.modalService.hasOpenModals()) {
      return;
    }

    if (isNullOrUndefined(x) || isNullOrUndefined(x.ingredientId)) {
      console.warn("can not quick add - ingredient is null");
      return;
    }

    const toAdd: KitchenIngredient = this.kitchenIngredientApi.createEmpty(x, this.activeKitchen.getActiveKitchenId());

    if (toAdd.kitchenId === 0) {
      this.toastService.showDanger("Cannot add to kitchen - kitchen id is 0");
      return;
    }

    this.kitchenIngredientApi.addIngredientToKitchen(toAdd).subscribe(data => {
      this.kitchenIngredientApi.setAddedIngredient(data);
      this.toastService.showSuccess("Added " + x.name);
    },
      resp => {
        this.toastService.showDanger(resp.error);
        this.kitchenIngredientApi.setAddedIngredient(null);
      },
    );

    return "";
  };


  openAddModal(selected: Ingredient): void {
    const modalRef = this.modalService.open(AddIngredientModalComponent);
    modalRef.componentInstance.ingredient = selected;

    modalRef.result.then((result) => {
      if (!isNullOrUndefined(result)) {
      }
    });
  }

  openCreateIngredientModal(): void {
    const modalRef = this.modalService.open(CreateIngredientModalComponent);
    modalRef.componentInstance.name = this.searchText;
  }

}
