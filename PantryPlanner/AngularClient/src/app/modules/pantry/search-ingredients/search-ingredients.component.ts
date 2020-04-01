import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import IngredientApi from '../../../data/services/ingredientApi.service';
import Ingredient from '../../../data/models/Ingredient';
import { AddIngredientModalComponent } from '../add-ingredient-modal/add-ingredient-modal.component';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError, map } from 'rxjs/operators';
import { isNullOrUndefined } from 'util';

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


  constructor(private apiService: IngredientApi, private modalService: NgbModal) { }

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
          map(r => r.slice(0,20)),
          catchError(() => {
            this.searchFailed = true;
            return of([]);
          }))
      ),
      tap(() => this.isSearching = false)
    )

  // don't keep the selected input just clear it out once done
  formatter = (x: { name: string }) => "";


  openAddModal(selected: Ingredient): void {
    const modalRef = this.modalService.open(AddIngredientModalComponent);
    modalRef.componentInstance.ingredient = selected;

    modalRef.result.then((result) => {
      if (!isNullOrUndefined(result)) {
        console.log(result);
      }
    });
  }

}
