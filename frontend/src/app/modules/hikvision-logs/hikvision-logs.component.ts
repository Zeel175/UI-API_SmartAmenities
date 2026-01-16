import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { HikvisionLogsService } from 'app/modules/hikvision-logs/hikvision-logs.service';
import { ResidentMasterService } from 'app/modules/resident-master/resident-master.service';
import { UnitService } from 'app/modules/unit/unit.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';

interface BuildingOption {
    id: number;
    buildingName?: string;
    name?: string;
    code?: string;
}

interface UnitOption {
    id: number;
    unitName?: string;
    name?: string;
    code?: string;
}

interface UserOption {
    id: number;
    fullName?: string;
    firstName?: string;
    lastName?: string;
    userName?: string;
    name?: string;
}

interface HikvisionLogEntry {
    major?: number;
    minor?: number;
    time?: string;
    cardNo?: string;
    name?: string;
    cardReaderNo?: number;
    doorNo?: number;
    employeeNoString?: string;
    serialNo?: number;
    userType?: string;
    currentVerifyMode?: string;
    mask?: string;
}

@Component({
    selector: 'hikvision-logs',
    standalone: true,
    templateUrl: './hikvision-logs.component.html',
    styleUrls: ['./hikvision-logs.component.scss'],
    providers: [DatePipe],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatPaginatorModule,
        MatIconModule,
        MatDatepickerModule,
        MatNativeDateModule,
        NgxMaterialTimepickerModule,
        FuseListComponent,
        ColumnFilterComponent
    ]
})
export class HikvisionLogsComponent implements OnInit {
    readonly allUsersValue = 'all';
    form: FormGroup;
    previewData: {
        startDateTime: string;
        endDateTime: string;
        building: string;
        unit: string;
        user: string;
    } | null = null;

    buildingOptions: BuildingOption[] = [];
    unitOptions: UnitOption[] = [];
    userOptions: UserOption[] = [];
    logData: HikvisionLogEntry[] = [];
    filteredLogData: HikvisionLogEntry[] = [];
    logLoading = false;
    logPageIndex = 0;
    logPageSize = 10;
    logTotalItems = 0;
    logColumns = [
        { name: 'Employee No', prop: 'employeeNoString', visible: true },
        { name: 'Name', prop: 'name', visible: true },
        { name: 'User Type', prop: 'userType', visible: true },
        //{ name: 'Major', prop: 'major', visible: true },
        //{ name: 'Minor', prop: 'minor', visible: true },
        { name: 'Time', prop: 'time', visible: true },
        { name: 'Verify Mode', prop: 'currentVerifyMode', visible: true },
        //{ name: 'Mask', prop: 'mask', visible: true },
        { name: 'Card No', prop: 'cardNo', visible: true },
        //{ name: 'Card Reader No', prop: 'cardReaderNo', visible: true },
        { name: 'Door No', prop: 'doorNo', visible: true },
        //{ name: 'Serial No', prop: 'serialNo', visible: true },
        
        
    ];

    constructor(
        private fb: FormBuilder,
        private hikvisionLogsService: HikvisionLogsService,
        private unitService: UnitService,
        private residentMasterService: ResidentMasterService,
        private datePipe: DatePipe
    ) {
        this.form = this.fb.group({
            startDate: [null, Validators.required],
            startTime: ['', Validators.required],
            endDate: [null, Validators.required],
            endTime: ['', Validators.required],
            buildingId: ['', Validators.required],
            unitId: ['', Validators.required],
            userIds: [[], Validators.required]
        });
    }

    ngOnInit(): void {
        this.setDefaultStartDateTime();
        this.loadBuildings();
    }

    onBuildingChange(buildingId: number): void {
        this.unitOptions = [];
        this.userOptions = [];
        this.form.patchValue({ unitId: '', userIds: [] });

        if (!buildingId) {
            return;
        }

        this.unitService.getUnitsByBuilding(buildingId).subscribe((units: any) => {
            this.unitOptions = units?.items || units || [];
        });
    }

    onUnitChange(unitId: number): void {
        this.userOptions = [];
        this.form.patchValue({ userIds: [] });

        if (!unitId) {
            return;
        }

        this.residentMasterService.getUsersByUnit(unitId).subscribe((users: any) => {
            this.userOptions = users?.items || users || [];
        });
    }

    private loadBuildings(): void {
        this.unitService.getBuildings().subscribe((buildings: any) => {
            this.buildingOptions = buildings?.items || buildings || [];
        });
    }

    private setDefaultStartDateTime(): void {
        const now = new Date();
        const formattedTime = this.datePipe.transform(now, 'h:mm a') ?? '';

        this.form.patchValue({
            startDate: now,
            startTime: formattedTime
        });
    }

    onUserSelectionChange(selectedValues: Array<number | string>): void {
        if (!selectedValues?.length) {
            return;
        }

        if (selectedValues.includes(this.allUsersValue)) {
            const allUserIds = this.userOptions.map(user => user.id);
            this.form.patchValue({ userIds: allUserIds }, { emitEvent: false });
        }
    }

    areAllUsersSelected(): boolean {
        const selectedIds = this.form.get('userIds')?.value as number[];
        return !!selectedIds?.length && selectedIds.length === this.userOptions.length;
    }

