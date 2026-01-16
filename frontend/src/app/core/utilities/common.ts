import { CommonConstant } from '../constant';
// import { List } from 'lodash';
import { List } from 'app/model';

export class CommonUtility {

    static isNull(item): boolean {
        return item === undefined || item === null;
    }

    static isEmpty(item): boolean {
        return item === undefined || item === null
            || item.length === 0 || item === 0 || item === '' || item === 'null';
    }

    static isNotNull(item): boolean {
        return item !== undefined && item !== null;
    }

    static isNotEmpty(item): boolean {
        return item !== undefined && item !== null && item.length !== 0;
    }

    static isObjectEmpty(obj): boolean {
        return CommonUtility.isNull(obj) || Object.keys(obj).length === 0 || !Object.keys(obj).some(k => CommonUtility.isNotEmpty(obj[k]));
    }

    static isString(value) {
        return typeof value === 'string' || value instanceof String;
    }

    static splitKeys(keys: string, separator: string = ','): string[] {
        return keys.split(separator);
    }

    static convertObjectToParams(paramObj: object) {
        let params = new URLSearchParams();
        for (let key in paramObj) {
            if (paramObj.hasOwnProperty(key) && paramObj[key]) {
                params.set(key, paramObj[key])
            }
        }
        return params;
    }

    static defaultOp(key, value) {
        const type: string = typeof value;
        switch (type) {
            case 'string':
                return `${key}~${CommonConstant.searchOperators.cn}~`;
            case 'number':
            case 'date':
            case 'boolean':
                return `${key}~${CommonConstant.searchOperators.eq}~`;
            default:
                return key;
        }
    }

    static getFilterObjectToString({ page, pageSize, sort, sortDirection, filter }: any): string {
        let query = `page=${page}&pageSize=${pageSize}`;
        if (sort) {
            query += `&sort=${sort}`;
        }
        if (sortDirection) {
            query += `&sortdirection=${sortDirection}`;
        }
        if (CommonUtility.isObjectEmpty(filter)) {
            return query;
        }
        query += `&filter=`;
        let filterQuery = '';
        Object.keys(filter).forEach(key => {
            if (CommonUtility.isNotEmpty(filter[key])) {
                let keyFields = CommonUtility.splitKeys(key);
                keyFields.forEach(k => {
                    if (filterQuery) {
                        filterQuery += `~or~`;
                    }
                    if (k.indexOf('~') > 0) {
                        filterQuery += k + filter[key];
                    } else {
                        filterQuery += `${CommonUtility.defaultOp(k, filter[key])}${filter[key]}`;
                    }
                });
            }
        });
        return query + filterQuery;
    }

    static getParameter(paramName) {
        var searchString = window.location.search.substring(1),
            i, val, params = searchString.split("&");

        for (i = 0; i < params.length; i++) {
            val = params[i].split("=");
            if (val[0] == paramName) {
                return val[1];
            }
        }
        return null;
    }

    static dateLarger(date1: Date, date2: Date): boolean {
        return new Date(date1) > new Date(date2);
    }

    static getDateDifference(startDate: Date, endDate: Date): number {
        var difference = endDate.getTime() - startDate.getTime();
        return Math.round(difference / 60000);
    }

    static addTimezoneOffset(date: Date): Date {
        var utcDate = new Date(date.toString().substring(4, 15));

        if (utcDate.getTimezoneOffset() < 0) {
            utcDate.setMinutes(utcDate.getMinutes() - utcDate.getTimezoneOffset());
        }
        else {
            utcDate.setMinutes(utcDate.getMinutes() + utcDate.getTimezoneOffset());
        }

        return utcDate;
    }

    static addTimezoneOffset1(date: Date): Date {
        var utcDate = date;

        if (utcDate.getTimezoneOffset() < 0) {
            utcDate.setMinutes(utcDate.getMinutes() - utcDate.getTimezoneOffset());
        }
        else {
            utcDate.setMinutes(utcDate.getMinutes() + utcDate.getTimezoneOffset());
        }

        return utcDate;
    }

    static getSelectedIds(list: List[]) {
        var ids = '';

        list.forEach(item => {
            if (ids === '') {
                ids += item.id.toString();
            }
            else {
                ids += ',' + item.id.toString();
            }
        })

        return ids;
    }

    static round(number, precision) {
        var factor = Math.pow(10, precision);
        var tempNo = number * factor;
        var roundedTempNo = Math.round(tempNo);

        return roundedTempNo / factor;
    }

    static isSmallScreen(): boolean {
        const mq = window.matchMedia("(max-width: 767px)");
        return mq.matches;
    }

    static generateUUID(): string {
        var d = new Date().getTime();//Timestamp
        var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16;//random number between 0 and 16
            if (d > 0) {//Use timestamp until depleted
                r = (d + r) % 16 | 0;
                d = Math.floor(d / 16);
            } else {//Use microseconds since page-load if supported
                r = (d2 + r) % 16 | 0;
                d2 = Math.floor(d2 / 16);
            }
            return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
    }

    static getSelectedCheckboxList(value: any[], list: any[]): any[] {
        return value.map((item, i) => {
            if (item) {
                return list[i]
            }
        }).filter(v => CommonUtility.isNotNull(v));
    }

    static addMinutes(date, minutes): Date {
        return new Date(date.getTime() + minutes * 60000);
    }
}