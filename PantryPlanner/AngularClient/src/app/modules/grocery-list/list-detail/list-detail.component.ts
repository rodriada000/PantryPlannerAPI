import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { Subscription } from 'rxjs';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'grocery-list-detail',
  templateUrl: './list-detail.component.html',
  styleUrls: ['./list-detail.component.css']
})
export class ListDetailComponent implements OnInit, OnDestroy, OnChanges {

  @Input()
  public selected: KitchenList;
  public selectedName: string = "";


  public allIngredients: Array<ListIngredient> = [];
  public filteredList: Array<ListIngredient> = [];

  public filterText: string = "";
  public selectedSortOrder: number = 1;
  public selectedSort: string = "name";
  public hoveredIndex: number;
  public isLoading: boolean;
  
  private itemAddedSub: Subscription;

  constructor(
    private service: ListIngredientApiService,
    private toasts: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.selected && changes.selected.currentValue !== null && changes.selected.currentValue !== undefined) {
      this.selected = changes.selected.currentValue;
      this.selectedName = this.selected.name;
      this.refreshList();
    }
  }


  ngOnInit(): void {

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.service.observableAddedIngredient.subscribe(newIngredient => {
      if (newIngredient !== null && newIngredient !== undefined) {
        this.allIngredients.push(newIngredient);
        this.sortBy(this.selectedSort, false);
        this.doFilter();
      }
    });

    this.refreshList();
  }

  ngOnDestroy(): void {
    this.itemAddedSub?.unsubscribe();
  }

  refreshList(): void {
    if (this.selected === null || this.selected === undefined) {
      return;
    }

    this.isLoading = true;

    this.service.getIngredientsForList(this.selected?.kitchenListId).subscribe(data => {
      this.allIngredients = data;
      this.sortBy('name', false)
      this.doFilter();
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  setSelected(index: number, $event) {
    if (this.hoveredIndex != index) {
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

  toggleSortOrder() {
    this.sortBy(this.selectedSort);
    this.doFilter();
  }

  quickEditQty(ingredient: ListIngredient, qtyToAdd: number) {
    if (ingredient.quantity + qtyToAdd <= 0) {
      return; // cant have 0 or negative qty
    }

    ingredient.quantity += qtyToAdd;
    this.updateIngredient(ingredient);
  }

  toggleChecked(ingredient: ListIngredient) {
    ingredient.isChecked = !ingredient.isChecked;
    this.updateIngredient(ingredient);
    this.sortBy(this.selectedSort, false);
    this.doFilter();
  }

  updateIngredient(ingredient: ListIngredient, showToast: boolean = false) {
    this.service.updateListIngredient(ingredient).subscribe(response => {
      if (showToast) {
        this.toasts.showSuccess("Updated " + ingredient.ingredient.name + ".");
      }
    },
    error => { this.toasts.showDanger(error.message + " - " + error.error); })
  }

  removeFromList(ingredient: ListIngredient, index: number) {
    this.service.removeListIngredient(ingredient).subscribe(data => {
      this.toasts.showStandard("Removed " + ingredient.ingredient.name + " from list.");
      this.allIngredients.splice(index, 1);
    },
    error => { this.toasts.showDanger(error.message + " - " + error.error); })
  }

  doFilter() {
    if (this.allIngredients === null || this.allIngredients === undefined) {
      this.filteredList = [];
      return;
    }

    this.filteredList = this.allIngredients.filter(p => this.filterText === "" || (p.ingredient.name.toLowerCase().includes(this.filterText.toLowerCase())));

    if (this.hoveredIndex >= this.filteredList.length) {
      this.hoveredIndex = -1;
    }
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

    this.selectedSort = field;

    let checkedItems = this.allIngredients.filter(i => i.isChecked).sort((a, b) => this.sortFn(a,b));
    let uncheckedItems = this.allIngredients.filter(i => !i.isChecked).sort((a, b) => this.sortFn(a,b));


    this.allIngredients = uncheckedItems.concat(checkedItems);
  }

  sortFn(a: ListIngredient, b: ListIngredient) {
    let valA: string = a.ingredient.name;
    let valB: string = b.ingredient.name;

    if (this.selectedSort == 'category') {
      valA = a.ingredient.categoryName;
      valB = a.ingredient.categoryName;
    }

    if (valA.toLowerCase() > valB.toLowerCase()) {
      return 1 * this.selectedSortOrder;
    } else if (valA.toLowerCase() < valB.toLowerCase()) {
      return -1 * this.selectedSortOrder;
    }

    if (this.selectedSort == 'category') {
      if (a.ingredient.name.toLowerCase() > b.ingredient.name.toLowerCase()) {
        return 1 * this.selectedSortOrder;
      } else if (a.ingredient.name.toLowerCase() < b.ingredient.name.toLowerCase()) {
        return -1 * this.selectedSortOrder;
      }
    }

    return 0;
  }


}
