import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
// @Component({
//     templateUrl: './role.component.html'
// })
@Component({
    selector: 'role',
    standalone: true,
    templateUrl: './role.component.html',
    imports: [RouterOutlet],
})
export class RoleComponent {
}