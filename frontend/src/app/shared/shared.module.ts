import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
// import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import{NgxDatatableModule} from '@swimlane/ngx-datatable';
import {
    ValidationMessage, FormServerErrorComponent, SearchTextComponent, SearchNumberComponent,
    SearchInActiveComponent, ConfirmationBoxComponent, ConfirmationBoxService,
    TruncateTextComponent, MatValidationMessage, AlertBoxComponent
} from './components';
import {
    ClickOutsideDirective, HrefPreventDefaultDirective, ImagePreviewDirective, AuthPermissionDirective,
    ButtonNoDblClickDirective, LimitToDirective, ImageFallbackDirective, LowercaseDirective, SpaceRestrictDirective
} from './directives';
import { SearchTablePipe, ObjectToArrayPipe, SafePipe, HTMLSanitizePipe, AccountNumberPipe, VehicleInfoPipe, CustomDecimalPipe } from './pipes';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxMaskDirective,NgxMaskPipe } from 'ngx-mask';

// import { TruncateCharactersPipe } from 'ng2-truncate';
import { NgSelectModule } from '@ng-select/ng-select';
import { MatModule } from './mat.module';
// import { FlexLayoutModule } from '@angular/flex-layout';
import { PublicService } from 'app/public/public.service';
// import { UserAuthService } from '../core/service';
// import { FileUploadModule } from 'ng2-file-upload';
import { FileUploaderComponent } from './components/file-uploader/file-uploader.component';
import { FilePreviewModalComponent } from './components/file-preview-modal/file-preview-modal.component';
import { FileUploadComponent } from './components/file-upload/file-upload.component';
import { AttachmentComponent } from './components/attachment/attachment.component';
import { SearchComponent } from './components/search/search.component';
import { EmailSenderComponent } from './components/email-sender/email-sender.component';
import { ColumnFilterComponent } from './components/column-filter/column-filter.component';
import { AuthService } from 'app/core/auth/auth.service';
import { ColumnFiltersComponent } from './components/column-filters/column-filters.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { ExcelDownloadComponent } from './components/excel-download/excel-download.component';
//import { ListComponent } from './components/list/list.component';
//import { ExcelDownloadComponent } from './components/excel-download/excel-download.component';
//import { ColumnMenuComponent } from './components/column-menu/column-menu.component';
const sharedModule = [
    CommonModule, FormsModule, ReactiveFormsModule, MatModule,
    RouterModule, NgxDatatableModule, NgSelectModule,NgxMaskDirective,NgxMaskPipe,
    MatTableModule,MatPaginatorModule,MatFormFieldModule,
    MatInputModule,MatIconModule,MatButtonModule,
];

const standaloneDirectives = [
    ClickOutsideDirective, HrefPreventDefaultDirective, ImagePreviewDirective
];

const declaredDirectives = [
    AuthPermissionDirective, ButtonNoDblClickDirective, LimitToDirective,
    ImageFallbackDirective, LowercaseDirective, SpaceRestrictDirective
];


const entryComponents = [ConfirmationBoxComponent, AlertBoxComponent, FilePreviewModalComponent];

const sharedPipes = [
    SearchTablePipe, ObjectToArrayPipe, SafePipe, HTMLSanitizePipe, AccountNumberPipe, VehicleInfoPipe, CustomDecimalPipe,
    // TruncateCharactersPipe
];

const sharedServices = [ConfirmationBoxService, DatePipe, PublicService, AuthService];

const sharedComponents = [
    ValidationMessage, FormServerErrorComponent, SearchTextComponent,
    SearchNumberComponent, SearchInActiveComponent,ColumnFilterComponent,ColumnFiltersComponent,
    TruncateTextComponent, MatValidationMessage, FileUploaderComponent,
    FileUploadComponent, AttachmentComponent, SearchComponent,EmailSenderComponent,
    //ListComponent,
    ExcelDownloadComponent,//ColumnMenuComponent
];

@NgModule({
    imports: [
        ...sharedModule,
        ...standaloneDirectives,
        ...sharedComponents,
        ...sharedPipes,
        ...entryComponents,
         ...declaredDirectives
    ],
    declarations: [
       
    ],   
        ...sharedModule,
        ...standaloneDirectives,
        ...declaredDirectives,
        ...sharedComponents,
        ...sharedPipes,
    providers: [
        ...sharedServices
    ]
})

export class SharedModule { }
