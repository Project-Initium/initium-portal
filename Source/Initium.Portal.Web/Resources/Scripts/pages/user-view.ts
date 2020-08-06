import Swal from "sweetalert2";

export class UserView {
    private unlockForm: HTMLFormElement;
    private enableAccountToggles: NodeListOf<HTMLButtonElement>;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.unlockForm = document.querySelector<HTMLFormElement>('#unlock-form');
        if (this.unlockForm) {
            this.unlockForm.addEventListener('submit', (event) => contextThis.unlockUser(event));
        }
        this.enableAccountToggles = document.querySelectorAll<HTMLButtonElement>('[data-enable-toggle]');
        if(this.enableAccountToggles) {
            this.enableAccountToggles.forEach((element) => {
                element.addEventListener('click', (event) => contextThis.toggleAccountEnableState(event))
            })
        }
    }

    async toggleAccountEnableState(event: MouseEvent) {
        event.preventDefault();
        try {
            const btn = event.currentTarget as HTMLButtonElement;
            const response = await fetch(btn.dataset.endpoint, {
                method: 'POST',
                mode: 'same-origin',
                cache: 'no-cache',
                body: JSON.stringify({
                    userId: btn.dataset.userId
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
                this.enableAccountToggles.forEach((element) => {
                    element.classList.toggle('d-none');
                });
                UserView.showSuccessToast(btn.dataset.message);
                return
            }
        }
        catch (e) {
            
        }
        UserView.showFailureToast('Sorry, there was an issue. Please try again.')
        
    }

    async unlockUser(event: Event) {
        event.preventDefault();
        try {
            const response = await fetch(this.unlockForm.dataset.endpoint, {
                method: 'POST',
                mode: 'same-origin',
                cache: 'no-cache',
                body: JSON.stringify({
                    userId: this.unlockForm.dataset.userId
                }),
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.unlockForm.dataset.arToken
                }
            });
            const result = await response.json();
            if (result && result.isSuccess) {
                UserView.showSuccessToast('The user has been unlocked.');
                this.unlockForm.parentElement.removeChild(this.unlockForm);
                return                
            }        
            
        } catch (e) {
            
        }
        UserView.showFailureToast('Sorry, there was an issue unlocking this user. Please try again.')
    }

    private static showSuccessToast(message: string) {
        Swal.fire({
            icon: 'success',
            text: message,
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        });
    }

    private static showFailureToast(message: string) {
        Swal.fire({
            icon: 'error',
            text: message,
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        });
    }
}
new UserView();