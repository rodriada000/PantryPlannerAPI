import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
  selector: 'pantry-root',
  templateUrl: './pantry.component.html'
})
export class PantryComponent implements OnInit, OnDestroy {

  public isAddPageSelected: boolean;
  public isSearchPantryPageSelected: boolean;
  public isManageUsersPageSelected: boolean;


  constructor() { }

  ngOnInit(): void {
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


}
