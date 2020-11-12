import { AppCore } from './core/app-core';

class App {
    private appCore: AppCore;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    private init() {
        this.appCore = new AppCore();
    }
}

// tslint:disable-next-line:no-unused-expression
new App();