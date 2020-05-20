import Swal  from 'sweetalert2';

export class ValidatEmailMfa {
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const contextThis = this;
        const request = document.querySelector('[data-resend]') as HTMLAnchorElement;
        request.addEventListener('click', (e) => contextThis.resendEmail(e))
    }

    private async resendEmail(e: MouseEvent) {
        e.preventDefault();
        var link = e.target as HTMLAnchorElement;
        let response;
        try {
            let res = await fetch(link.href, {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                }
            });
            response = await res.json();
        } catch (e) {
            response = {
                success: false
            };
        }
        Swal.fire({
            icon: response.success ? 'info' : 'error',
            titleText: 'Authentication Email',
            text: response.success ? 'Email was sent.' : 'There was an issue sending the email.',
            toast: true,
            position: "top-end",
            timer: 4500,
            showConfirmButton: false
        });
    }
}

new ValidatEmailMfa();