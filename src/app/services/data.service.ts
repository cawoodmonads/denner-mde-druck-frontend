import { inject, Injectable } from '@angular/core';
import { APIService } from './api.service';

const MINIMUM_SEARCH_LENGTH = 3;

export class Material {
  materialNo!: string;
  description!: string;
  unit!: string;
  price!: number;
  quantity!: number;

  constructor(o: any) {
    this.materialNo = o.materialNo;
    this.description = o.description;
    this.unit = o.unit;
    this.price = o.price;
    this.quantity = o.quantity || 1;
  }
  toSearchText() {
    return `${this.materialNo} ${this.description}`.toLowerCase();
  }
}

@Injectable({
  providedIn: 'root',
})
export class DataService {
  apiService = inject(APIService);

  constructor() {}

  public async getMaterials(searchTerm: string): Promise<Material[]> {
    //if (searchTerm?.length < MINIMUM_SEARCH_LENGTH) return [];
    try {
      let materials = await this.apiService.articleSearch(searchTerm);
      materials.forEach((m) => {
        m.quantity = 1;
      });
      return materials.filter(this.doSearch(searchTerm));
    } catch (e) {
      console.error(e);
      return [];
    }
  }
  doSearch(searchTerm: string) {
    return (m: Material) =>
      !searchTerm || m.toSearchText().includes(searchTerm.toLowerCase());
  }
}
