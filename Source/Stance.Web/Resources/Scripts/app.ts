import { Validator } from './services/validator';
import Swal  from 'sweetalert2';
import 'bootstrap';
import 'metismenu';


class App {
    private header: HTMLElement;
    private pageWrapper: HTMLElement;
    init() {
        this.header = document.querySelector('header.topbar');
        this.pageWrapper = document.querySelector(".page-wrapper");

        this.setupNavbar();
        this.setupFormValidation();
        this.publishPageNotifications();

        var url = window.location;
        var element = $('ul#sidebarnav a').filter(function () {
            return (this as HTMLAnchorElement).href == url.href;
        }).addClass('active').parent().addClass('active');
        while (true) {
            if (element.is('li')) {
                element = element.parent().addClass('in').parent().addClass('active');
            }
            else {
                break;
            }
        }
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
    private setupNavbar() {
        if (this.header) {
            $("#sidebarnav").metisMenu();
            const contextThis = this;
            window.addEventListener('resize', () => contextThis.windowResizeListener());
            document.querySelector('.nav-toggler').addEventListener('click', () => {
                document.body.classList.toggle('mini-sidebar');                
            });

            this.windowResizeListener();
        }
    }

    private windowResizeListener(): void {
        if (this.header) {
            var width = (window.innerWidth > 0) ? window.innerWidth : screen.width;
            var topOffset = 70;

            if (width < 1170) {
                document.body.classList.add('mini-sidebar')
                //document.querySelector('.navbar-brand span').classList.add('d-none');
                // document.querySelector(".scroll-sidebar, .slimScrollDiv").classList.add('overflow-auto')
                // document.querySelector(".scroll-sidebar, .slimScrollDiv").parentElement.classList.add('overflow-hidden');
            } else {
                document.body.classList.remove('mini-sidebar');
                document.querySelector('.navbar-brand span').classList.remove('d-none');
            }
        }
        
    }
}

let app = new App();
$(() => {
    app.init();
});