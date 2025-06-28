import { Injectable, signal } from '@angular/core';

/**
 * A simple, injectable service to manage the state of global modals.
 * Components can call `open()` to show a modal and `close()` to hide it.
 * The AppLayoutComponent listens to this service to render the correct modal.
 */
@Injectable({
  providedIn: 'root'
})
export class ModalService {
  // A private signal to hold the open/closed state.
  private isModalOpen = signal(false);
  
  // A private signal to hold data passed to the modal,
  // including a 'type' to identify which modal to show.
  private modalData = signal<any>(null);

  /**
   * A public, readonly signal for components to check if a modal is open.
   */
  public isModalOpen$ = this.isModalOpen.asReadonly();
  
  /**
   * A public, readonly signal for modal components to get the data they need.
   */
  public modalData$ = this.modalData.asReadonly();

  /**
   * Opens a modal.
   * @param data An object containing data for the modal. Crucially, it must
   *             include a `type` property (e.g., 'addEditExpense') that
   *             matches a @case in the AppLayoutComponent's template.
   */
  open(data: { type: string, [key: string]: any }): void {
    this.modalData.set(data);
    this.isModalOpen.set(true);
  }

  /**
   * Closes the currently open modal and clears its data.
   */
  close(): void {
    this.isModalOpen.set(false);
    this.modalData.set(null);
  }
}