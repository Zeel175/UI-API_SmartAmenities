import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

interface ColumnState {
  name: string;
  prop: string;
  visible: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class SharedStateService {
  private columnsSubject = new BehaviorSubject<ColumnState[]>([]);
  columns$ = this.columnsSubject.asObservable();

  // /** Use this to seed the service with your default columns */
  // initColumns(defaults: ColumnState[]) {
  //   // Only set once if empty
  //   if (this.columnsSubject.getValue().length === 0) {
  //     this.columnsSubject.next(defaults);
  //   }
  // }

  setColumns(columns: ColumnState[]) {
    this.columnsSubject.next(columns);
  }

  getColumns(): ColumnState[] {
    return this.columnsSubject.getValue();
  }
}
