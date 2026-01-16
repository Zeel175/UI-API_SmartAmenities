import { FileUploader } from "ng2-file-upload";
import { APIConstant } from "../constant";
import { CommonConstant } from "../constant";
import { CommonUtility } from "../utilities";
//import { FileConfiguration, UploadParameters } from "app/models";
import { Injectable } from "@angular/core";
import { FileConfiguration, UploadParameters } from "app/model/file-configuration";

export enum FileType {
    image = 'image',
    pdf = 'pdf',
    word = 'word'
}

const types: { [key: string]: string[] } = {
    [FileType.image]: ['image/png', 'image/jpg', 'image/jpeg'],
    [FileType.pdf]: ['application/pdf'],
    [FileType.word]: ['application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'],
}

@Injectable( {providedIn: 'root' })
export class FileUploaderService {

    uploader: FileUploader;

    //completeAllCallback?: Function, type?: string, maxAllowedFile?: number
    constructor(options: FileConfiguration) {
        this.uploader = new FileUploader({
            url: APIConstant.upload,
            authToken: `Bearer ${window.localStorage.getItem(CommonConstant.token)}`,
            //queueLimit: maxAllowedFile || 50
            maxFileSize: 5 * 1024 * 1024,
            allowedMimeType: ['image/png', 'image/jpg', 'image/jpeg', 'application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document']
        });

        this.uploader.onWhenAddingFileFailed = (item) => {
            this.uploader.onBeforeUploadItem = (item) => {
                item.withCredentials = false;
            }

            if (options.onWhenAddingFileFailed) {
                options.onWhenAddingFileFailed();
            }
        }

        this.uploader.onBeforeUploadItem = (item) => {
            item.withCredentials = false;
        }

        if (options.type) {
            this.uploader.options.allowedMimeType = types[options.type];
        }

        if (options.completeAllCallback) {
            this.uploader.onCompleteAll = () => {
                options.completeAllCallback();
            };
        }

        if (options.completeCallback) {
            this.uploader.onCompleteItem = (item, response, status, header) => {
                if (status === 200) {
                    options.completeCallback(JSON.parse(response));
                } else {
                    options.completeCallback(null);
                }
            }
        }
        // console.log('--Before Queue--');
        
        if (options.maxAllowedFile) {
        // console.log('--Inside Queue--');
        // console.log('--options.maxAllowedFile--', options.maxAllowedFile);

            this.uploader.onAfterAddingFile = (f) => {
                if (this.uploader.queue.length > options.maxAllowedFile) {
                  // console.log('--Filename--', this.uploader.queue[0]);
                    this.uploader.removeFromQueue(this.uploader.queue[0]);
                }
                if (options.addingFileCallback) {
                    // console.log('--Inside options.addingFileCallbac--');
                    options.addingFileCallback();
                }
            };
        }
    }

    uploadFiles(parameters: UploadParameters): void {
        // console.log('uploadFiles called with parameters:', parameters);  
        let url = `${this.uploader.options.url}`;
        Object.keys(parameters).forEach(key => {
            url += `/${parameters[key]}`;
        });

        this.uploader.setOptions({ url });
        // console.log('--uploadAll--', url);

        this.uploader.uploadAll();
    }

    hasFile(): boolean {
        return !CommonUtility.isEmpty(this.uploader.queue);
    }
}