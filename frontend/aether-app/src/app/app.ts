import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from './core/store/auth/auth.actions';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{
  protected title = 'aether-app';

  private store = inject(Store);

  ngOnInit(): void {
    // Dispatch the auto-login action as soon as the app component loads.
    this.store.dispatch(AuthActions.autoLoginStart());
  }
}
