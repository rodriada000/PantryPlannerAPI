import { Injectable } from '@angular/core';
import Kitchen from '../../data/models/Kitchen';
import { isNullOrUndefined } from 'util';

@Injectable({
  providedIn: 'root'
})
export class ActiveKitchenService {

  public activeKitchen: Kitchen;
  private activeKitchenId: number;

  constructor() {
    this.activeKitchenId = 0;
  }

  getActiveKitchenId(): number {
    const activeId: string = localStorage.getItem("ActiveKitchenID");

    if (isNullOrUndefined(activeId)) {
      return 0;
    }

    this.activeKitchenId = parseInt(activeId);
    return this.activeKitchenId;
  }

  setActiveKitchen(kitchen: Kitchen) {
    this.activeKitchenId = kitchen.kitchenId;
    this.activeKitchen = kitchen;
    localStorage.setItem("ActiveKitchenID", kitchen.kitchenId.toString());
  }


}
