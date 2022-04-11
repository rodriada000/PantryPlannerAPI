import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { skipWhile } from 'rxjs/operators';
import Kitchen from 'src/app/data/models/Kitchen';
import KitchenList from 'src/app/data/models/KitchenList';
import ListIngredientApiService from 'src/app/data/services/grocery-list-ingredient.service';
import GroceryListApi from 'src/app/data/services/grocery-list.service';
import KitchenUserApi from 'src/app/data/services/kitchenUserApi.service';
import { ActiveKitchenService } from 'src/app/shared/services/active-kitchen.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-grocery-list',
  templateUrl: './grocery-list.component.html',
  styleUrls: ['./grocery-list.component.css']
})
export class GroceryListComponent implements OnInit {

  public showSideMenu: boolean;
  public activeKitchen: Kitchen;
  private observingKitchen: Subscription;

  public allLists: Array<KitchenList> = [];
  public selectedIndex: number = 0;

  constructor(
    private activeKitchenService: ActiveKitchenService,
    private listService: GroceryListApi,
    private ingredientService: ListIngredientApiService,
    private toastService: ToastService,
    private kitchenUserApi: KitchenUserApi,
  ) { }

  ngOnInit(): void {
    this.showSideMenu = true;
    const lastSelected: string = localStorage.getItem("LastSelectedKitchenListId");

    this.observingKitchen = this.activeKitchenService.observableKitchen.subscribe(k => {
      if (k !== null && k !== undefined) {
        this.activeKitchen = k;
      }
    });

    this.listService.observableList.subscribe(k => {
      if (k === null || k === undefined) {
        return;
      }

      this.allLists = k;

      if (this.allLists.length !== 0) {
        this.selectedIndex = 0;
        
        if (lastSelected !== null && lastSelected !== undefined) {
          this.selectedIndex = this.allLists.findIndex(k => k.kitchenListId === parseInt(lastSelected));
        }
        
        if (this.selectedIndex >= this.allLists.length) {
          this.selectedIndex = 0;
        }
      }

    })

    this.refreshLists();
  }

  refreshLists() {
    this.listService.getAllGroceryLists().subscribe(lists => {
      this.allLists = lists ?? [];
      this.listService.setObservable(this.allLists);
    }
      , error => this.toastService.showDanger(error.message));
  }

  switchList(index: number) {
    this.selectedIndex = index;
    localStorage.setItem("LastSelectedKitchenListId", this.allLists[index]?.kitchenListId?.toString());
  }

  getSelected(): KitchenList {
    if (this.selectedIndex === -1 && this.selectedIndex >= this.allLists.length) {
      return null;
    }

    return this.allLists[this.selectedIndex];
  }

  ngOnDestroy(): void {
    this.observingKitchen?.unsubscribe();
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }


}
