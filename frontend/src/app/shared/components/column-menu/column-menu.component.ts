// column-menu.component.ts
import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule }               from '@angular/common';
import { FormsModule }                from '@angular/forms';
import { MatIconModule }              from '@angular/material/icon';
import { MatMenuModule }              from '@angular/material/menu';
import { MatButtonModule }            from '@angular/material/button';
import { MatCheckboxModule }          from '@angular/material/checkbox';
import { SharedStateService }         from '../../services/shared-state.service';

export interface ColumnState {
  name: string;
  prop: string;
  visible: boolean;
}

@Component({
  standalone: true,
  selector: 'app-column-menu',
  template: `
    <div class="flex items-center px-6">
  <button
    mat-icon-button
    [matMenuTriggerFor]="menu"
    (click)="onMenuOpen()"
  >
    <mat-icon>view_column</mat-icon>
  </button>

  <mat-menu
   #menu="matMenu"
   [overlapTrigger]="false"
   class="rounded-lg"
   (closed)="onMenuClosed()"
 >
    <div class="px-4 py-2 font-semibold border-b">Manage Columns</div>
    <div class="max-h-60 overflow-auto">
      <button
        mat-menu-item
        *ngFor="let col of tempColumns; let i = index"
        (click)="$event.stopPropagation()"
        class="justify-start"
      >
        <mat-checkbox
          [(ngModel)]="tempColumns[i].visible"
          (click)="$event.stopPropagation()"
        >
          {{ col.name }}
        </mat-checkbox>
      </button>
    </div>

  </mat-menu>
</div>
  `,
  styles: [`
    /* no extra deep selectors needed */
  `],
  imports: [
    CommonModule,
    FormsModule,
    MatIconModule,
    MatMenuModule,
    MatButtonModule,
    MatCheckboxModule
  ]
})
export class ColumnMenuComponent implements OnInit{
  @Input() columns: ColumnState[] = [];
  @Output() columnChange = new EventEmitter<ColumnState[]>();
  tempColumns: ColumnState[] = [];

  constructor(private shared: SharedStateService) {}

  ngOnInit() {
    this.resetTemp();
  }

  onMenuOpen() {
    this.resetTemp();
  }

  private resetTemp() {
    this.tempColumns = this.columns.map(c => ({ ...c }));
  }

  onMenuClosed() {
    const changed = this.columns.some((c, i) => c.visible !== this.tempColumns[i].visible);
    if (!changed) return;

    // 1) Commit into service
    this.shared.setColumns(this.tempColumns);
    // 2) Emit for any other listeners
    this.columnChange.emit(this.tempColumns);
  }
}
