import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  @Input() placeholder: string = '';
  @Output() search = new EventEmitter<string>();
  searchText: string = '';

  onSearch(): void {
      this.search.emit(this.searchText);
  }
    clearSearch(): void {
    this.searchText = '';
    this.onSearch(); // Emit the cleared value
  }
}
