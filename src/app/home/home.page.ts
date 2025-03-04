import { NgIf } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
  IonButton,
  IonButtons,
  IonContent,
  IonGrid,
  IonHeader,
  IonIcon,
  IonItem,
  IonItemDivider,
  IonItemGroup,
  IonLabel,
  IonList,
  IonSearchbar,
  IonSpinner,
  IonTitle,
  IonToolbar,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import { MaterialComponent } from '../material/material.component';
import { QueueMaterialComponent } from '../queue-material/queue-material.component';

import { barcodeOutline, printOutline } from 'ionicons/icons';
import { DataService, Material, User } from '../services/data.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.page.html',
  styleUrls: ['home.page.scss'],
  imports: [
    IonHeader,
    IonToolbar,
    IonTitle,
    IonContent,
    IonList,
    MaterialComponent,
    QueueMaterialComponent,
    IonSearchbar,
    IonItem,
    IonGrid,
    FormsModule,
    IonItemDivider,
    IonItemGroup,
    IonLabel,
    IonIcon,
    IonSpinner,
    NgIf,
    IonButton,
    IonButtons,
  ],
})
export class HomePage {
  private data = inject(DataService);
  private lastKeyDown = new Date();
  private loading?: HTMLIonLoadingElement;

  user = signal<User>({ name: '', email: '', roles: '' });
  spinning = signal(false);
  materials = signal<Material[]>([]);
  printQueue = signal<Material[]>([]);
  searchTerm = signal('');

  constructor() {
    addIcons({ barcodeOutline, printOutline });
    this.spinning.set(true);
    this.loadMaterials();
    this.loadUser();
    //this.searchTerm.set('hello');
    // Focus searchbar every 5 seconds (unless a key was pressed in the last 5 seconds)
    const focus = this.focusSearchBar.bind(this);
    setInterval(focus, 5000);
    window.addEventListener('keydown', this.registerKeyDown.bind(this));
    window.addEventListener('load', focus);
  }
  private focusSearchBar() {
    if (new Date().getTime() - this.lastKeyDown.getTime() < 5000) return;
    let el = document.querySelector('#searchbar input') as HTMLInputElement;
    el.focus();
  }
  private registerKeyDown(event: KeyboardEvent) {
    this.lastKeyDown = new Date();
  }

  loadMaterials() {
    this.spinning.set(true);
    this.data.getMaterials(this.searchTerm()).then((r) => {
      this.spinning.set(false);
      this.materials.set(r);
    });
  }
  loadUser() {
    this.data.getUser().then(this.user.set);
  }
  unqueueMaterial(material: Material) {
    this.printQueue.update((queue) => {
      queue = queue.filter((m) => m.materialNo !== material.materialNo);
      return JSON.parse(JSON.stringify(queue));
    });
  }
  queueMaterial(material: Material) {
    let existingItem = this.printQueue().find(
      (m) => m.materialNo === material.materialNo
    );
    if (existingItem) {
      existingItem.quantity++;
      this.printQueue.update((queue) => {
        // Generate new reference to trigger change detection
        return JSON.parse(JSON.stringify(queue));
      });
    } else {
      this.printQueue.update((queue) => {
        queue.push(material);
        return JSON.parse(JSON.stringify(queue));
      });
    }
  }
}
