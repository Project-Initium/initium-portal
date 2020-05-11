import {ArrayHelpers} from '../helpers/array-helper'
import Swal  from 'sweetalert2';

export class ValidateDeviceMfa {
    private assertionOptionsUri: string;
    private makeAssertionUri: string;
    async init() {
        const contextThis = this;
        const form = document.getElementById('start-verification');
        this.assertionOptionsUri = form.dataset.assertionOptionsUri;
        this.makeAssertionUri = form.dataset.makeAssertionUri;
        form.addEventListener('submit', (e) => contextThis.startVerification(e))
    }

    private async startVerification(e: Event) {
        e.preventDefault();
        let makeAssertionOptions;
        try {
            const res = await fetch(this.assertionOptionsUri, {
                method: 'POST',            
                headers: {
                    'Accept': 'application/json'
                }
            });
            makeAssertionOptions = await res.json();
        } catch (e) {
            this.showErrorAlert();
            return            
        }
        console.log("Assertion Options Object", makeAssertionOptions);

        if (makeAssertionOptions.status !== "ok") {
            this.showErrorAlert();
            return
        }

        const challenge = makeAssertionOptions.challenge.replace(/-/g, "+").replace(/_/g, "/");
        makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

        makeAssertionOptions.allowCredentials.forEach(function (listItem) {
            const fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
             listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
        });
        
        let credential;
        try {
            credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
        } catch (err) {
            this.showErrorAlert();
            return
        }

        try {
            await this.verifyAssertionWithServer(credential);
        } catch (e) {
            this.showErrorAlert();
            return
        }
    }

    private async verifyAssertionWithServer(assertedCredential) {

        let authData = new Uint8Array(assertedCredential.response.authenticatorData);
        let clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
        let rawId = new Uint8Array(assertedCredential.rawId);
        let sig = new Uint8Array(assertedCredential.response.signature);
        const data = {
            id: assertedCredential.id,
            rawId: ArrayHelpers.coerceToBase64Url(rawId),
            type: assertedCredential.type,
            extensions: assertedCredential.getClientExtensionResults(),
            response: {
                authenticatorData: ArrayHelpers.coerceToBase64Url(authData),
                clientDataJson: ArrayHelpers.coerceToBase64Url(clientDataJSON),
                signature: ArrayHelpers.coerceToBase64Url(sig)
            }            
        };
    
        let response;
        try {
            let res = await fetch(this.makeAssertionUri, {
                method: 'POST',
                body: JSON.stringify(data),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            });
    
            response = await res.json();
        } catch (e) {
            this.showErrorAlert();
            return
        }
    
        if (response.assertionVerificationResult.status !== "ok") {
            this.showErrorAlert();
            return
        }
    
        window.location.href = response.url;
    }
    private showErrorAlert() {
        Swal.fire({
            icon: 'error',
            text: 'Authentication failed, please try again.',
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        });
    }

    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    } 
}

new ValidateDeviceMfa();