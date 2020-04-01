import { Injectable } from '@angular/core';
import Kitchen from '../../data/models/Kitchen';
import { isNullOrUndefined } from 'util';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ActiveKitchenService {

  public activeKitchen: Kitchen;
  private activeKitchenId: number;

  public observableKitchenId: BehaviorSubject<number>;

  constructor() {
    this.activeKitchenId = 0;
    this.observableKitchenId = new BehaviorSubject<number>(this.activeKitchenId);
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

    this.observableKitchenId.next(this.activeKitchenId);
  }


}
