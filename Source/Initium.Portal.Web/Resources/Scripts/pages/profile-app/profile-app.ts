import 'bootstrap';
import {ValidationProvider} from '../../providers';
import {ToastHelper} from '../../helpers';

export class ProfileApp {
    private enrollmentForm: HTMLFormElement;
    private enrollmentFormSlideOut: JQuery<any>;
    private enrollmentValidator: ValidationProvider;
    private enrollmentButton: HTMLButtonElement;
    private revokeForm: HTMLFormElement;
    private revokeFormSlideOut: JQuery<any>;
    private revokeValidator: ValidationProvider;
    private revokeButton: HTMLButtonElement;
    private sharedKey: string;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
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
        this.enrollmentButton = document.querySelector<HTMLButtonElement>('#setup-app');
        this.enrollmentButton.addEventListener('click', (event) => contextThis.displayEnrollment(event));

        this.revokeForm = document.querySelector<HTMLFormElement>('#revoke-app-modal');
        this.revokeFormSlideOut = $(this.revokeForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.revokeValidator = new ValidationProvider(this.revokeForm, false);
        this.revokeForm.addEventListener('submit', (event) => contextThis.completeRevoke(event));
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
            try {
                const response = await fetch(this.revokeForm.dataset.completeUrl, {
                    method: 'POST',
                    mode: 'same-origin',
                    cache: 'no-cache',
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        password: this.revokeForm.querySelector<HTMLInputElement>('[data-app-password]').value
                    }),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.revokeForm.dataset.afToken
                    }
                });
                const data =  await response.json();
                if (data.isSuccess) {
                    this.enrollmentButton.classList.toggle('d-none');
                    this.revokeButton.classList.toggle('d-none');
                    this.revokeFormSlideOut.modal('hide');
                    ToastHelper.showSuccessToast('The app has been revoked');
                    return;
                }
            } catch (e) {
            }
            ToastHelper.showFailureToast('Sorry there was a problem revoking the app. Please try again.')
        }
    }

    async completeEnrollment(event: Event) {
        event.preventDefault();
        if (this.enrollmentValidator.validate().isValid) {
            try {
                const response = await fetch(this.enrollmentForm.dataset.completeUrl, {
                    method: 'POST',
                    mode: 'same-origin',
                    cache: 'no-cache',
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        code: this.enrollmentForm.querySelector<HTMLInputElement>('[data-app-code]').value,
                        sharedKey: this.sharedKey
                    }),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.enrollmentForm.dataset.afToken
                    }
                });
                const data =  await response.json();
                if (data.isSuccess) {
                    this.enrollmentButton.classList.toggle('d-none');
                    this.revokeButton.classList.toggle('d-none');
                    this.enrollmentFormSlideOut.modal('hide');
                    ToastHelper.showSuccessToast('The app has been enrolled');
                    return;
                }
            } catch (e) {
            }
            ToastHelper.showFailureToast('Sorry there was a problem enrolling the app. Please try again.')
        }
    }

    async displayEnrollment(event: MouseEvent) {
        event.preventDefault();
        this.enrollmentValidator.reset();
        try {
            const response = await fetch(this.enrollmentForm.dataset.initiateUrl, {
                method: 'POST',
                mode: 'same-origin',
                cache: 'no-cache',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.enrollmentForm.dataset.afToken
                }
            });
            const data =  await response.json();
            if (data.isSuccess) {
                this.enrollmentForm.querySelector('kbd').innerHTML = data.formattedSharedKey;
                const qrCode = this.enrollmentForm.querySelector<HTMLImageElement>('img[data-mfa-image]');
                qrCode.src = qrCode.dataset.mfaImage.replace('__url__', encodeURI(data.authenticatorUri)).replace('__nocache__', `${new Date().getTime() / 1000}`);
                this.sharedKey = data.sharedKey;
                this.enrollmentFormSlideOut.modal('show');
                return;
            }
        } catch (e) {
        }
        ToastHelper.showFailureToast('Sorry there was a problem. Please try again.')
    }
}
const p = new ProfileApp();