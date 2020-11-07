import { ToastHelper, XhrHelper } from '../../helpers';
import { IBasicApiResponse } from '../../types/IBasicApiResponse';

export class UserView {
    private unlockForm: HTMLFormElement;
    private unlockFormButtons: NodeListOf<HTMLButtonElement>;
    private enableAccountToggles: NodeListOf<HTMLButtonElement>;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.unlockForm = document.querySelector<HTMLFormElement>('#unlock-form');
        if (this.unlockForm) {
            this.unlockForm.addEventListener('submit', (event) => contextThis.unlockUser(event));
            this.unlockFormButtons = this.unlockForm.querySelectorAll('button');
        }
        this.enableAccountToggles = document.querySelectorAll<HTMLButtonElement>('[data-enable-toggle]');
        if(this.enableAccountToggles) {
            this.enableAccountToggles.forEach((element) => {
                element.addEventListener('click', (event) => contextThis.toggleAccountEnableState(event));
            });
        }
    }

    async toggleAccountEnableState(event: MouseEvent) {
        event.preventDefault();

        this.enableAccountToggles.forEach(ele => ele.disabled = true);

        const btn = event.currentTarget as HTMLButtonElement;
        const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
            btn.dataset.endpoint,
            {
                userId: btn.dataset.userId
            },
            btn.dataset.afToken);
       if (result.isSuccess && result.value.isSuccess) {
            this.enableAccountToggles.forEach(ele => ele.hidden = !ele.hidden);
            ToastHelper.showSuccessToast(btn.dataset.message);
       } else {
           ToastHelper.showFailureToast('Sorry, there was an issue. Please try again.');
       }

        this.enableAccountToggles.forEach(ele => ele.disabled = false);
    }

    async unlockUser(event: Event) {
        event.preventDefault();
        this.unlockFormButtons.forEach(ele => ele.disabled = true);
        const result = await XhrHelper.PostJsonInternalOfT<IBasicApiResponse>(
            this.unlockForm.dataset.endpoint,
            {
                userId: this.unlockForm.dataset.userId
            },
            this.unlockForm.dataset.arToken
        );

        if (result.isSuccess && result.value.isSuccess) {
            ToastHelper.showSuccessToast('The user has been unlocked.');
            this.unlockForm.parentElement.removeChild(this.unlockForm);
            return;
        }
        else {
            ToastHelper.showFailureToast('Sorry, there was an issue unlocking this user. Please try again.');
            this.unlockFormButtons.forEach(ele => ele.disabled = false);
        }
    }
}
// tslint:disable-next-line:no-unused-expression
new UserView();