import { Pipe, PipeTransform } from '@angular/core';

/**
 * Convert Object to array of keys.
 */
@Pipe({
    name: 'vehicleinfo'
})
export class VehicleInfoPipe implements PipeTransform {

    transform(value: any): string {

        if (!value) {
            return '';
        }
        else {
            var vehicle: string;

            vehicle = `${value.make ? value.make : ''} ${value.model ? value.model : ''} ${value.year ? value.year : ''}`;
            if (value.color) {
                vehicle = vehicle + `(${value.color ? value.color : value.color ? value.color : ''})`;
            }
            return vehicle;
        }
    }
}