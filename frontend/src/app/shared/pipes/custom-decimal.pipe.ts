import { Pipe, PipeTransform } from '@angular/core';
import { DecimalPipe } from '@angular/common';

@Pipe({
  name: 'customDecimal'
})
export class CustomDecimalPipe extends DecimalPipe implements PipeTransform {
  transform(value: any, digitsInfo?: string, locale?: string): any {
    return super.transform(value, digitsInfo || '1.2-2', locale);
  }
}
