import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListDetailComponent } from './list-detail/list-detail.component';
import { ManageListComponent } from './manage-list/manage-list.component';
import { GroceryListComponent } from './grocery-list.component';
import { PantryModule } from '../pantry/pantry.module';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    ListDetailComponent,
    ManageListComponent,
    GroceryListComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    NgbModule,
    NgSelectModule,
    PantryModule
  ],
  providers: [
    NgbActiveModal
  ],
  exports: [
    GroceryListComponent,
    ManageListComponent,
    ListDetailComponent,
  ]
})
export class GroceryListModule {
}
