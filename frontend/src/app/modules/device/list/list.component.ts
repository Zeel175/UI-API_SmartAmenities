import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { ToastrService } from 'ngx-toastr';
import { DeviceService } from '../device.service';
import { Device } from 'app/model';

@Component({
    selector: 'device-list',
    standalone: true,
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    imports: [
        CommonModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatIconModule,
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatInputModule,
        MatCardModule,
        FormsModule,
        ReactiveFormsModule,
        FuseListComponent,
        ColumnFilterComponent
    ]
})
export class DeviceListComponent implements OnInit {
    deviceData: Device[] = [];
    filteredData: Device[] = [];
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    deviceColumns = [
        { name: 'Device ID', prop: 'deviceId', visible: true },
        { name: 'Device Name', prop: 'displayName', visible: true },
        { name: 'Device Status', prop: 'devStatus', visible: true }
    ];

    constructor(
        private deviceService: DeviceService,
        private shared: SharedStateService,
        private notificationService: ToastrService
    ) {}

    ngOnInit(): void {
        this.shared.setColumns(this.deviceColumns);
        this.getDeviceData();
    }

    private getDeviceData(): void {
        this.loading = true;
        this.deviceService.getDevices().subscribe(
            (result: any) => {
                const items = result?.items ?? result ?? [];
                this.deviceData = (items as Device[]).map((device) => {
                    const deviceId = device.deviceId ?? device.id;
                    const displayName = device.devName ?? device.name ?? device.deviceName ?? deviceId;
                    return {
                        ...device,
                        deviceId,
                        displayName
                    } as Device;
                });
                this.filteredData = this.deviceData;
                this.totalItems = this.deviceData.length;
                this.loading = false;
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load device list.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.deviceData;
            return;
        }
        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();
        this.filteredData = (this.deviceData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
