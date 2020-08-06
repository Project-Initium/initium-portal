import {ArrayHelpers} from '../helpers/array-helper'
import Swal from 'sweetalert2';
import {Validator} from '../services/validator';
import 'bootstrap';
import * as moment from 'moment';
import {IBasicApiResponse} from '../types/IBasicApiResponse';

export class ProfileDevice {
    private enrollmentForm: HTMLFormElement;
    private enrollmentFormSlideOut: JQuery<any>;
    private enrollmentValidator: Validator;
    private tokenType: string;

    private revokeForm: HTMLFormElement;
    private revokeFormSlideOut: JQuery<any>;
    private revokeValidator: Validator;
    private deviceToRevoke: string;

    private registeredDevices: HTMLFormElement;

    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    init() {
        const contextThis = this;
        this.enrollmentForm = document.querySelector<HTMLFormElement>('#new-device-modal');
        this.enrollmentFormSlideOut = $(this.enrollmentForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.enrollmentValidator = new Validator(this.enrollmentForm, false);
        this.enrollmentForm.addEventListener('submit', (event: Event) => contextThis.completeEnrollment(event));
        document.querySelectorAll('[data-type]').forEach(value => {
            value.addEventListener('click', (event)=> contextThis.displayEnrollment(event))
        });

        this.revokeForm = document.querySelector<HTMLFormElement>('#revoke-device-modal');
        this.revokeFormSlideOut = $(this.revokeForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.revokeValidator = new Validator(this.revokeForm, false);
        this.revokeForm.addEventListener('submit', (event: Event) => contextThis.revokeDevice(event));

        this.registeredDevices = document.getElementById('registered-devices') as HTMLFormElement;
        this.registeredDevices.addEventListener('click', (event) => contextThis.displayRevoke(event));

    }

    private async completeEnrollment(event: Event) {
        event.preventDefault();
        if (this.enrollmentValidator.validate().isValid) {
            let makeCredentialOptions;
            const name = this.enrollmentForm.querySelector<HTMLInputElement>('[data-device-name]').value;
            try {
                makeCredentialOptions = await this.startDeviceRegistration();
            } catch (e) {
                ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
                return;
            }

            if (makeCredentialOptions.status !== 'ok') {
                ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
                return;
            }

            makeCredentialOptions.challenge = ArrayHelpers.coerceToArrayBuffer(makeCredentialOptions.challenge);
            makeCredentialOptions.user.id = ArrayHelpers.coerceToArrayBuffer(makeCredentialOptions.user.id);

            makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
                c.id = ArrayHelpers.coerceToArrayBuffer(c.id);
                return c;
            });

            if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null) {
                makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;
            }

            let newCredential;
            try {
                newCredential = await navigator.credentials.create({
                    publicKey: makeCredentialOptions
                });
            } catch (e) {
                ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
                return
            }

            try {
                await this.registerNewCredential(name, newCredential);
            } catch (e) {
                ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
                return;
            }
        }
    }

    private async startDeviceRegistration(): Promise<PublicKeyCredentialCreationOptions> {
        const response = await fetch(this.enrollmentForm.dataset.initiateUrl, {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            body: JSON.stringify({
                authenticatorAttachment: this.tokenType
            }),
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.enrollmentForm.dataset.afToken
            }
        });
        return await response.json();
    }

    private async registerNewCredential(name: string, newCredential) {
        const attestationObject = new Uint8Array(newCredential.response.attestationObject);
        const clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
        const rawId = new Uint8Array(newCredential.rawId);

        const data = {
            name,
            attestationResponse: {
               id:  newCredential.id,
                rawId: ArrayHelpers.coerceToBase64Url(rawId),
                type: newCredential.type,
                extensions: newCredential.getClientExtensionResults(),
                response: {
                    AttestationObject: ArrayHelpers.coerceToBase64Url(attestationObject),
                    clientDataJson: ArrayHelpers.coerceToBase64Url(clientDataJSON)
                }
            }
        };

        let response;
        try {
            response = await this.registerCredentialWithServer(data);
        } catch (e) {
            ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
            return;
        }

        if (response.result.status !== 'ok') {
            ProfileDevice.showFailureToast('There was an issue setting up your device. Please try again');
            return;
        }

        ProfileDevice.showSuccessToast('The device has been registered.');

        const d = document.createElement('div');
        d.innerHTML = document.getElementById('device-template').innerText;
        d.dataset.deviceId = response.deviceId;
        (d.querySelector('[data-device-name]') as HTMLElement).innerText = `${response.name}`;
        (d.querySelector('[data-device-remove]') as HTMLButtonElement).value =`${response.deviceId}`;
        (d.querySelector('[data-when-enrolled]') as HTMLElement).innerText = moment().format('DD/MM/YYYY');
        document.getElementById('registered-devices').appendChild(d);
        this.enrollmentFormSlideOut.modal('hide');
    }

    private async registerCredentialWithServer(formData) {
        const response = await fetch(this.enrollmentForm.dataset.completeUrl, {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            body: JSON.stringify(formData),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'RequestVerificationToken': this.enrollmentForm.dataset.afToken
            }
        });

        return await response.json();
    }

    private displayEnrollment(event: Event) {
        event.preventDefault();
        this.enrollmentForm.reset();
        this.enrollmentValidator.reset();
        this.tokenType  = (event.target as HTMLButtonElement).dataset.type;
        this.enrollmentFormSlideOut.modal('show');
    }

    private displayRevoke(event: Event) {
        event.preventDefault();
        this.revokeForm.reset();
        this.revokeValidator.reset();
        this.deviceToRevoke  = (event.target as HTMLButtonElement).value;
        this.revokeFormSlideOut.modal('show');
    }

    private async revokeDevice(event) {
        event.preventDefault();
        if (this.revokeValidator.validate().isValid) {
            try {
                const response = await fetch(this.registeredDevices.dataset.endpointUrl, {
                    method: 'POST',
                    mode: 'same-origin',
                    cache: 'no-cache',
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        deviceId: this.deviceToRevoke,
                        password: this.revokeForm.querySelector<HTMLInputElement>('[data-device-password]').value
                    }),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.registeredDevices.dataset.afToken
                    }
                });
                const data: any = await response.json() as IBasicApiResponse;
                if (data.isSuccess) {
                    this.registeredDevices.removeChild(this.registeredDevices.querySelector(`[data-device-id="${this.deviceToRevoke}"]`));
                    ProfileDevice.showSuccessToast('The device has been revoked.');
                    this.revokeFormSlideOut.modal('hide');
                    return;
                }

            } catch (e) {

            }

            ProfileDevice.showFailureToast('Sorry, there was an issue revoking the device.  Please try again.');
        }
    }

    private static showSuccessToast(message: string) {
        Swal.fire({
            icon: 'success',
            text: message,
            toast: true,
            position: 'top-end',
            timer: 4500,
            showConfirmButton: false
        });
    }

    private static showFailureToast(message: string) {
        Swal.fire({
            icon: 'error',
            text: message,
            toast: true,
            position: 'top-end',
            timer: 4500,
            showConfirmButton: false
        });
    }    
}

new ProfileDevice();