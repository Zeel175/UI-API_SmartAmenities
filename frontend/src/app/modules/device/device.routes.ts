import { Routes } from '@angular/router';
import { DeviceListComponent } from './list/list.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: DeviceListComponent }
        ]
    }
] as Routes;
