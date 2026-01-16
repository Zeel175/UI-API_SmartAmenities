import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'user',
    standalone: true,
    templateUrl: './user.component.html',
    imports: [RouterOutlet],
})
export class UserComponent { }