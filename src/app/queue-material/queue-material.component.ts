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
  chevronForward,
  removeCircleOutline,
  trashOutline,
} from 'ionicons/icons';
import { Material } from '../services/data.service';

@Component({
  selector: 'app-queue-material',
  templateUrl: './queue-material.component.html',
  styleUrls: ['./queue-material.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [IonNote, IonIcon, IonButton, IonRow, IonCol],
})
export class QueueMaterialComponent {
  unqueue = output<Material>();
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
      trashOutline,
    });
  }
  incrementQty() {
    this.material().quantity++;
  }
  decrementQty() {
    if (this.material().quantity <= 1) return;
    this.material().quantity--;
  }
  unqueueMaterial() {
    this.unqueue.emit(this.material());
  }
}
