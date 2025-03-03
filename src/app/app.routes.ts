import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./home/home.page').then((m) => m.HomePage),
  },
  {
    path: 'material/:id',
    loadComponent: () =>
      import('./view-material/view-material.page').then(
        (m) => m.ViewMaterialPage
      ),
  },
];
