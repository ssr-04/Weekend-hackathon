import { Component } from '@angular/core';
import { HeaderComponent } from "../../header/header.component/header.component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-app-layout.component',
  imports: [HeaderComponent, RouterOutlet],
  templateUrl: './app-layout.component.html',
  styleUrl: './app-layout.component.css'
})
export class AppLayoutComponent {

}
