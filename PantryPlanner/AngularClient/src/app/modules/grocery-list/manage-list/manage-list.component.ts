import { Component, Input, OnInit } from '@angular/core';
import Kitchen from 'src/app/data/models/Kitchen';
import KitchenList from 'src/app/data/models/KitchenList';
import GroceryListApi from 'src/app/data/services/grocery-list.service';
import { ToastService } from 'src/app/shared/services/toast.service';

@Component({
  selector: 'app-manage-list',
  templateUrl: './manage-list.component.html',
  styleUrls: ['./manage-list.component.css']
})
export class ManageListComponent implements OnInit {

  @Input()
  public activeKitchen: Kitchen;

  @Input()
  public selectedList: KitchenList;


  public isSaving: boolean;
  public isEditing: boolean;
  
  public editId: number;
  public newListName: string = "";

  constructor(
    private listService: GroceryListApi,
    private toastService: ToastService,
  ) { }

  ngOnInit(): void {
    this.isEditing = false;
    this.isSaving = false;
  }

  addMode(): void {
    this.newListName = "";
    this.isEditing = true;
    this.editId = 0;
  }

  removeSelected(): void {
    if (this.isSaving || this.selectedList === null) {
      return;
    }

    this.isSaving = true;

    this.listService.deleteList(this.selectedList.kitchenListId).subscribe(response => {
      this.toastService.showSuccess("Successfully removed list: " + response.name);
      this.listService.removeFromObservable(response);
      this.isSaving = false;
    },
      error => {
        this.toastService.showDanger("Could not remove: " + error.message);
        this.isSaving = false;
      });
  }

  editSelected(): void {
    this.newListName = this.selectedList.name;
    this.editId = this.selectedList.kitchenListId;
    this.isEditing = true;
  }

  cancelEdit(): void {
    this.newListName = "";
    this.isEditing = false;
    this.editId = 0;
  }

  confirmSave(): void {
    if (this.isSaving || this.newListName === null || this.newListName.trim() === "") {
      return;
    }

    this.isSaving = true;

    if (this.editId === 0) {
      this.addNew();
    } else {
      // this.saveEdit();
    }
  }


  private addNew() {
    let toAdd: KitchenList = new KitchenList();
    toAdd.name = this.newListName;
    toAdd.kitchenId = this.activeKitchen.kitchenId;

    this.listService.addList(toAdd).subscribe(response => {
      this.toastService.showSuccess("Successfully added list: " + toAdd.name);
      this.listService.addToObservable(response);
      this.isSaving = false;
      this.isEditing = false;
    },
      error => {
        this.toastService.showDanger("Could not add: " + error.message);
        this.isSaving = false;
      });
  }

  // private saveEdit() {
  //   this.selectedList.name = this.newListName;
  //   this.listService.(this.selectedList).subscribe(response => {
  //     this.toastService.showSuccess("Successfully added list: " + toAdd.name);
  //     this.isSaving = false;
  //   },
  //     error => {
  //       this.toastService.showDanger("Could not add: " + error.message);
  //       this.isSaving = false;
  //     });
  // }

}
