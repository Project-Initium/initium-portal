import 'datatables.net'
import 'datatables.net-bs4'
import { BaseList } from '../base-list';
import  moment from 'moment';
import { FilterViewModel } from './filter-view-model';
import {ToastHelper} from '../../helpers';

interface INotification {
    Message: string;
    NotificationId: string;
    SerializedEventData: string
    Subject: string;
    Type: string;
    WhenNotified: Date
    WhenViewed: Date
}

export class NotificationList extends BaseList<FilterViewModel> {
    private selectedNotification: INotification;
    private infoPanel: HTMLDivElement;
    private infoPanelDismiss: HTMLButtonElement;
    private infoPanelAction: HTMLButtonElement;
    private infoPanelSubject: HTMLDivElement;
    private infoPanelMessage: HTMLDivElement;
    private infoPanelDate: HTMLDivElement;

    protected readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'WhenNotified',
                type: 'date',
                visible: false
            },
            {
                data: 'WhenViewed',
                type: 'date',
                visible: false
            },
            {
                data: 'Subject',
                type: 'string',
                title: 'Subject',
                render: (data, type, row) => {
                    const m = moment(row.WhenNotified);
                    const readState = row.WhenViewed ? 'font-weight-lighter' : 'font-weight-normal';
                    return `<div class="message-item ${readState} d-flex justify-content-between ${row.Type}"><span>${row.Subject}</span> <span>${m.format('DD/MM/YYYY HH:mm')}</span></div>`
                }
            },
            {
                data: 'Message',
                type: 'string',
                visible: false
            },
            {
                data: 'SerializedEventData',
                type: 'string',
                visible: false
            },
            {
                data: 'Type',
                type: 'number',
                visible: false
            },
            {
                data: 'NotificationId',
                type: 'uuid',
                visible: false
            }
        ],
        dom: 'r<"row"<"col-7 message-listing"t><"col-5 info-panel">><"table-information"lpi>',
        language: {
            processing: '<div class="processing-inner">Processing...</div>'
        }
    };

    constructor() {
        super();
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private init() {
        const contextThis = this;
        this.baseInit('#notifications', new FilterViewModel());
        this.infoPanel = document.querySelector('.info-panel');
        this.infoPanel.innerHTML = document.querySelector('#message-details').innerHTML;

        this.infoPanelAction = this.infoPanel.querySelector('[data-action]');
        this.infoPanelAction.addEventListener('click', (event) => NotificationList.action(event));
        this.infoPanelDismiss = this.infoPanel.querySelector('[data-dismiss]');
        this.infoPanelDismiss.addEventListener('click', (event) => contextThis.dismiss(event));
        this.infoPanelSubject = this.infoPanel.querySelector('[data-subject]');
        this.infoPanelMessage = this.infoPanel.querySelector('[data-message]');
        this.infoPanelDate = this.infoPanel.querySelector('[data-date]');

        document.getElementById('read-all').addEventListener('click', (event) => contextThis.readAll(event));
        document.getElementById('dismiss-all').addEventListener('click', (event) => contextThis.dismissAll(event))
    }

    private static action(event: MouseEvent): any {
        ToastHelper.showSuccessToast('Make something happen here');
    }

    private async dismissAll(event: MouseEvent) {
        event.preventDefault();
        await fetch((event.currentTarget as HTMLButtonElement).dataset.url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            redirect: 'follow',
            referrerPolicy: 'no-referrer'
        });
        this.customizedDataTable.tableApi.draw();
    }

    private async readAll(event: MouseEvent) {
        event.preventDefault();
        await fetch((event.currentTarget as HTMLButtonElement).dataset.url, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            redirect: 'follow',
            referrerPolicy: 'no-referrer'
        });
        this.customizedDataTable.tableApi.draw();
    }

    protected rowClicked(event: JQuery.ClickEvent) {

        this.selectedNotification = this.customizedDataTable.tableApi.row(event.currentTarget).data() as INotification;

        if(this.selectedNotification) {
            if (!this.selectedNotification.WhenViewed) {
                this.selectedNotification.WhenViewed = new Date();
                fetch(this.customizedDataTable.table.dataset.readItemUri, {
                    method: 'POST',
                    body: JSON.stringify({
                        notificationId: this.selectedNotification.NotificationId
                    }),
                    headers: {
                        'Accept': 'application/json',
                        'Content-Type': 'application/json'
                    },
                    mode: 'same-origin',
                    cache: 'no-cache',
                    credentials: 'same-origin',
                    redirect: 'follow',
                    referrerPolicy: 'no-referrer'
                });
            }
            this.infoPanel.dataset.type = this.selectedNotification.Type;
            this.infoPanelDismiss.classList.remove('d-none');
            this.infoPanelSubject.textContent = this.selectedNotification.Subject;
            this.infoPanelMessage.textContent = this.selectedNotification.Message;
            const m = moment(this.selectedNotification.WhenNotified);
            this.infoPanelDate.textContent =  m.format('DD/MM/YYYY HH:mm');
        }
    }
    private async dismiss(event: MouseEvent) {
        event.preventDefault();
        this.customizedDataTable.tableApi.processing(true);

        const response = await fetch(this.customizedDataTable.table.dataset.dismissItemUri, {
            method: 'POST',
            body: JSON.stringify({
                notificationId: this.selectedNotification.NotificationId
            }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            redirect: 'follow',
            referrerPolicy: 'no-referrer'
        });
        const data = await response.json();
        if (data && data.isSuccess){
            this.infoPanel.dataset.type = '';
            this.infoPanelDismiss.classList.add('d-none');
            this.infoPanelAction.classList.add('d-none');
            this.infoPanelSubject.textContent = '';
            this.infoPanelMessage.textContent = '';
            this.infoPanelDate.textContent = '';
            this.customizedDataTable.tableApi.draw();
        }

        this.customizedDataTable.tableApi.processing(false);
    }
}
const p = new NotificationList();

