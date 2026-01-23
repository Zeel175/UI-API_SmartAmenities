import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { AmenitySlotTemplate } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { AmenitySlotTemplateService } from '../amenity-slot-template.service';

@Component({
    selector: 'amenity-slot-template-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    standalone: true,
    imports: [
        CommonModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        RouterModule,
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
export class AmenitySlotTemplateListComponent implements OnInit {
    slotTemplateData: AmenitySlotTemplate[] = [];
    filteredData: AmenitySlotTemplate[] = [];
    page = ApplicationPage.amenitySlotTemplate;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    slotTemplateColumns = [
        { name: 'Amenity', prop: 'amenityName', visible: true },
        { name: 'Day', prop: 'dayOfWeek', visible: true },
        //{ name: 'Slot Times', prop: 'slotTimesSummary', visible: true },
        { name: 'Slot Duration (Min)', prop: 'slotDurationMinutes', visible: true },
        { name: 'Buffer (Min)', prop: 'bufferTimeMinutes', visible: true },
        //{ name: 'Capacity', prop: 'capacitySummary', visible: true },
        //{ name: 'Active', prop: 'isActive', visible: true }
    ];

    constructor(
        private slotTemplateService: AmenitySlotTemplateService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Amenity Slot Template (PER_AMENITY_SLOT_TEMPLATE) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Amenity Slot Template (PER_AMENITY_SLOT_TEMPLATE) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Amenity Slot Template (PER_AMENITY_SLOT_TEMPLATE) - Delete');
        this.getSlotTemplateData();
    }

    private getSlotTemplateData(): void {
        this.loading = true;
        this.slotTemplateService.getSlotTemplates(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                const rows = result.items || result;
                this.slotTemplateData = (rows || []).map((row: any) => ({
                    ...row,
                    slotTimesSummary: this.formatSlotTimes(row?.slotTimes),
                    capacitySummary: this.formatCapacity(row?.slotTimes)
                }));
                this.totalItems = result.totalCount || this.slotTemplateData.length;
                this.filteredData = this.slotTemplateData;
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load slot template data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getSlotTemplateData();
    }

    editTemplate(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteTemplate(id: number): void {
        if (confirm('Delete this slot template?')) {
            this.slotTemplateService.deleteSlotTemplate(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getSlotTemplateData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.slotTemplateData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.slotTemplateData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }

    private formatSlotTimes(slotTimes: any[] | null | undefined): string {
        if (!slotTimes || !slotTimes.length) {
            return '';
        }
        return slotTimes
            .map((slot) => `${slot.startTime} - ${slot.endTime}`)
            .join(', ');
    }

    private formatCapacity(slotTimes: any[] | null | undefined): string {
        if (!slotTimes || !slotTimes.length) {
            return '';
        }
        return slotTimes
            .map((slot) => slot.capacityPerSlot ?? '')
            .filter((value) => value !== '')
            .join(', ');
    }
}
