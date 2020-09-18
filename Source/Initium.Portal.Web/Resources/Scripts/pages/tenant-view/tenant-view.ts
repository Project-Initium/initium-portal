import {ToastHelper} from '../../helpers';

export class UserView {
    private enableTenantToggles: NodeListOf<HTMLButtonElement>;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.enableTenantToggles = document.querySelectorAll<HTMLButtonElement>('[data-enable-toggle]');
        if(this.enableTenantToggles) {
            this.enableTenantToggles.forEach((element) => {
                element.addEventListener('click', (event) => contextThis.toggleTenantEnableState(event))
            })
        }
    }

    async toggleTenantEnableState(event: MouseEvent) {
        event.preventDefault();
        try {
            const btn = event.currentTarget as HTMLButtonElement;
            const response = await fetch(btn.dataset.endpoint, {
                method: 'POST',
                mode: 'same-origin',
                cache: 'no-cache',
                body: JSON.stringify({
                    tenantId: btn.dataset.tenantId
                }),
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': btn.dataset.afToken
                }
            });
            const result = await response.json();
            if (result && result.isSuccess) {
                this.enableTenantToggles.forEach((element) => {
                    element.classList.toggle('d-none');
                });
                ToastHelper.showSuccessToast(btn.dataset.message);
                return
            }
        }
        catch (e) {
        }
        ToastHelper.showFailureToast('Sorry, there was an issue. Please try again.')
    }
}
const p = new UserView();