import { ToastHelper, XhrHelper } from '../../../../../Initium.Portal.Web/Resources/Scripts/helpers';
import { IBasicApiResponse } from '../../../../../Initium.Portal.Web/Resources/Scripts/types/IBasicApiResponse';

export class UserView {
    private enableTenantToggles: NodeListOf<HTMLButtonElement>;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.enableTenantToggles = document.querySelectorAll<HTMLButtonElement>('[data-enable-toggle]');
        if(this.enableTenantToggles) {
            this.enableTenantToggles.forEach((element) => {
                element.addEventListener('click', (event) => contextThis.toggleTenantEnableState(event));
            });
        }
    }

    async toggleTenantEnableState(event: MouseEvent) {
        event.preventDefault();

        this.enableTenantToggles.forEach(ele => ele.disabled = true);
        const btn = event.currentTarget as HTMLButtonElement;
        const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
            btn.dataset.endpoint,
            {
                tenantId: btn.dataset.tenantId
            },
            btn.dataset.afToken
        );

        if (result.isSuccess && result.value.isSuccess) {
            this.enableTenantToggles.forEach(ele => ele.hidden = !ele.hidden);
            ToastHelper.showSuccessToast(btn.dataset.message);
        } else {
            ToastHelper.showFailureToast('Sorry, there was an issue. Please try again.');
        }
        this.enableTenantToggles.forEach(ele => ele.disabled = false);

    }
}
// tslint:disable-next-line:no-unused-expression
new UserView();