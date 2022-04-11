import { Injectable } from '@angular/core';
import Kitchen from '../../data/models/Kitchen';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ActiveKitchenService {

  public activeKitchen: Kitchen;
  public activeKitchenId: number;

  public observableKitchenId: BehaviorSubject<number>;
  public observableKitchen: BehaviorSubject<Kitchen>;

  constructor() {
    this.activeKitchenId = this.getActiveKitchenId();
    this.observableKitchenId = new BehaviorSubject<number>(this.activeKitchenId);
    this.observableKitchen = new BehaviorSubject<Kitchen>(this.activeKitchen);
  }

  getActiveKitchenId(): number {
    const activeId: string = localStorage.getItem("ActiveKitchenID");

    if (activeId === null || activeId === undefined) {
      return 0;
    }

    this.activeKitchenId = parseInt(activeId);
    return this.activeKitchenId;
  }

  setActiveKitchen(kitchen: Kitchen): void {
    this.activeKitchenId = kitchen.kitchenId;
    this.activeKitchen = kitchen;
    localStorage.setItem("ActiveKitchenID", kitchen.kitchenId.toString());

    this.observableKitchenId.next(this.activeKitchenId);
    this.observableKitchen.next(this.activeKitchen);
  }

  clearActiveKitchen(updateObservable: boolean): void {
    this.activeKitchenId = 0;
    this.activeKitchen = null;
    localStorage.removeItem("ActiveKitchenID");

    if (updateObservable) {
      this.observableKitchenId.next(this.activeKitchenId);
      this.observableKitchen.next(this.activeKitchen);
    }
  }


}
