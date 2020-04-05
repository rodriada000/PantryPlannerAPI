import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CreateIngredientModalComponent } from './create-ingredient-modal/create-ingredient-modal.component';
import { ActiveKitchenService } from '../../shared/services/active-kitchen.service';
import KitchenUserApi from '../../data/services/kitchenUserApi.service';
import { ToastService } from '../../shared/services/toast.service';
import KitchenApi from '../../data/services/kitchenApi.service';
import { Subscription } from 'rxjs';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'pantry-root',
  templateUrl: './pantry.component.html'
})
export class PantryComponent implements OnInit, OnDestroy {

  public isAddPageSelected: boolean;
  public isSearchPantryPageSelected: boolean;
  public isManagePantryPageSelected: boolean;
  public isLeavePantryPageSelected: boolean;
  public isOwnerOfKitchen: boolean;
  public activeKitchenName: string;
  public showSideMenu: boolean;

  private observingKitchen: Subscription;

  constructor(
    private modalService: NgbModal,
    private activeKitchen: ActiveKitchenService,
    private toastService: ToastService,
    private kitchenUserApi: KitchenUserApi,
    private kitchenApi: KitchenApi) { }

  ngOnInit(): void {
    this.showSideMenu = true;
    this.activeKitchenName = "";
    this.switchToAddIngredients();
    this.isOwnerOfKitchen = false;

    this.observingKitchen = this.activeKitchen.observableKitchen.subscribe(k => {
      if (!isNullOrUndefined(k)) {
        this.activeKitchenName = k.name;

        if (this.isManagePantryPageSelected || this.isLeavePantryPageSelected) {
          this.switchToAddIngredients(); // don't leave user on manage/leave pantry page after they switch kitchens in case their ownership changes 
        }

        this.kitchenUserApi.isOwnerOfKitchen(k.kitchenId).subscribe(
          data => {
            this.isOwnerOfKitchen = data;
          }
        );
      }
    });

  }

  ngOnDestroy(): void {
    this.observingKitchen.unsubscribe();
  }


  public switchToAddIngredients(): void {
    this.isAddPageSelected = true;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = false;

  }

  public switchToSearchPantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = true;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = false;

  }

  public switchToManagePantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = true;
    this.isLeavePantryPageSelected = false;
  }

  public switchToLeavePantry(): void {
    this.isAddPageSelected = false;
    this.isSearchPantryPageSelected = false;
    this.isManagePantryPageSelected = false;
    this.isLeavePantryPageSelected = true;
  }

  openCreateIngredientModal(): void {
    this.modalService.open(CreateIngredientModalComponent);
  }

  toggleSideMenu(): void {
    this.showSideMenu = !this.showSideMenu;
  }

  deleteKitchen(): void {
    this.kitchenApi.deleteKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(
      data => {
        this.activeKitchen.clearActiveKitchen(false);
        window.location.reload();
      },
      error => {
        this.toastService.showDanger("Failed to delete pantry - " + error.error);
      }
    );
  }

  removeSelfFromKitchen(): void {
    this.kitchenUserApi.deleteSelfFromKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(
      data => {
        this.activeKitchen.clearActiveKitchen(false);
        window.location.reload();
      },
      error => {
        this.toastService.showDanger("Failed to leave pantry - " + error.error);
      }
    );
  }

}
