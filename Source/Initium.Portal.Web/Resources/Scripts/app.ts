import { ValidationProvider } from './providers';
import Swal  from 'sweetalert2';
import 'bootstrap';
import 'metismenu';
import {TopNav} from './sections/top-nav';
import PerfectScrollbar from 'perfect-scrollbar';

class App {
    private header: HTMLElement;
    private pageWrapper: HTMLElement;
    init() {
        this.header = document.querySelector('header.topbar');
        this.pageWrapper = document.querySelector('.page-wrapper');

        this.setupNavbar();
        this.setupFormValidation();
        this.publishPageNotifications();

        const url = window.location;
        let element = $('ul#sidebarnav a').filter(function () {
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

        const systemAlertToggle = document.querySelector<HTMLButtonElement>('#global-system-alert-toggle');
        if(systemAlertToggle) {
            systemAlertToggle.addEventListener('click', (event) => {
                event.preventDefault();
                document.getElementById('global-system-alerts').classList.toggle('d-none');
            })
        }
        const t = new TopNav('#nav-notifications');
    }

    private setupFormValidation() {
        const forms = document.querySelectorAll('form:not([data-no-auto-validation])');
        Array.prototype.forEach.call(forms, (form: any) => {
            const v = new ValidationProvider(form);
        });
    }

    private publishPageNotifications() {
        const pageNotifications = document.querySelectorAll('span[data-page-notification]');
        if (pageNotifications.length) {
            pageNotifications.forEach((value) => {
                const pageNotification = value as HTMLSpanElement;
                Swal.fire({
                    icon: pageNotification.dataset.type as any,
                    text: pageNotification.dataset.message,
                    toast: true,
                    position: 'top-end',
                    timer: 4500,
                    showConfirmButton: false
                });
            }, this)
        }
    }
    private setupNavbar() {
        if (this.header) {
            $('#sidebarnav').metisMenu();
            const ps = new PerfectScrollbar('.scroll-sidebar', {
                wheelSpeed: 2,
                wheelPropagation: true,
                minScrollbarLength: 20
            });
            
            const contextThis = this;
            window.addEventListener('resize', () => contextThis.windowResizeListener());
            document.querySelector('.nav-toggler').addEventListener('click', () => {
                document.body.classList.toggle('mini-sidebar');
            });
            $('.dropdown-toggle').dropdown();
            this.windowResizeListener();
        }
    }

    private windowResizeListener(): void {
        if (this.header) {
            const width = (window.innerWidth > 0) ? window.innerWidth : screen.width;
            const topOffset = 70;

            if (width < 1170) {
                document.body.classList.add('mini-sidebar');
            } else {
                document.body.classList.remove('mini-sidebar');
                document.querySelector('.navbar-brand span').classList.remove('d-none');
            }
        }
    }
}

const app = new App();
$(() => {
    app.init();
});