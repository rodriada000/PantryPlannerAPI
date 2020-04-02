import { Component, OnInit, OnDestroy } from '@angular/core';
import KitchenUser from '../../../data/models/KitchenUser';
import KitchenUserApi from '../../../data/services/kitchenUserApi.service';
import { ToastService } from '../../../shared/services/toast.service';
import { ActiveKitchenService } from '../../../shared/services/active-kitchen.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'pantry-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit, OnDestroy {

  public isLoading: boolean;
  public isSendingInvite: boolean;
  public hoveredIndex: number;
  public username: string;
  public users: Array<KitchenUser>

  private activeKitchenSub: Subscription;

  constructor(private apiService: KitchenUserApi, private toasts: ToastService, private activeKitchen: ActiveKitchenService) { }

  ngOnInit(): void {
    this.isLoading = false;
    this.isSendingInvite = false;
    this.hoveredIndex = -1;
    this.username = "";

    this.activeKitchenSub = this.activeKitchen.observableKitchenId.subscribe(id => { this.getUsersInKitchen(); });
  }

  ngOnDestroy(): void {
    this.activeKitchenSub.unsubscribe();
  }

  public getUsersInKitchen(): void {
    this.isLoading = true;
    this.apiService.getAllUsersForKitchen(this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.users = data;
    },
      error => { this.toasts.showDanger(error.message + " - " + error.error) },
      () => { this.isLoading = false; });
  }

  public removeUser(selected: KitchenUser, index: number): void {
    this.apiService.deleteKitchenUserByKitchenUserId(selected.kitchenUserId).subscribe(data => {
      this.toasts.showSuccess("Successfully removed " + this.username);
      this.users.splice(index, 1);
    },
      error => { this.toasts.showDanger("Failed to remove user - " + error.error) });
  }

  public sendInvite(): void {
    this.isSendingInvite = true;
    this.apiService.inviteUserToKitchen(this.username, this.activeKitchen.getActiveKitchenId()).subscribe(data => {
      this.toasts.showSuccess("Successfully sent invite to " + this.username + "! They must accept the invite before being able to add to the pantry.");
      this.getUsersInKitchen();
    },
      error => {
        this.toasts.showDanger("Failed to send invite - " + error.error);
        this.isSendingInvite = false;
      },
      () => { this.isSendingInvite = false; });
  }

}
