import {
  ChangeDetectionStrategy,
  Component,
  inject,
  input,
  output,
} from '@angular/core';
import {
  IonButton,
  IonCol,
  IonIcon,
  IonNote,
  IonRow,
  Platform,
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import {
  addCircleOutline,
  bagAddOutline,
  chevronForward,
  removeCircleOutline,
} from 'ionicons/icons';
import { Material } from '../services/data.service';

@Component({
  selector: 'app-material',
  templateUrl: './material.component.html',
  styleUrls: ['./material.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IonNote, IonIcon, IonButton, IonRow, IonCol],
})
export class MaterialComponent {
  queue = output<Material>();
  private platform = inject(Platform);
  material = input.required<Material>();
  isIos() {
    return this.platform.is('ios');
  }
  constructor() {
    addIcons({
      chevronForward,
      addCircleOutline,
      removeCircleOutline,
      bagAddOutline,
    });
  }
  incrementQty() {
    this.material().quantity++;
  }
  decrementQty() {
    if (this.material().quantity <= 1) return;
    this.material().quantity--;
  }
  queueMaterial() {
    this.queue.emit(this.material());
  }
}
