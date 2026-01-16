import { Component, Output, EventEmitter, ViewChild } from '@angular/core';
// import { SearchInActiveComponent } from '@app-shared';

@Component({
    selector: 'user-search-panel',
    templateUrl: './search-panel.component.html'
})

export class UserSearchPanelComponent {
    // @ViewChild(SearchInActiveComponent) searchInactive: SearchInActiveComponent;

    @Output()
    searchChanged = new EventEmitter();

    searchData: { [key: string]: any } = [];
    searchKey = "userName,firstName,lastNumber,email,phoneNumber,parkingAuthorityName";

    updateSearchTerms(key: string, value) {
        // const isActive: boolean = this.searchInactive.searchBox.value;
        // this.searchData['isActive'] = isActive;

        this.searchData[key] = value;
       // console.log("search data for inactive", this.searchData)
        this.searchChanged.emit(this.searchData);
    }
}