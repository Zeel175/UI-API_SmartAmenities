import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class StockLedger extends BaseAuditable {
    id: number;
    stockLedgerCode: string;
    revisionNo: string;
    warehouseId: number;
    fromWarehouseName: string;
    productId: number;
    transactionDate: Date;
    transactionType: string; // For example: 'In', 'Out', 'Transfer'
    quantity: number;
    uomId: number;
    uomName: string;
    balanceQuantity?: number;
    remarks?: string;

    // Nested arrays for transaction details and attachments
    stockLedgerTransactionDetails: StockLedgerTransactionDetails[] = [];
    stockLedgerAttachments: StockLedgerAttachments[] = [];
    status: string;
}

export class StockLedgerTransactionDetails extends BaseAuditable {
    id: number;
    stockLedgerId: number;  // Ensure this is optional if not always present
    productId: number;
    warehouseId: number;
    transactionType: string; // 'In', 'Out', or 'Transfer'
    qty: number;
    uomId: number;
    uomName: string;
    remarks?: string;
    transactionDate: Date;
    balanceQty: number;
}

export class StockLedgerAttachments extends BaseAuditable {
    id?: number;
    stockLedgerId?: number;
    documentName: string;
    pictureId?: number;
    picture: DocumentDetails;
    hideUploader: boolean;
    uniqueId: string;
}
