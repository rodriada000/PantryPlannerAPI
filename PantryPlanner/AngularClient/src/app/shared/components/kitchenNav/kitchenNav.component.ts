import { Component, OnInit } from '@angular/core';
import ApiService from '../../services/api.service';
import Kitchen from '../../models/Kitchen';

@Component({
  selector: 'kitchen-nav',
  templateUrl: './kitchenNav.component.html',
})
export class KitchenNavComponent implements OnInit {
  collapsed = true;

  myKitchens: Array<Kitchen>;
  activeKitchenName: string;
  public newKitchenName: string;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.newKitchenName = "";
    this.myKitchens = [];
    this.setActiveKitchenName();

    this.apiService.getAllKitchens().subscribe(data => {
      this.myKitchens = data;

      console.log(this.myKitchens);
      this.setActiveKitchenName();
    });
  }

  setActiveKitchenName() {

    const activeId: string = localStorage.getItem("ActiveKitchenID");

    if (activeId === null) {
      if (this.myKitchens.length > 0) {
        this.activeKitchenName = "Select Kitchen";
      } else {
        this.activeKitchenName = "Create Kitchen";
      }
    }
    else {
      if (this.myKitchens.length > 0) {
        console.log(activeId);
        console.log(parseInt(activeId));
        const kitchenIndex: number = this.myKitchens.findIndex(kit => { return kit.kitchenId === parseInt(activeId); });

        if (kitchenIndex === -1) {
          this.activeKitchenName = "Select Kitchen";
        } else {
          this.activeKitchenName = this.myKitchens[kitchenIndex].name;
        }

      } else {
        this.activeKitchenName = "Loading Kitchens";
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
      console.log(data);
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

    if (selected === null || selected === undefined) {
      return;
    }

    localStorage.setItem("ActiveKitchenID", selected.kitchenId.toString());
    this.setActiveKitchenName(); // call this to update navbar UI of selected
  }

}
