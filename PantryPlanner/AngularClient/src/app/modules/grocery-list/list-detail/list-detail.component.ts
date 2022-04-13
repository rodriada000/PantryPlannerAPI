import { ChangeDetectorRef, Component, Input, OnChanges, OnDestroy, OnInit, QueryList, SimpleChanges, ViewChild, ViewChildren } from '@angular/core';
import { NgSelectComponent } from '@ng-select/ng-select';
import { Observable, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import Category from 'src/app/data/models/Category';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredient from 'src/app/data/models/ListIngredient';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';
import KitchenIngredientApi from 'src/app/data/services/kitchenIngredientApi.service';
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
  public categories: Array<Category> = [];

  public lastCatSearch: string;
  public filterText: string = "";
  public selectedSortOrder: number = 1;
  public selectedSort: string = "name";
  public hoveredIndex: number;
  public isLoading: boolean;
  public isEditing: boolean = false;
  public isSaving: boolean = false;
  
  private itemAddedSub: Subscription;
  origIngredient: ListIngredient;

  constructor(
    private service: ListIngredientApiService,
    private pantryService: KitchenIngredientApi,
    private toasts: ToastService
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
        this.sortBy(this.selectedSort);
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
      this.sortBy('name')
      this.doFilter();
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error); },
      () => { this.isLoading = false; });
  }

  setSelected(index: number, $event) {
    if (this.hoveredIndex != index) {
      this.cancelEdit(this.filteredList[this.hoveredIndex], $event);
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
    this.sortBy(this.selectedSort, true);
    this.doFilter();
  }

  toggleChecked(ingredient: ListIngredient) {
    ingredient.isChecked = !ingredient.isChecked;
    this.updateIngredient(ingredient);
    this.sortBy(this.selectedSort);
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
      this.sortBy(this.selectedSort);
      this.doFilter();
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

  sortBy(field: string, toggleSort: boolean = false): void {
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

  confirmAddToPantry() {
    if (confirm('Are you sure you want to add checked items to the pantry?')) {
      this.addCheckedToPantry();
    }
  }

  addCheckedToPantry() {
    this.allIngredients.forEach(i => {
      if (!i.isChecked) {
        return;
      }

      let k = this.pantryService.createEmpty(i.ingredient, this.selected.kitchenId);
      k.quantity = i.quantity ?? 1;

      this.pantryService.addIngredientToKitchen(k).subscribe(data => {
        this.pantryService.setAddedIngredient(data);
      }, error => {
        this.toasts.showDanger('could not add to pantry - ' + error.message);
      });
      
    })
  }

  editIngredient(ingredient: ListIngredient, $event): void {
    this.isEditing = true;
    this.origIngredient = { ...ingredient };
    $event.preventDefault();
    $event.stopPropagation();
  }

  quickEditQty(ingredient: ListIngredient, qtyToAdd: number) {
    if (this.isSaving) {
      return;
    }

    if (ingredient.quantity + qtyToAdd <= 0) {
      return; // cant have 0 or negative qty
    }

    ingredient.quantity += qtyToAdd;
    this.saveEdit(ingredient, false);
  }

  saveEdit(ingredient: ListIngredient, showToast: boolean = true): void {
    if (this.isSaving) {
      return;
    }
    
    if (ingredient.quantity <= 0) {
      return; // cant have 0 or negative qty
    }

    this.isSaving = true;

    this.service.updateListIngredient(ingredient).subscribe(data => {
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

  cancelEdit(ingredient: ListIngredient, $event): void {
    if (ingredient === null || ingredient === undefined || !this.isEditing) {
      return;
    }

    ingredient.note = this.origIngredient?.note;
    ingredient.quantity = this.origIngredient?.quantity;
    this.origIngredient = null;
    this.isEditing = false;
    $event.preventDefault();
    $event.stopPropagation();
  }

  // onBlur() {
  //   console.log('blurred')
  // }

  // onKeyDown($event: KeyboardEvent) {
  //   if ($event.key === 'Enter' || $event.key === 'Tab') {
  //     console.log('keydown', this.lastCatSearch, this.categories);
  //     if (this.lastCatSearch && this.categories.findIndex(c => c.name.toLowerCase() === this.lastCatSearch.toLowerCase()) === -1) {
  //       let newCat = new Category();
  //       newCat.name = this.lastCatSearch;
  //       this.categories = [...this.categories, newCat];
        
  //       this.filteredList[this.hoveredIndex].category = newCat;
  //       console.log(this.categories, this.filteredList[this.hoveredIndex].category);
  //       this.lastCatSearch = null;
  //       return false;
  //     }
  //   }

  //   return true;
  // }

  doCatSearch(text$: Observable<string>) {

  }

  // doCatSearch = (text$: Observable<string>) =>
  // text$.pipe(
  //   debounceTime(300),
  //   distinctUntilChanged(),
  //   tap(() => this.isSearching = true),
  //   switchMap(term => term === "" || term.length < 2 ? of([]) :
  //     this.service.getIngredientsByName(term).pipe(
  //       tap(() => this.searchFailed = false),
  //       map(r => r.slice(0, 20)),
  //       map(r => {
  //         const createMissing: Ingredient = new Ingredient();
  //         createMissing.name = "Create Missing Ingredient";
  //         createMissing.categoryName = "CreateMissing";
  //         r.push(createMissing);

  //         return r;
  //       }),
  //       catchError(() => {
  //         this.searchFailed = true;
  //         return of([]);
  //       }))
  //   ),
  //   tap(() => {
  //     this.isSearching = false;
  //   })
  // )

}
