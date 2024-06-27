import { Injectable } from '@angular/core';
import Dexie from 'dexie';
import { Producto } from '../dto/producto';

@Injectable({
  providedIn: 'root',
})
export class DbService extends Dexie {
  constructor() {
    // Llama al constructor de la clase base Dexie, proporcionando el nombre de la base de datos.
    super('ventasDb');

    // Define el esquema de la base de datos.
    this.version(1).stores({
      ventas: '++id, codigo, nombre, total', 
      // Agrega aquí más tablas si es necesario.
    });

    // Abre la base de datos.
    this.open().catch((err) => {
      console.error(`Open failed: ${err.stack}`);
    });
  }

  // Método para agregar un producto a la base de datos.
  async addProduct(product: Producto) {
    try {
      const id = await this.table('ventas').add(product);
      console.log('Product added with id', id);
      return id;
    } catch (error) {
      console.error('Error adding product:', error);
      throw error;
    }
  }

  async addDelete(product: Producto) {
    try {
      const id = await this.table('ventas').delete(1);
      console.log('Product deleted with id', id);
      return true;
    } catch (error) {
      console.error('Error deleting product:', error);
      throw error;
    }
  }
}
