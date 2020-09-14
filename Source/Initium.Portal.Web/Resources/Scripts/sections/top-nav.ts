import {o, OdataConfig} from 'odata';
import 'bootstrap';
import {ToastHelper} from '../helpers';
import moment from 'moment';
import PerfectScrollbar from
        'perfect-scrollbar';

export interface Value {
    NotificationId: string;
    UserId: string;
    WhenNotified: Date;
    Type: string;
    SerializedEventData: string;
    Subject: string;
    Message: string;
    WhenViewed?: any;
}

export interface ODataResponse {
    '@odata.context': string;
    value: Value[];
}
export class TopNav {
    private notifications: HTMLUListElement;
    private notificationListContainer: HTMLDivElement;
    private hasData: boolean = false;
    constructor(notificationSelector: string) {
        this.notifications = document.querySelector(notificationSelector);
        this.notificationListContainer = this.notifications.querySelector('.message-center');

        const $not = $(this.notifications);
        const contextThis = this;
        $not.on('show.bs.dropdown',  () => contextThis.getNotifications());

        const ps = new
        PerfectScrollbar(`${notificationSelector} .message-center`, {
            wheelSpeed: 2,
            wheelPropagation: true,
            minScrollbarLength: 20
        });
    }

    async getNotifications() {
        if (this.hasData) {
            return;
        }

        this.hasData = true;
        const config: OdataConfig = {
            disablePolyfill: false,
            fragment: '',
            onStart: () => null,
            onFinish: () => null,
            onError: () => null,
            rootUrl: new URL(this.notifications.dataset.odataRoot, window.location.href),
            batch: {
                useChangset: false,
                headers:new Headers({
                    'Content-Type': 'application/json',
                })
            },
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            redirect: 'follow',
            referrerPolicy: 'no-referrer'
        }
        const data: ODataResponse = await o(this.notifications.dataset.odataRoot, config)
            .get('UserNotification')
            .query({ $top: 5, $orderby: 'WhenNotified' });

        const contextThis = this;

        data.value.forEach(item => {
            const m = moment(item.WhenNotified);

            const status = item.WhenViewed ? 'read' : 'unread';
            const template = `<a href="" id="notification-item-${item.NotificationId}" class="message-item ${status} d-flex flex-column border-bottom px-3 py-2 ${item.Type}">
            <h6 class="message-title mb-1 mt-1">${item.Message}</h6>
            <span class="font-12 text-nowrap d-block text-muted small text-right">${m.format('DD/MM/YYYY HH:mm')}</span>
            </a>`;
            const container: HTMLDivElement = document.createElement('div');
            container.innerHTML = template;
            const ele = container.firstChild;
            contextThis.notificationListContainer.appendChild(ele);
            ele.addEventListener('click', (event) => contextThis.markAsReadAndPerformAction(event, item));
        });
    }

    async markAsReadAndPerformAction(event: Event, item: Value) {
        event.preventDefault();
        const target: HTMLAnchorElement = document.querySelector(`#notification-item-${item.NotificationId}`);
        if (target.classList.contains('unread')) {
            await fetch(this.notifications.dataset.readItemUri, {
                method: 'POST',
                body: JSON.stringify({
                    notificationId: item.NotificationId
                }),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.notifications.dataset.afToken
                },
                mode: 'same-origin',
                cache: 'no-cache',
                credentials: 'same-origin',
                redirect: 'follow',
                referrerPolicy: 'no-referrer'
            });
            target.classList.add('read');
            target.classList.remove('unread');
        }

        ToastHelper.showSuccessToast('Make something happen here');

    }
}