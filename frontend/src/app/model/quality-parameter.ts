import { FileUploaderService } from '../core/service';
import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class QualityParameter extends BaseAuditable {
    id: number;
    qualityParameterCode: string;
    revisionNo: string;
    drawingNumber: string;
    productId: number;
    approvalStatusId: number;
    qcQuantity: number;
    createdByUser : string;

    qcParameterDetails: QualityParameterDetail[] = [];
    qcParameterDocuments: QualityParameterDocument[] = [];
}

export class QualityParameterDetail  extends BaseAuditable{
    id: number;
    qualityParameterId: number;
    parameterId: number;
    classId: number;
    lslDimensionId: number;
    uslDimensionId: number;
    measuringInstrumentId: number;
    sampleFrequencyId: number;
    reportingFrequencyId: number;
    dimension?: number;
    tolerance?: string;
}

export class QualityParameterDocument  extends BaseAuditable{
    id: number;
    qualityParameterId: number;
    documentName: string;
    pictureId?: number;
    picture: DocumentDetails;
     uploader: FileUploaderService;
    hideUploader: boolean;
    uniqueId: string
}
