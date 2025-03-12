import { Injectable } from '@angular/core';
import { Material, PrintRequest, User } from './data.service';

const DEV_API_URL = 'http://localhost:7071/api';
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
  async userDetails(): Promise<User> {
    if (document.location.host.match(/localhost/)) {
      return {
        name: 'Local User',
        email: '',
        roles: ['admin'],
        branch: '9999',
      };
    }
    const ignoreRoles = ['anonymous', 'authenticated'];
    try {
      const res = await fetch(`/.auth/me`);
      const obj = await res.json();
      if (!obj?.clientPrincipal) throw new Error('No user found');
      const user = obj.clientPrincipal;
      if (!user.userRoles) user.userRoles = [];
      const roles = user.userRoles.filter(
        (r: string) => !r.startsWith('branch:') && !ignoreRoles.includes(r)
      );
      const branches = user.userRoles
        .filter((r: string) => r.startsWith('branch:'))
        .map((r: string) => r.split(':')[1]);
      return {
        name: user.userDetails || 'UNKNOWN',
        email: user.userDetails || 'UNKNOWN',
        roles,
        branch: branches.length ? branches[0] : '',
      };
    } catch (e) {
      return {
        name: 'ERROR',
        email: '',
        roles: [],
        branch: '',
      };
    }
  }
  async print(printRequest: PrintRequest): Promise<string> {
    const res = await fetch(`/api/print`, {
      method: 'POST',
      body: JSON.stringify(printRequest),
      headers: {
        'Content-Type': 'application/json',
      },
    });
    return await res.text();
  }
}