    applyFilters(): void {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        const { startDate, startTime, endDate, endTime, buildingId, unitId, userIds } = this.form.value;
        const startDateTime = this.buildDateTime(startDate, startTime);
        const endDateTime = this.buildDateTime(endDate, endTime);

        const endTimeControl = this.form.get('endTime');
        const existingErrors = { ...(endTimeControl?.errors || {}) };

        if (!startDateTime || !endDateTime || startDateTime >= endDateTime) {
            endTimeControl?.setErrors({ ...existingErrors, endBeforeStart: true });
            endTimeControl?.markAsTouched();
            return;
        }

        // Clear only the custom error
        if (existingErrors['endBeforeStart']) delete existingErrors['endBeforeStart'];
        endTimeControl?.setErrors(Object.keys(existingErrors).length ? existingErrors : null);

        const building = this.buildingOptions.find(item => item.id === buildingId);
        const unit = this.unitOptions.find(item => item.id === unitId);
        const selectedUsers = this.userOptions.filter(user => userIds?.includes(user.id));

        this.logLoading = true;
        this.hikvisionLogsService
            .searchLogs({
                start: startDateTime.toISOString(),
                end: endDateTime.toISOString(),
                buildingId,
                unitId,
                userIds
            })
            .subscribe((result: any) => {
                this.previewData = {
                    startDateTime: this.datePipe.transform(startDateTime, 'dd-MM-yyyy HH:mm') ?? '',
                    endDateTime: this.datePipe.transform(endDateTime, 'dd-MM-yyyy HH:mm') ?? '',
                    building: building?.buildingName || building?.name || building?.code || '',
                    unit: unit?.unitName || unit?.name || unit?.code || '',
                    user: this.getSelectedUsersDisplay(selectedUsers)
                };
                const items = result?.data || result?.items || result || [];
                this.logData = (items || []).map((entry: HikvisionLogEntry) => ({
                    ...entry,
                    time: this.formatLogTime(entry?.time),
                    currentVerifyMode: this.getVerifyMode(entry?.mask)
                }));
                this.logTotalItems = result?.total || result?.totalCount || this.logData.length;
                this.logLoading = false;
                this.applyLogFilters();
            }, () => {
                this.logLoading = false;
            });
    }

    private getUserDisplayName(user?: UserOption): string {
        if (!user) {
            return '';
        }

        if (user.fullName) {
            return user.fullName;
        }

        const fullName = [user.firstName, user.lastName].filter(Boolean).join(' ').trim();
        if (fullName) {
            return fullName;
        }

        return user.userName || user.name || '';
    }

    private getSelectedUsersDisplay(users: UserOption[]): string {
        if (!users?.length) {
            return '';
        }

        if (users.length === this.userOptions.length) {
            return 'All Users';
        }

        return users.map(user => this.getUserDisplayName(user)).filter(Boolean).join(', ');
    }

    private buildDateTime(date: Date | null, time: string): Date | null {
        if (!date || !time) {
            return null;
        }

        // ngx-material-timepicker returns values like "9:00 AM" by default.
        // Support both "HH:mm" (24h) and "h:mm AM/PM" (12h) formats.
        const parsed = this.parseTimeString(time);
        if (!parsed) {
            return null;
        }

        const combined = new Date(date);
        combined.setHours(parsed.hours, parsed.minutes, 0, 0);
        return Number.isFinite(combined.getTime()) ? combined : null;
    }
    private parseTimeString(time: string): { hours: number; minutes: number } | null {
        const value = (time || '').trim();
        if (!value) return null;

        // Matches:
        //  - 09:30
        //  - 9:30
        //  - 9:30 AM
        //  - 9:30PM
        //  - 9 AM
        const match = value.match(/^\s*(\d{1,2})(?::(\d{1,2}))?\s*(AM|PM)?\s*$/i);
        if (!match) return null;

        let hours = Number(match[1]);
        const minutes = Number(match[2] ?? '0');
        const meridiem = (match[3] ?? '').toUpperCase();

        if (!Number.isFinite(hours) || !Number.isFinite(minutes)) return null;
        if (minutes < 0 || minutes > 59) return null;

        // Convert 12h to 24h
        if (meridiem === 'AM' || meridiem === 'PM') {
            if (hours < 1 || hours > 12) return null;

            if (hours === 12) hours = (meridiem === 'AM') ? 0 : 12;
            else if (meridiem === 'PM') hours += 12;
        } else {
            // 24h style input
            if (hours < 0 || hours > 23) return null;
        }

        return { hours, minutes };
    }
    private formatLogTime(value: string | number | undefined): string {
        if (value == null || value === '') {
            return '';
        }

        let timestamp: number | null = null;

        if (typeof value === 'number') {
            timestamp = value;
        } else if (/^\d+$/.test(value)) {
            timestamp = Number(value);
        }

        if (timestamp != null) {
            if (value.toString().length <= 10) {
                timestamp *= 1000;
            }
            const date = new Date(timestamp);
            if (Number.isFinite(date.getTime())) {
                return this.datePipe.transform(date, 'dd-MM-yyyy HH:mm:ss') ?? value.toString();
            }
            return value.toString();
        }

        const date = new Date(value);
        if (!Number.isFinite(date.getTime())) {
            return value.toString();
        }

        return this.datePipe.transform(date, 'dd-MM-yyyy HH:mm:ss') ?? value.toString();
    }

    private getVerifyMode(mask: string | undefined): string {
        const normalized = (mask ?? '').toString().trim().toLowerCase();
        return normalized === 'no'
            ? 'Authenticated via Face'
            : 'Authenticated via Fingerprint';
    }

    applyLogFilters(): void {
        this.filteredLogData = this.logData;
    }

    onLogColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredLogData = this.logData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredLogData = (this.logData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }

    onLogPageChange(evt: PageEvent): void {
        this.logPageIndex = evt.pageIndex;
        this.logPageSize = evt.pageSize;
    }
}
