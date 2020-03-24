import { Validator } from './services/validator';
import Swal  from 'sweetalert2';
import 'bootstrap';
import 'metismenu';


class App {
    init() {
        $("#menu1").metisMenu();
        this.setupFormValidation();
        this.publishPageNotifications();
    }

    private setupFormValidation() {
        const forms = document.querySelectorAll('form:not([data-no-auto-validation])');
        Array.prototype.forEach.call(forms, (form: any) => {
            const v = new Validator(form);
        });
    }
    
    private publishPageNotifications() {
        var pageNotifications = document.querySelectorAll('span[data-page-notification]');
        if (pageNotifications.length) {
            pageNotifications.forEach((value) => {
                var pageNotification = value as HTMLSpanElement;
                Swal.fire({
                    icon: <any>pageNotification.dataset.type,
                    titleText: pageNotification.dataset.title,
                    text: pageNotification.dataset.message,
                    toast: true,
                    position: "top-end",
                    timer: 4500,
                    showConfirmButton: false
                });
            }, this)
        }
    }
}

let app = new App();
$(() => {
    app.init();
});