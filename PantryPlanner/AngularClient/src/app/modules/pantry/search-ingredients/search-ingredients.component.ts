import { Component, OnInit } from '@angular/core';
import IngredientApi from '../../../data/services/ingredientApi.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import Ingredient from '../../../data/models/Ingredient';

@Component({
  selector: 'pantry-search-ingredients',
  templateUrl: './search-ingredients.component.html',
  styleUrls: ['./search-ingredients.component.css']
})
export class SearchIngredientsComponent implements OnInit {

  public searchText: string;
  public searchResults: Array<Ingredient>


  constructor(private apiService: IngredientApi, private activeKitchen: ActiveKitchenService) { }

  ngOnInit(): void {
    this.searchText = "";
    this.searchResults = [];
  }

  doSearch(): void {

    if (this.searchText === "") {
      this.searchResults = [];
      return;
    }

    const currentText: string = this.searchText;

    setTimeout(() => {
      if (currentText !== this.searchText) {
        return; // user started typing more after a second so don't search yet
      }

      this.apiService.getIngredientsByName(this.searchText).subscribe(data => {
        this.searchResults = data;
      });
    }, 1000);
  }

  showAddModal(): void {
    return;
  }


}
