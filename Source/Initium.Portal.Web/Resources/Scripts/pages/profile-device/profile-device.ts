import { ArrayHelpers, XhrHelper } from '../../helpers';
import { ValidationProvider } from '../../providers';
import 'bootstrap';
import moment from 'moment';
import { IBasicApiResponse } from '../../types/IBasicApiResponse';
import { ToastHelper } from '../../helpers';

interface ICredentialCreateOptionsResult {
    rp: PublicKeyCredentialRpEntity;
    user: {
        name: string;
        id: string;
        displayName: string;
    };
    challenge: string;
    pubKeyCredParams: PublicKeyCredentialParameters[];
    timeout: number;
    attestation: AttestationConveyancePreference;
    authenticatorSelection: AuthenticatorSelectionCriteria;
    excludeCredentials: {
        type: PublicKeyCredentialType;
        id: string;
        transports: AuthenticatorTransport[]
    }[];
    extensions: AuthenticationExtensionsClientInputs;
    status: string;
    errorMessage: string;
}

interface IRegisterNewCredentialResult {
    deviceId: string;
    name: string;
    credentialMakeResult: {
        status: string;
        errorMessage: string;
    };
}


export class ProfileDevice {
    private enrollmentForm: HTMLFormElement;
    private enrollmentFormSlideOut: JQuery<any>;
    private enrollmentValidator: ValidationProvider;
    private tokenType: string;

    private revokeForm: HTMLFormElement;
    private revokeFormSlideOut: JQuery<any>;
    private revokeFormButtons: NodeListOf<HTMLButtonElement>;
    private revokeValidator: ValidationProvider;
    private deviceToRevoke: string;

    private registeredDevices: HTMLFormElement;

    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    init() {
        const contextThis = this;
        this.enrollmentForm = document.querySelector<HTMLFormElement>('#new-device-modal');
        this.enrollmentFormSlideOut = $(this.enrollmentForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.enrollmentValidator = new ValidationProvider(this.enrollmentForm, false);
        this.enrollmentForm.addEventListener('submit', (event: Event) => contextThis.completeEnrollment(event));
        document.querySelectorAll('[data-type]').forEach(value => {
            value.addEventListener('click', (event)=> contextThis.displayEnrollment(event));
        });

        this.revokeForm = document.querySelector<HTMLFormElement>('#revoke-device-modal');
        this.revokeFormSlideOut = $(this.revokeForm).modal({
            show: false,
            backdrop: 'static'
        });
        this.revokeValidator = new ValidationProvider(this.revokeForm, false);
        this.revokeForm.addEventListener('submit', (event: Event) => contextThis.revokeDevice(event));
        this.revokeFormButtons = this.revokeForm.querySelectorAll('button');

        this.registeredDevices = document.getElementById('registered-devices') as HTMLFormElement;
        this.registeredDevices.addEventListener('click', (event) => contextThis.displayRevoke(event));

    }

    private async completeEnrollment(event: Event) {
        event.preventDefault();
        if (this.enrollmentValidator.validate().isValid) {
            const name = this.enrollmentForm.querySelector<HTMLInputElement>('[data-device-name]').value;

            const credentialCreateOptionsResult = await XhrHelper.PostJsonInternalOfT<ICredentialCreateOptionsResult>(
                this.enrollmentForm.dataset.initiateUrl,
                {
                    authenticatorAttachment: this.tokenType
                },
                this.enrollmentForm.dataset.afToken
            );

            if (credentialCreateOptionsResult.isFailure || credentialCreateOptionsResult.value.status !== 'ok') {
                ToastHelper.showFailureToast('There was an issue setting up your device. Please try again');
                return;
            }

            const publicKeyCredentialCreationOptions: PublicKeyCredentialCreationOptions = {
                attestation: credentialCreateOptionsResult.value.attestation,
                authenticatorSelection: credentialCreateOptionsResult.value.authenticatorSelection ?? undefined,
                challenge: ArrayHelpers.coerceToArrayBuffer(credentialCreateOptionsResult.value.challenge),
                excludeCredentials: [],
                extensions: credentialCreateOptionsResult.value.extensions,
                pubKeyCredParams: credentialCreateOptionsResult.value.pubKeyCredParams,
                rp: credentialCreateOptionsResult.value.rp,
                timeout: credentialCreateOptionsResult.value.timeout,
                user: {
                    displayName: credentialCreateOptionsResult.value.user.displayName,
                    name: credentialCreateOptionsResult.value.user.name,
                    id: ArrayHelpers.coerceToArrayBuffer(credentialCreateOptionsResult.value.user.id)
                }
            };

            credentialCreateOptionsResult.value.excludeCredentials.forEach(excludeCredential => {
                publicKeyCredentialCreationOptions.excludeCredentials.push({
                    type: excludeCredential.type,
                    transports: excludeCredential.transports,
                    id: ArrayHelpers.coerceToArrayBuffer(excludeCredential.id)
                });
            });

            const newCredential: any = await navigator.credentials.create({publicKey: publicKeyCredentialCreationOptions});

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

            const registerNewCredentialResult = await XhrHelper.PostJsonInternalOfT<IRegisterNewCredentialResult>(
                this.enrollmentForm.dataset.completeUrl,
                data,
                this.enrollmentForm.dataset.afToken);

            if (registerNewCredentialResult.isFailure || registerNewCredentialResult.value.credentialMakeResult.status !== 'ok') {
                ToastHelper.showFailureToast('There was an issue setting up your device. Please try again');
                return;
            }

            const d = document.createElement('div');
            d.innerHTML = document.getElementById('device-template').innerText;
            d.dataset.deviceId = registerNewCredentialResult.value.deviceId;
            (d.querySelector('[data-device-name]') as HTMLElement).innerText = `${registerNewCredentialResult.value.name}`;
            (d.querySelector('[data-device-remove]') as HTMLButtonElement).value =`${registerNewCredentialResult.value.deviceId}`;
            (d.querySelector('[data-when-enrolled]') as HTMLElement).innerText = moment().format('DD/MM/YYYY');
            document.getElementById('registered-devices').appendChild(d);
            this.enrollmentFormSlideOut.modal('hide');
        }
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
            this.revokeFormButtons.forEach(ele => ele.disabled = true);
            const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
                this.registeredDevices.dataset.endpointUrl,
                {
                    deviceId: this.deviceToRevoke,
                    password: this.revokeForm.querySelector<HTMLInputElement>('[data-device-password]').value
                },
                this.registeredDevices.dataset.afToken);

            if (result.isSuccess && result.value.isSuccess) {
                this.registeredDevices.removeChild(this.registeredDevices.querySelector(`[data-device-id="${this.deviceToRevoke}"]`));
                ToastHelper.showSuccessToast('The device has been revoked.');
                this.revokeFormSlideOut.modal('hide');
            } else {
                ToastHelper.showFailureToast('Sorry, there was an issue revoking the device.  Please try again.');
            }
            this.revokeFormButtons.forEach(ele => ele.disabled = false);
        }
    }
}

// tslint:disable-next-line:no-unused-expression
new ProfileDevice();