import { FileUploaderService } from '../core/service';
import { BaseAuditable } from './base-auditable';
//import { BOMMaster } from './bom-master';
import { DocumentDetails } from './document-details';

// Main BOM class
export class BOMProcess extends BaseAuditable {
    id: number;
    bomCode: string; // Auto-generated code
    bomDate: Date; // Date when the BOM was created
    productCode: number; // Foreign key to Product
    baseQty: number; // Base quantity
    versionCode: string | null; // Version of the BOM
    reasonForChange?:string;
    reasonForChangeImageId ?: number | null;
    reasonForChangeImage?: DocumentDetails;
    remarks: string; // Optional remarks
    isReject: boolean;
    isApprove: boolean;
    isPublish: boolean;
    bomProcessDetails: BOMProcessDetail[]; // Details related to BOM processes
    bomAttachments: BOMAttachments[];
    bomVersion: BOMProcessVersion[];
    static id: number;
    bomQuantity: any;
    // bomProcessVersions: any;
    constructor() {
        super();
        this.bomProcessDetails = [];
        this.bomAttachments = [];
        this.bomVersion = [];
    }
}

// Details related to BOM Process
export class BOMProcessDetail extends BaseAuditable {
    id: number;
    processCode: number; // Foreign key to Process
    outputProductCode: number; // Foreign key to Product
    qty: number; // Quantity of output product
    uom: string; // Unit of Measure
    bomId: number; // Foreign key to BOM
    bom: BOMProcess; // Reference to BOM
    isNewProcess?: boolean;
     processName?: string; // Name of the process
    bomRawProductDetails: BOMRawProductDetail[]; // Details related to raw products
    
    constructor() {
        super();
        this.bomRawProductDetails = [];
    }
}

// Details related to BOM Raw Product
export class BOMRawProductDetail extends BaseAuditable {
    id: number;
    inputProductCode: number; // Foreign key to Product
    productName: string;
    qty: number; // Quantity of input product
    drawingNumber : string;
    uom: string; // Unit of Measure
    bomProcessDetailId: number; // Foreign key to BOMProcessDetail,
    bomProcessDetail: BOMProcessDetail; // Reference to BOMProcessDetail
}
export class BOMAttachments extends BaseAuditable {
    id?: number;                        // Unique identifier for the attachment
    bomId?: number;                     // The ID of the associated BOM
    documentName: string;               // The name of the uploaded document
    pictureId?: number;                 // ID of the uploaded picture, if any
    picture: DocumentDetails;            // Object containing details about the uploaded document
    uploader: FileUploaderService;      // Service to handle file uploads
    hideUploader: boolean;               // Flag to hide/show the uploader in the UI
    uniqueId: string;                   // Unique ID for identifying the attachment instance

    // constructor() {
    //     super();
    //     this.documentName = '';
    //     this.hideUploader = false;
    //     this.uniqueId = this.generateUniqueId(); // Generate a unique ID
    //     this.pictureId = Number(this.generateUniqueId());
    // }

    // private generateUniqueId(): string {
    //     return 'bom-attachment-' + Math.random().toString(36).substr(2, 9); // Simple unique ID generator
    // }
}
    // BOMProcessVersion model
export class BOMProcessVersion extends BaseAuditable {
    id: number;
    bomId: number; // Foreign key to BOM
    createdOn: string; // String to represent date, adjust type as necessary
    createdBy: string; // User ID or reference to user
    version: string; // Version of the BOM
    reasonForChange: string; // Reason for the chang
    uploader: FileUploaderService;  
    pictureId?: number; // Foreign key to PictureEntity
    // bomMaster: BOMMaster; // Reference to BOMMaster entity
    picture: DocumentDetails; 
    isPublish: boolean;
    uniqueId: string;
        
    bomCode: string;  // Add this property
    hideUploader: boolean;               // Flag to hide/show the uploader in the UI
           
    // constructor() {
    //     super();
    // }
}
// Assuming a Product classaa
// export class Product {
//     id: number;
//     productCode: number;
//     name: string;
// }

// // Assuming a Process class
// export class Process {
//     id: number;
//     processCode: number;
//     name: string;
// }
