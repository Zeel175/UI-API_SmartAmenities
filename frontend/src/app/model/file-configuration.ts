import { UploadType } from "app/enum";
import { DocumentDetails } from "./document-details";

export class FileConfiguration {
    completeAllCallback?: () => void;
    completeCallback?: (response: DocumentDetails) => any;
    type?: string;
    maxAllowedFile?: number;
    addingFileCallback?: () => void;
    onWhenAddingFileFailed?: () => void;
}

export class UploadParameters {
    uploadType: UploadType;
    id: string;
    imageType?: string;
}