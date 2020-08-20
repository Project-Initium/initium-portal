import {IBasicApiResponse} from '../../types/IBasicApiResponse';
import {ToastHelper} from '../../helpers';

export class ValidateEmailMfa {
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const request = document.querySelector('[data-resend]') as HTMLAnchorElement;
        request.addEventListener('click', (e) => ValidateEmailMfa.resendEmail(e))
    }

    private static async resendEmail(e: MouseEvent) {
        e.preventDefault();
        const link = e.target as HTMLAnchorElement;
        let response: IBasicApiResponse;
        try {
            const res = await fetch(link.href, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': link.dataset.afToken
                }
            });
            response = await res.json();
        } catch (e) {
            response = {
                isSuccess: false
            };
        }
        if (response.isSuccess) {
            ToastHelper.showSuccessToast('Email was sent.');
        } else {
            ToastHelper.showFailureToast('There was an issue sending the email.');
        }
    }
}

const p = new ValidateEmailMfa();