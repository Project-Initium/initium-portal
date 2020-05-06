import {ArrayHelpers} from '../helpers/array-helper'
import Swal from 'sweetalert2';
import TriggeredEvent = JQuery.TriggeredEvent;
import {Validator} from "../services/validator";
import 'bootstrap';
import * as moment from 'moment';

export class ProfileDevice {
    private name: HTMLInputElement;
    private form: HTMLFormElement;
    private tokenType: string;
    private validator: Validator;
    private slideOut: JQuery;
    private registeredDevices: HTMLFormElement;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private async submit(event: Event) {
        event.preventDefault();
        if (!this.validator.validate()) {            
            let makeCredentialOptions;
            try {
                makeCredentialOptions = await this.startDeviceRegistration(this.name.value);
            } catch (e) {
                ProfileDevice.showErrorAlert();
                return;
            }

            if (makeCredentialOptions.status !== "ok") {
                ProfileDevice.showErrorAlert();
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
                ProfileDevice.showErrorAlert();
                return
            }

            try {
                this.registerNewCredential(this.name.value, newCredential);
            } catch (e) {
                ProfileDevice.showErrorAlert();
                return;
            }
            
            
        }
    }

    private async startDeviceRegistration(name: string): Promise<PublicKeyCredentialCreationOptions> {
        const response = await fetch(this.form.dataset.initiateUrl, {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            body: JSON.stringify({
                name: name,
                authenticatorAttachment: this.tokenType
            }),
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });
        return await response.json();
    }

    private async registerNewCredential(name: string, newCredential) {
        let attestationObject = new Uint8Array(newCredential.response.attestationObject);
        let clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
        let rawId = new Uint8Array(newCredential.rawId);
    
        const data = {
            name: name,
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
            ProfileDevice.showErrorAlert();
            return;
        }
    
        if (response.result.status !== "ok") {
            ProfileDevice.showErrorAlert();
            return;
        }

        Swal.fire({
            icon: 'success',
            text: 'The device has been registered.',
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        });

        const d = document.createElement('div');
        d.innerHTML = document.getElementById('device-template').innerText;
        d.dataset.deviceId = response.deviceId;
        (d.querySelector('[data-device-name]') as HTMLElement).innerText = `${response.name}`;
        (d.querySelector('[data-device-remove]') as HTMLButtonElement).value =`${response.deviceId}`;
        (d.querySelector('[data-when-enrolled]') as HTMLElement).innerText = moment().format('DD/MM/YYYY');
        document.getElementById('registered-devices').appendChild(d);
        this.slideOut.modal('hide');
    }

    private async registerCredentialWithServer(formData) {
        let response = await fetch(this.form.dataset.completeUrl, {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            body: JSON.stringify(formData),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });
    
        return await response.json();
    }

    private static showErrorAlert() {
        Swal.fire({
            icon: 'error',
            text: 'Sorry, there was an issue processing the device.  It might be already registered.',
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        })
    }
    
    private showPanel(event: Event) {
        this.name.value = '';
        this.tokenType  = (event.target as HTMLButtonElement).dataset.type;
        this.slideOut.modal('show');
    }
    
    private async revokeDevice(event) {
        event.preventDefault();
        var target = event.target as HTMLButtonElement;
        
        try {
            const response =await fetch(this.registeredDevices.dataset.endpointUrl, {
                method: 'POST',
                mode: 'same-origin',
                cache: 'no-cache',
                credentials: 'same-origin',
                body: JSON.stringify({
                    deviceId: target.value,
                }),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            });  
            const data: any = await response.json();
            debugger;
            if (data.isSuccess) {
                this.registeredDevices.removeChild(this.registeredDevices.querySelector(`[data-device-id="${target.value}"]`))
                Swal.fire({
                    icon: 'success',
                    text: 'The device has been revoked.',
                    toast: true,
                    position: "top-end",
                    timer: 4500,
                    showConfirmButton: false
                });
                return ;
            }
            
        } catch(e) {
            
        }

        Swal.fire({
            icon: 'error',
            text: 'Sorry, there was an issue revoking the device.  Please try again.'
        })
        
    }
    
    init() {
        this.form = document.querySelector('form[data-device-registration]') as HTMLFormElement;
        this.validator = new Validator(this.form, false);
        this.name = document.querySelector('input[data-name]') as HTMLInputElement;
        const contextThis = this;

        this.form.addEventListener('submit', (event: Event) => contextThis.submit(event));
        this.slideOut = $('#new-device-model').modal({
            show: false,
        });
        
        document.querySelectorAll('[data-type]').forEach(value => {
            value.addEventListener('click', (event)=> contextThis.showPanel(event))
        });

        this.registeredDevices = document.getElementById('registered-devices') as HTMLFormElement;
        this.registeredDevices.addEventListener('click', (event) => contextThis.revokeDevice(event));
            
    }
    
    
}

new ProfileDevice();