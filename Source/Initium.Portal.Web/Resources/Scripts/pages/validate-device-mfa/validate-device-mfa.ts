import { ArrayHelpers, ToastHelper, XhrHelper } from '../../helpers';

interface IAssertionOptionsResult {
    challenge: string;
    timeout: number;
    rpId: string;
    allowCredentials: {
        id: string;
        transports?: AuthenticatorTransport[];
        type: PublicKeyCredentialType;
    }[];
    userVerification?: UserVerificationRequirement;
    extensions: AuthenticationExtensionsClientInputs;
    status: string;
    errorMessage: string;
}

interface IAssertionVerificationResult {
    url: string;
    assertionVerificationResult: {
        credentialId: string;
        counter: number
        status: string;
        errorMessage: string;
    };
}

export class ValidateDeviceMfa {
    private assertionOptionsUri: string;
    private makeAssertionUri: string;
    private form: HTMLFormElement;
    private formButtons: NodeListOf<HTMLButtonElement>;

    async init() {
        const contextThis = this;
        this.form = document.getElementById('start-verification') as HTMLFormElement;
        this.assertionOptionsUri = this.form.dataset.assertionOptionsUri;
        this.makeAssertionUri = this.form.dataset.makeAssertionUri;
        this.form.addEventListener('submit', (e) => contextThis.startVerification(e));
        this.formButtons = this.form.querySelectorAll('button');
    }

    private async startVerification(e: Event) {
        e.preventDefault();
        this.formButtons.forEach(ele => ele.disabled = true);
        const makeAssertionOptionsResult = await XhrHelper.PostInternalOfT<IAssertionOptionsResult>(this.assertionOptionsUri, this.form.dataset.afToken);

        if (makeAssertionOptionsResult.isFailure) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            this.formButtons.forEach(ele => ele.disabled = false);
            return;
        }

        const makeAssertionOptions = makeAssertionOptionsResult.value;
        if (makeAssertionOptions.status !== 'ok') {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            this.formButtons.forEach(ele => ele.disabled = false);
            return;
        }


        const challenge = makeAssertionOptions.challenge.replace(/-/g, '+').replace(/_/g, '/');

        const publicKeyCredentialRequestOptions: PublicKeyCredentialRequestOptions = {
            userVerification: makeAssertionOptions.userVerification,
            rpId: makeAssertionOptions.rpId,
            extensions: makeAssertionOptions.extensions,
            timeout: makeAssertionOptions.timeout,
            challenge: Uint8Array.from(atob(challenge), c => c.charCodeAt(0)),
            allowCredentials: []
        };

        makeAssertionOptions.allowCredentials.forEach((listItem) => {
            const fixedId = (listItem.id as unknown as string).replace(/\_/g, '/').replace(/\-/g, '+');
            const publicKeyCredentialDescriptor: PublicKeyCredentialDescriptor = {
                transports: listItem.transports,
                type: listItem.type,
                id: Uint8Array.from(atob(fixedId), c => c.charCodeAt(0))
            };

            publicKeyCredentialRequestOptions.allowCredentials.push(publicKeyCredentialDescriptor);
        });

        let credential;
        try {
            credential = await navigator.credentials.get({ publicKey: publicKeyCredentialRequestOptions });
        } catch (err) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            this.formButtons.forEach(ele => ele.disabled = false);
            return;
        }

        await this.verifyAssertionWithServer(credential);
        this.formButtons.forEach(ele => ele.disabled = false);
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

        const result = await XhrHelper.PostJsonInternalOfT<IAssertionVerificationResult>(this.makeAssertionUri, data, this.form.dataset.afToken);

        if (result.isFailure) {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return;
        }

        if (result.value.assertionVerificationResult.status !== 'ok') {
            ToastHelper.showFailureToast('Authentication failed, please try again.');
            return;
        }

        window.location.href = result.value.url;
    }

    constructor() {
        if (document.readyState !== 'loading') {
            this.init().then();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }
}


// tslint:disable-next-line:no-unused-expression
new ValidateDeviceMfa();