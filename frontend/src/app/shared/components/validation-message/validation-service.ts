import { AbstractControl, FormControl } from "@angular/forms";

export class ValidationService {
    static getValidatorErrorMessage(validatorName: string, validatorValue?: any, message?: string) {
        let config = {
            'required': `${message} Is Required`,
            'invalidEmailAddress': 'Invalid Email Address',
            'invalidWebsite': 'Invalid Website',
            'minlength': `Minimum ${validatorValue.requiredLength} Characters Required`,
            'mismatchedPassword': 'Please Enter the Same Password as Above',
            'mismatchedEmail': 'Email Is Not Matching',
            'onlyNumber': 'Only Numbers Are Allowed',
            'min': `Minimum Value ${validatorValue.min}`,
            'max': `Maximum Value ${validatorValue.max}`,
            'minSelection': `Atleast Select ${validatorValue.min} ${message}`,
            'invalidPhoneNumber': ' Invalid Phone Number',
            'expDate': 'Invalid Expiry Date',
            'ccNumber': 'Invalid Card Number',
            'maxlength': `Maximum Length ${validatorValue.requiredLength}`,
            'invalidPassword': `Password Must Be Minimum 6 Characters Long with At least 1 Upper Case, 1 Lower Case, 1 Special Character and 1 Digit.`,
            'mismatchedDocument': 'Driver License Number Is Not Matching',
            'mismatchedAccount': 'Account Number Is Not Matching',
            'routingNumberLength': 'Routing# Length Must Be 9 Digits',
            'routingNumberCheckSum': 'Incorrect Routing#',
            'passwordMinLength': 'Password Must Be At Least 6 Characters Long'
        };

        return config[validatorName];
    }

    static emailValidator(control: any) {
        // RFC 2822 compliant regex
        if (!control.value || control.value.match(/[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?/)) {
            return null;
        } else {
            return { 'invalidEmailAddress': true };
        }
    }

    static websiteValidator(control: any) {
        if (!control.value || control.value.match(/^((https?|ftp|smtp):\/\/)?(www.)?[a-z0-9]+\.[a-z]+(\/[a-zA-Z0-9#]+\/?)*$/)) {
            return null;
        } else {
            return { 'invalidWebsite': true };
        }
    }

    static passwordValidator(control: any) {
        if (!control.value || control.value.match('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{5,}')) {
            return null;
        } else {
            return { 'invalidPassword': true };
        }
    }

    static passwordMinLengthValidator(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }

        const exactMatch = control.root.controls.password.value.length > 6;
        return exactMatch ? null : { passwordMinLength: true };
    }

    static compareNewPassword(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }
        const exactMatch = control.root.controls.newPassword.value === control.value;
        return exactMatch ? null : { mismatchedPassword: true };
    }

    static comparePassword(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }
        const exactMatch = control.root.controls.password.value === control.value;
        return exactMatch ? null : { mismatchedPassword: true };
    }

    static compareEmail(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }
        const exactMatch = control.root.controls.email.value === control.value;
        return exactMatch ? null : { mismatchedEmail: true };
    }

    static allowOnlyNumber(control: any) {
        if (control.value) {
            let tokenValue: number = control.value.toString().trim();
            if (!isNaN(tokenValue)) {
                return null;
            }
            return { 'onlyNumber': true };
        }
        else {
            return null;
        }
    }

    static minSelectedCheckboxes(min: number) {
        return (control: AbstractControl) => {
            const totalSelected = (control.value as any[]).filter((x) => x);

            return totalSelected.length >= min ? null : { minSelection: { valid: false, min: min } };
        }
    }

    static phoneNoValidator(control: any) {
        // RFC 2822 compliant regex
        if (!control.value || control.value.match(/^\(\d{3}\)\s\d{3}-\d{4}$/)) {
            return null;
        } else {
            return { 'invalidPhoneNumber': true };
        }
    }

    static compareDocumentNumber(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }
        const exactMatch = control.root.controls.documentNumber.value === control.value;
        return exactMatch ? null : { mismatchedDocument: true };
    }

    static compareAccountNumber(control: any) {
        if (!control.root || !control.root.controls) {
            return null;
        }
        const exactMatch = control.root.controls.accountNumber.value === control.value;
        return exactMatch ? null : { mismatchedAccount: true };
    }

    static multipleemailrestrictValidator(control) {
        // RFC 2822 compliant regex
        if (!control.value || control.value.match(/^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/)) {
            return null;
        } else {
            return { 'invalidEmailAddress': true };
        }
    }
}