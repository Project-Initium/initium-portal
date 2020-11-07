import slugify  from 'slugify';

export class TenantCreate {
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        const nameInput: HTMLInputElement = document.querySelector('input[data-name]');
        const identifierInput: HTMLInputElement = document.querySelector('input[data-identifier]');

        nameInput.addEventListener('blur', () => {
            identifierInput.value = slugify(nameInput.value, {
                lower: true,
                strict: true,
            });
        });
    }
}

// tslint:disable-next-line:no-unused-expression
new TenantCreate();