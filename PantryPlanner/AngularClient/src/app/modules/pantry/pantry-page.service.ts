import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PantryPageService {
  
  public observableSelectedPages: BehaviorSubject<any>;

  public isAddPageSelected: boolean;
  public isSearchPantryPageSelected: boolean;
  public isManagePantryPageSelected: boolean;
  public isLeavePantryPageSelected: boolean;
  
  public get pageSelection(): any {
    return {
      'isAddPageSelected': this.isAddPageSelected,
      'isSearchPantryPageSelected': this.isSearchPantryPageSelected,
      'isManagePantryPageSelected': this.isManagePantryPageSelected,
      'isLeavePantryPageSelected': this.isLeavePantryPageSelected
    };
  }

  constructor() { 
    this.observableSelectedPages = new BehaviorSubject<any>(this.pageSelection);
  }

  public switchToAddIngredients(): void {
    this.isAddPageSelected = true;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = false;
    this.observableSelectedPages.next(this.pageSelection);
  }

  public switchToSearchPantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = true;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = false;
    this.observableSelectedPages.next(this.pageSelection);
  }

  public switchToManagePantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = true;
    this.isLeavePantryPageSelected = false;
    this.observableSelectedPages.next(this.pageSelection);
  }

  public switchToLeavePantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = true;
    this.observableSelectedPages.next(this.pageSelection);
  }
}
