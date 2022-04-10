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
  public selectedSortOrder: number = 1;
  public selectedSort: string = "Name";
  public hoveredIndex: number;
  public isLoading: boolean;
  
  private itemAddedSub: Subscription;

  constructor(
    private service: ListIngredientApiService,
    private toasts: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    console.log(changes);
    if (changes.selected && changes.selected.currentValue !== null && changes.selected.currentValue !== undefined) {
      this.selected = changes.selected.currentValue;
      this.selectedName = this.selected.name;
      this.cdr.detectChanges();
      console.log(this.selected, this.selectedName);
    }
  }


  ngOnInit(): void {

    // add new ingredients to list when they are added to kitchen
    this.itemAddedSub = this.service.observableAddedIngredient.subscribe(newIngredient => {
      if (newIngredient !== null && newIngredient !== undefined) {
        this.allIngredients.push(newIngredient);
        // this.sortBy(this.selectedSort, false);
        // this.doFilter();
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
      // this.doFilter();
      // this.sortBy('name')
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
    // this.sortBy(this.selectedSort);
    // this.doFilter();
  }

  quickEditQty(ingredient: ListIngredient, qtyToAdd: number) {

  }

}
