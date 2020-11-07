import 'bootstrap';
import { ValidationProvider } from '../../providers';
import { ToastHelper, XhrHelper } from '../../helpers';
import { IBasicApiResponse } from '../../types/IBasicApiResponse';

interface IEnrollmentInitResult extends IBasicApiResponse {
    sharedKey: string;
    authenticatorUri: string;
    formattedSharedKey: string;
}

export class ProfileApp {
    private enrollmentForm: HTMLFormElement;
    private enrollmentFormButtons : NodeListOf<HTMLButtonElement>;
    private enrollmentFormSlideOut: JQuery<any>;
    private enrollmentValidator: ValidationProvider;
    private enrollmentButton: HTMLButtonElement;
    private revokeForm: HTMLFormElement;
    private revokeFormSlideOut: JQuery<any>;
    private revokeFormButtons : NodeListOf<HTMLButtonElement>;
    private revokeValidator: ValidationProvider;
    private revokeButton: HTMLButtonElement;
    private sharedKey: string;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.enrollmentForm = document.querySelector<HTMLFormElement>('#enroll-app-modal');
        this.enrollmentFormSlideOut = $(this.enrollmentForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.enrollmentValidator = new ValidationProvider(this.enrollmentForm, false);
        this.enrollmentForm.addEventListener('submit', (event) => contextThis.completeEnrollment(event));
        this.enrollmentFormButtons = this.enrollmentForm.querySelectorAll('button');
        this.enrollmentButton = document.querySelector<HTMLButtonElement>('#setup-app');
        this.enrollmentButton.addEventListener('click', (event) => contextThis.displayEnrollment(event));

        this.revokeForm = document.querySelector<HTMLFormElement>('#revoke-app-modal');
        this.revokeFormSlideOut = $(this.revokeForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.revokeValidator = new ValidationProvider(this.revokeForm, false);
        this.revokeForm.addEventListener('submit', (event) => contextThis.completeRevoke(event));
        this.revokeFormButtons = this.revokeForm.querySelectorAll('button');
        this.revokeButton = document.querySelector<HTMLButtonElement>('#remove-app');
        this.revokeButton.addEventListener('click', (event) => contextThis.displayRevoke(event));
    }

    displayRevoke(event: MouseEvent): any {
        event.preventDefault();
        this.revokeValidator.reset();
        this.revokeFormSlideOut.modal('show');
    }

    async completeRevoke(event: Event) {
        event.preventDefault();
        if (this.revokeValidator.validate().isValid) {
            this.revokeFormButtons.forEach(ele => ele.disabled = true);

            const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
                this.revokeForm.dataset.completeUrl,
                {
                    password: this.revokeForm.querySelector<HTMLInputElement>('[data-app-password]').value
                },
                this.revokeForm.dataset.afToken);
            if (result.isSuccess && result.value.isSuccess) {
                this.enrollmentButton.hidden = !this.enrollmentButton.hidden;
                this.revokeButton.hidden = !this.revokeButton.hidden;
                this.revokeFormSlideOut.modal('hide');
                ToastHelper.showSuccessToast('The app has been revoked');
            } else {
                ToastHelper.showFailureToast('Sorry there was a problem revoking the app. Please try again.');
            }

            this.revokeFormButtons.forEach(ele => ele.disabled = false);
        }
    }

    async completeEnrollment(event: Event) {
        event.preventDefault();
        if (this.enrollmentValidator.validate().isValid) {
            this.enrollmentFormButtons.forEach(ele => ele.disabled = true);

            const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
                this.enrollmentForm.dataset.completeUrl,
                {
                    code: this.enrollmentForm.querySelector<HTMLInputElement>('[data-app-code]').value,
                    sharedKey: this.sharedKey
                },
                this.enrollmentForm.dataset.afToken);

            if (result.isSuccess && result.value.isSuccess) {
                this.enrollmentButton.hidden = !this.enrollmentButton.hidden;
                this.revokeButton.hidden = !this.revokeButton.hidden;
                this.enrollmentFormSlideOut.modal('hide');
                ToastHelper.showSuccessToast('The app has been enrolled');
            } else {
                ToastHelper.showFailureToast('Sorry there was a problem enrolling the app. Please try again.');
            }

            this.enrollmentFormButtons.forEach(ele => ele.disabled = false);
        }
    }

    async displayEnrollment(event: MouseEvent) {
        event.preventDefault();
        this.enrollmentForm.reset();
        this.enrollmentValidator.reset();
        this.enrollmentButton.disabled = true;
        const result = await XhrHelper.PostInternalOfT<IEnrollmentInitResult>(this.enrollmentForm.dataset.initiateUrl, this.enrollmentForm.dataset.afToken);

        if (result.isSuccess && result.value.isSuccess) {
            this.enrollmentForm.querySelector('kbd').innerHTML = result.value.formattedSharedKey;
            const qrCode = this.enrollmentForm.querySelector<HTMLImageElement>('img[data-mfa-image]');
            qrCode.src = qrCode.dataset.mfaImage.replace('__url__', encodeURI(result.value.authenticatorUri)).replace('__nocache__', `${new Date().getTime() / 1000}`);
            this.sharedKey = result.value.sharedKey;
            this.enrollmentFormSlideOut.modal('show');
        } else {
            ToastHelper.showFailureToast('Sorry there was a problem. Please try again.');
        }

        this.enrollmentButton.disabled = false;
    }
}

// tslint:disable-next-line:no-unused-expression
new ProfileApp();