import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from "../../sidebar/sidebar.component/sidebar.component";
import { AddEditExpenseModalComponent } from '../../../../features/expenses/components/add-edit-expense-modal/add-edit-expense-modal.component/add-edit-expense-modal.component';
import { ModalService } from '../../../../core/services/modal.service';

@Component({
  selector: 'app-app-layout.component',
  imports: [RouterOutlet, SidebarComponent, AddEditExpenseModalComponent],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.css'
})
export class AppLayoutComponent {
  public modalService = inject(ModalService);

}
