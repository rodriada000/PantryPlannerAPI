import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { isNullOrUndefined } from 'util';
import { CreateIngredientModalComponent } from './create-ingredient-modal/create-ingredient-modal.component';

@Component({
  selector: 'pantry-root',
  templateUrl: './pantry.component.html'
})
export class PantryComponent implements OnInit, OnDestroy {

  public isAddPageSelected: boolean;
  public isSearchPantryPageSelected: boolean;
  public isManageUsersPageSelected: boolean;
  public showSideMenu: boolean;


  constructor(private modalService: NgbModal) { }

  ngOnInit(): void {
    this.showSideMenu = true;
    this.switchToAddIngredients();
  }

  ngOnDestroy(): void {
  }


  public switchToAddIngredients(): void {
    this.isAddPageSelected = true;
    this.isSearchPantryPageSelected = false;
    this.isManageUsersPageSelected = false;
  }

  public switchToSearchPantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = true;
    this.isManageUsersPageSelected = false;
  }

  public switchToManageUsers(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = false;
    this.isManageUsersPageSelected = true;
  }

  openCreateIngredientModal(): void {
    this.modalService.open(CreateIngredientModalComponent);
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }


}
