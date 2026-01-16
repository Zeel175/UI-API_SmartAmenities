import { Pipe, PipeTransform } from '@angular/core';

/**
 * Convert Object to array of keys.
 */
@Pipe({
    name: 'objToArray'
})
export class ObjectToArrayPipe implements PipeTransform {

    transform(value: {}): string[] {

        if (!value) {
            return [];
        }

        console.log(Object.keys(value));

        return Object.keys(value);
    }
}
