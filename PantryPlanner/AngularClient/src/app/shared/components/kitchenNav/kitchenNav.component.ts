import { Component, OnInit } from '@angular/core';
import KitchenApi from '../../../data/services/kitchenApi.service';
import Kitchen from '../../../data/models/Kitchen';
import { ActiveKitchenService } from '../../services/active-kitchen.service';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'kitchen-nav',
  templateUrl: './kitchenNav.component.html',
})
export class KitchenNavComponent implements OnInit {
  collapsed = true;

  myKitchens: Array<Kitchen>;
  activeKitchenName: string;
  public newKitchenName: string;

  constructor(private apiService: KitchenApi, private activeKitchen: ActiveKitchenService) {}

  ngOnInit() {
    this.newKitchenName = "";
    this.activeKitchenName = "Loading Kitchens";
    this.myKitchens = [];

    this.apiService.getAllKitchens().subscribe(data => {
      this.myKitchens = data;
      this.updateActiveKitchenName();
    });
  }

  updateActiveKitchenName() {

    const activeId: number = this.activeKitchen.getActiveKitchenId();

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

    console.log("adding kitchen ...");

    if (!this.validateKitchen()) {
      console.log("not valid");
      return;
    }

    const kitchen: Kitchen = new Kitchen();
    kitchen.name = this.newKitchenName;
    kitchen.description = "";

    this.apiService.addKitchen(kitchen).subscribe(data => {
      this.myKitchens.push(data);
    });
  }

  validateKitchen(): boolean {

    if (this.newKitchenName === "") {
      return false;
    }

    if (this.myKitchens.some(kitchen => { return kitchen.name === this.newKitchenName})) {
      return false;
    }

    if (this.myKitchens.length >= 5) {
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

}
