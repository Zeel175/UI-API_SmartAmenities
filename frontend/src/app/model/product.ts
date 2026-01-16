import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class Product extends BaseAuditable {
  productId: any;

  id: number;
  productCode: string;
  productName: string;
  productGroupId: number | null;
  productGroup: string | null; // Added property
  typeOfConnectionId: number | null;
  typeOfConnection: string | null; // Added property
  productCategoryId: number;
  productCategory: string; // Added property
  productTypeId: number;
  productType: string; // Added property
  productBrandId: number | null;
  productBrand: string | null; // Added property
  productSizeId: number | null;
  productSize: string | null; // Added property
  hsnCode: string;
  drawingNumber: string;
  productDescription: string;
  partCode: string | null;
  interStateTaxId: number;
  interStateTax: string; // Added property
  intraStateTaxId: number;
  intraStateTax: string; // Added property
  // uomId: number;
  // uom: string; // Added property
  uomIds: number[]; // Add this property
  productModelNumber: string | null;
  productImageId: number | null;
  productImage: DocumentDetails;
  productionCompletionDays: number;
  name: any;
  discountTypeId: null;
  discountTypeAmount: number;
  amount: number;
  uomName: string;
  /**
   * Combined UOM names for display in lists
   */
  uom?: string;
  qty: number;
  some: any;
  processes: any;
  minQty: number;
  maxQty: number;
  quantity?: any;
 // New Property - Product Level
 productLevelId?: number;
 productLevel: string;
 productUOMs: ProductUOM[]; // Collection mapping
  productLocationId: any;
  uoMs: ProductUOMs[];
  productConversions?: ProductConversion[];
}
export class ProductUOM extends BaseAuditable {
  id: number;
  productId: number;
  uomId: number;
  /**
   * Optional name of the UOM when returned from the API
   */
  uomName?: string;
 
}
export class ProductUOMs extends BaseAuditable {
  uomId: number;
  //productId: number;
  uomName: string;
  
}

export class ProductConversion extends BaseAuditable {
  id: number;
  productId: number;
  fromUomId: number;
  toUomId: number;
  value: number;
}
