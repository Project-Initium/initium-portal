import Swal from "sweetalert2";

export class UserView {
    private unlockForm: HTMLFormElement;
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
                    'Content-Type': 'application/json'
                }
            });
            const result = await response.json();
            if (result && result.isSuccess) {
                Swal.fire({
                    icon: 'success',
                    text: 'The user has been unlocked.',
                    toast: true,
                    position: "top-end",
                    timer: 4500,
                    showConfirmButton: false
                });
                this.unlockForm.parentElement.removeChild(this.unlockForm)
                return                
            }
            
            
        } catch (e) {
            
        }
        Swal.fire({
            icon: 'error',
            text: 'Sorry, there was an issue unlocking this user. Please try again.',
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        })
        
            
    }
}
new UserView()