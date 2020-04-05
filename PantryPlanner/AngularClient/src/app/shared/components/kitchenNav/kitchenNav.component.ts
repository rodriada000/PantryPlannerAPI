import { Component, OnInit } from '@angular/core';
import { isNullOrUndefined } from 'util';
import Kitchen from '../../../data/models/Kitchen';
import KitchenUser from '../../../data/models/KitchenUser';
import KitchenApi from '../../../data/services/kitchenApi.service';
import KitchenUserApi from '../../../data/services/kitchenUserApi.service';
import { ActiveKitchenService } from '../../services/active-kitchen.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'kitchen-nav',
  templateUrl: './kitchenNav.component.html',
})
export class KitchenNavComponent implements OnInit {
  collapsed = true;

  pendingKitchens: Array<KitchenUser>;
  myKitchens: Array<Kitchen>;
  activeKitchenName: string;
  public newKitchenName: string;

  constructor(
    private apiService: KitchenApi,
    private activeKitchen: ActiveKitchenService,
    private kitchenUserService: KitchenUserApi,
    private toastService: ToastService) { }

  ngOnInit() {
    this.newKitchenName = "";
    this.activeKitchenName = "Loading Kitchens";
    this.myKitchens = [];

    this.apiService.getAllKitchens().subscribe(data => {
      this.myKitchens = data;
      this.updateActiveKitchenName();
    });

    this.kitchenUserService.getKitchenInvitesForLoggedInUser().subscribe(data => {
      this.pendingKitchens = data;
    },
      error => {
        this.toastService.showDanger("Failed to get pending invites. refresh the page to try again - " + error.error);
      });
  }

  updateActiveKitchenName() {

    let activeId: number = this.activeKitchen.getActiveKitchenId();

    if (activeId === 0 && this.myKitchens.length > 0) {
      activeId = this.myKitchens[0].kitchenId;
      this.activeKitchen.setActiveKitchen(this.myKitchens[0]);
    }

    if (activeId === 0) {
      // user has not set the active kitchen so show text based on amount of kitchens the user has
      if (this.myKitchens.length > 0) {
        this.activeKitchenName = "Select Kitchen";
      } else {
        this.activeKitchenName = "Create Kitchen";
      }
    }
    else {
      // user has 
      if (this.myKitchens.length > 0) {

        const kitchenIndex: number = this.myKitchens.findIndex(kit => { return kit.kitchenId === activeId; });
        if (kitchenIndex === -1) {
          this.activeKitchenName = "Select Kitchen";
        } else {
          this.activeKitchenName = this.myKitchens[kitchenIndex].name;
        }

      } else {
        // user has active kitchen id set but no kitchens so default 'Create' text
        this.activeKitchenName = "Create Kitchen";
      }
    }

  }

  addNewKitchen() {

    if (!this.validateKitchen(this.newKitchenName)) {
      return;
    }

    const kitchen: Kitchen = new Kitchen();
    kitchen.name = this.newKitchenName;
    kitchen.description = "";

    this.apiService.addKitchen(kitchen).subscribe(data => {
      this.myKitchens.push(data);
    });
  }

  validateKitchen(name: string): boolean {

    if (name === "") {
      this.toastService.showDanger("Kitchen name required.");
      return false;
    }

    if (this.myKitchens.some(kitchen => { return kitchen.name === name })) {
      this.toastService.showDanger("Kitchen with that name already exists.");
      return false;
    }

    if (this.myKitchens.length >= 5) {
      this.toastService.showDanger("Only 5 kitchens allowed. Delete or leave another kitchen first.");
      return false;
    }

    return true;
  }

  setActiveKitchen(selected: Kitchen) {
    console.log(selected);

    if (isNullOrUndefined(selected)) {
      return;
    }

    this.activeKitchen.setActiveKitchen(selected);
    this.updateActiveKitchenName(); // call this to update navbar UI of selected
  }

  acceptInvite(selected: KitchenUser, index: number): void {

    if (!this.validateKitchen(selected.kitchenName)) {
      return;
    }

    this.kitchenUserService.acceptKitchenInvite(selected.kitchenId).subscribe(
      data => {
        console.log(data);
        this.toastService.showStandard("Accepted invite to " + selected.kitchenName);
        this.pendingKitchens.splice(index, 1);
        this.myKitchens.push(selected.kitchen);
      },
      error => {
        this.toastService.showDanger("Failed to accept kitchen invite - " + error.error);
      });
  }

  denyInvite(selected: KitchenUser, index: number): void {
    this.kitchenUserService.denyKitchenInvite(selected.kitchenId).subscribe(
      data => {
        console.log(data);
        this.toastService.showStandard("Denied kitchen invite - removed.");
        this.pendingKitchens.splice(index, 1);
      },
      error => {
        this.toastService.showDanger("Failed to deny kitchen invite - " + error.error);
      });
  }

}
