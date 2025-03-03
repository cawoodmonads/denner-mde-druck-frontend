import { Component } from '@angular/core';
import { IonApp } from '@ionic/angular/standalone';
import { HomePage } from './home/home.page';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: 'app.component.html',
  imports: [IonApp, HomePage],
})
export class AppComponent {
  constructor() {
    console.log('Hello, world!');
  }
}
