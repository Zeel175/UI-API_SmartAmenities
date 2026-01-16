import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'accountNumber'
})
export class AccountNumberPipe implements PipeTransform {
    transform(value: string): string {
        if (!value) {
            return '';
        }
        else {
            return value.replace(/.(?=.{4,}$)/g, '*')
        }
    }
}