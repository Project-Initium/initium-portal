import {ArrayHelpers} from '../../helpers'
import {ToastHelper} from '../../helpers';

export class ValidateDeviceMfa {
    private assertionOptionsUri: string;
    private makeAssertionUri: string;
    private form: HTMLFormElement;
    async init() {
        const contextThis = this;
        this.form = document.getElementById('start-verification') as HTMLFormElement;
        this.assertionOptionsUri = this.form.dataset.assertionOptionsUri;
        this.makeAssertionUri = this.form.dataset.makeAssertionUri;
        this.form.addEventListener('submit', (e) => contextThis.startVerification(e))
    }

    private async startVerification(e: Event) {
        e.preventDefault();
        let makeAssertionOptions;
        try {
            const res = await fetch(this.assertionOptionsUri, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'RequestVerificationToken': this.form.dataset.afToken
                }
            });
            makeAssertionOptions = await res.json();
        } catch (e) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return;
        }

        if (makeAssertionOptions.status !== 'ok') {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return
        }

        const challenge = makeAssertionOptions.challenge.replace(/-/g, '+').replace(/_/g, '/');
        makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

        makeAssertionOptions.allowCredentials.forEach((listItem) => {
            const fixedId = listItem.id.replace(/\_/g, '/').replace(/\-/g, '+');
             listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
        });

        let credential;
        try {
            credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
        } catch (err) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return
        }

        try {
            await this.verifyAssertionWithServer(credential);
        } catch (e) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return
        }
    }

    private async verifyAssertionWithServer(assertedCredential) {

        const authData = new Uint8Array(assertedCredential.response.authenticatorData);
        const clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
        const rawId = new Uint8Array(assertedCredential.rawId);
        const sig = new Uint8Array(assertedCredential.response.signature);
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
            const res = await fetch(this.makeAssertionUri, {
                method: 'POST',
                body: JSON.stringify(data),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.form.dataset.afToken
                }
            });
            response = await res.json();
        } catch (e) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return
        }

        if (response.assertionVerificationResult.status !== 'ok') {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return
        }

        window.location.href = response.url;
    }

    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }
}

const p = new ValidateDeviceMfa();