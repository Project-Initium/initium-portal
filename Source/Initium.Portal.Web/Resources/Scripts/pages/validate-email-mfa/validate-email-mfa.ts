import { IBasicApiResponse } from '../../types/IBasicApiResponse';
import { ToastHelper, XhrHelper } from '../../helpers';

export class ValidateEmailMfa {
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        const request = document.querySelector('[data-resend]') as HTMLAnchorElement;
        request.addEventListener('click', (e) => ValidateEmailMfa.resendEmail(e));
    }

    private static async resendEmail(e: MouseEvent) {
        e.preventDefault();
        const link = e.target as HTMLAnchorElement;
        const result = await XhrHelper.PostInternalOfT<IBasicApiResponse>(link.href, link.dataset.afToken);

        if (result.isSuccess && result.value.isSuccess) {
            ToastHelper.showSuccessToast('Email was sent.');
        } else {
            ToastHelper.showFailureToast('There was an issue sending the email.');
        }
    }
}

// tslint:disable-next-line:no-unused-expression
new ValidateEmailMfa();