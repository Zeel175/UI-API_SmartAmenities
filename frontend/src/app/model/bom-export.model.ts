export interface BomRawItem {
  inputProductCode: string;
  inputProductName: string;
  qty:              number;
  uom:              string;
  drawingNumber:    string;
  materialGrade: string;
}

export interface BomProcessDetailExport {
  processName:       string;
  outputProductCode:  string;
  outputProductName: string;
  qty:               number;
  uom:               string;
  rawItems:          BomRawItem[];
}

export interface BomProcessExport {
  bomCode:        string;
  bomDate:        string;    // ISO string
  productName:    string;
  baseQty:        number;
  versionCode:    string;
  remarks:        string;
  processDetails: BomProcessDetailExport[];
}

