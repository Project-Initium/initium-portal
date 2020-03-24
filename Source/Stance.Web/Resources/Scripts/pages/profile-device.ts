import {ArrayHelpers} from '../helpers/array-helper'

export class ProfileDevice {
    private name: HTMLInputElement;
    private form: HTMLFormElement;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private async submit(event: Event) {
        event.preventDefault();
        let makeCredentialOptions;
        try {
            makeCredentialOptions = await this.startDeviceRegistration(this.name.value);
        } catch (e) {
            console.error(e);
        }

        console.log("Credential Options Object", makeCredentialOptions);

        if (makeCredentialOptions.status !== "ok") {
            console.log("Error creating credential options");
            console.log(makeCredentialOptions.errorMessage);
            return;
        }

        // Turn the challenge back into the accepted format of padded base64
        makeCredentialOptions.challenge = ArrayHelpers.coerceToArrayBuffer(makeCredentialOptions.challenge);
        // Turn ID into a UInt8Array Buffer for some reason
        makeCredentialOptions.user.id = ArrayHelpers.coerceToArrayBuffer(makeCredentialOptions.user.id);

        makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
            c.id = ArrayHelpers.coerceToArrayBuffer(c.id);
            return c;
        });

        if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null) {
            makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;
        }

        console.log("Creating PublicKeyCredential...");

        let newCredential;
        try {
            newCredential = await navigator.credentials.create({
                publicKey: makeCredentialOptions
            });
        } catch (e) {
            var msg = "Could not create credentials in browser. Probably because the username is already registered with your authenticator. Please change username or authenticator."
            console.error(msg, e);
        }

        console.log("PublicKeyCredential Created", newCredential);

        try {
            this.registerNewCredential(this.name.value, newCredential);

        } catch (e) {
            //showErrorAlert(err.message ? err.message : err);
        }
    }

    private async startDeviceRegistration(name: string): Promise<PublicKeyCredentialCreationOptions> {
        const response = await fetch('/api/auth-device/initiate-registration', {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });

        const data = await response.json();
        console.log(data);
        return data;
    }

    async registerNewCredential(name: string, newCredential) {
        // Move data into Arrays incase it is super long
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
            //showErrorAlert(e);
        }
    
        console.log("Credential Object", response);
    
        // show error
        if (response.status !== "ok") {
            console.log("Error creating credential");
            console.log(response.errorMessage);
            return;
        }
    }

    async registerCredentialWithServer(formData) {
        let response = await fetch('/api/auth-device/complete-registration', {
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
    
        let data = await response.json();
    
        return data;
    }

    init() {
        this.form = document.querySelector('form[data-device-registration]') as HTMLFormElement;
        this.name = document.querySelector('input[data-name]') as HTMLInputElement;
        const contextThis = this;

        this.form.addEventListener('submit', (event: Event) => contextThis.submit(event));
    }
}

new ProfileDevice();