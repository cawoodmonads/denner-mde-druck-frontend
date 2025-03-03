import { Injectable } from '@angular/core';
import { Material } from './data.service';

//const DEV_API_URL = 'https://67bf103cb2320ee050127892.mockapi.io/api/v1';
const DEV_API_URL = 'http://localhost:7091/api';
const PROD_API_URL = '/api';
const baseUrl = document.location.host.match(/localhost/)
  ? DEV_API_URL
  : PROD_API_URL;

@Injectable({
  providedIn: 'root',
})
export class APIService {
  async articleSearch(searchTerm: string): Promise<Material[]> {
    const params = new URLSearchParams({ query: searchTerm }).toString();
    const res = await fetch(`${baseUrl}/articles/search?${params}`);
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
