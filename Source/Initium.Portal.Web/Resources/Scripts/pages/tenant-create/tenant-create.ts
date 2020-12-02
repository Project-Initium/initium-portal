import slugify  from 'slugify'
export class TenantCreate {
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const nameInput: HTMLInputElement = document.querySelector('input[data-name]');
        const identifierInput: HTMLInputElement = document.querySelector('input[data-identifier]');

        nameInput.addEventListener('blur', () => {
            identifierInput.value = slugify(nameInput.value, {
                lower: true,      // convert to lower case, defaults to `false`
                strict: true,
            });
        })
    }
}

const p = new TenantCreate()