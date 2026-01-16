import moment from "moment";

export class DateUtility {

    static serverToApp(date: string, format: string = 'MM-DD-YYYY hh:mm:ss'): Date {
        return moment(date, format).toDate();
    }

    static compareDate(fromDate: Date, compareDate: Date, format: string = 'MM-DD-YYYY hh:mm:ss'): boolean {
        var newFromDate = moment(fromDate, format).toDate();
        var compareDate = moment(compareDate, format).toDate();
        if (newFromDate < compareDate) {
            return true;
        }
        return false;
    }
}