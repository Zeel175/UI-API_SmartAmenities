import { Component, ViewEncapsulation } from '@angular/core';
import { ProjectComponent } from '../dashboards/project/project.component';

@Component({
    selector     : 'example',
    standalone   : true,
    templateUrl  : './example.component.html',
    encapsulation: ViewEncapsulation.None,
   // imports      : [ProjectComponent],
})
export class ExampleComponent
{
    /**
     * Constructor
     */
    constructor()
    {
    }
}
