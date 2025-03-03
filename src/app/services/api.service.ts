import { Injectable } from '@angular/core';
import { Material } from './data.service';

const baseUrl = 'https://67bf103cb2320ee050127892.mockapi.io/api/v1';

@Injectable({
  providedIn: 'root',
})
export class APIService {
  async articleSearch(searchTerm: string): Promise<Material[]> {
    const params = new URLSearchParams({}).toString();
    //const params = new URLSearchParams({ q: searchTerm }).toString();
    const res = await fetch(`${baseUrl}/articles?${params}`);
    const articles = await res.json();
    let materials = articles.map((article: any) => {
      return new Material({
        materialNo: article.materialNo,
        description: article.description,
        unit: article.unit,
        price: article.price,
        quantity: 1,
      });
    });
    return materials;
  }
}
