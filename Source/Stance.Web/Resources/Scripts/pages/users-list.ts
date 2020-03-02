import 'datatables.net'
import 'datatables.net-bs4'
import { DataTablesODataProvider } from '../services/datatables-odata-provider'

export class UsersList {
    private tableApi: DataTables.Api;
    private detailsUrl: string;

    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    private rowClicked(event: JQuery.ClickEvent): void {
        window.location.href = this.detailsUrl.replace('__id__', (<any>this.tableApi.row(event.currentTarget).data()).Id);
    }

    init() {
        var contextThis = this;
        const $tableElement = $('#users')
        this.detailsUrl = $tableElement.data('details')

        this.tableApi = $tableElement.DataTable({
            processing: true,
            serverSide: true,
            ajax: DataTablesODataProvider.providerFunction($tableElement.data('route')),
            columns: [
                { data: 'EmailAddress' },
                { data: 'FirstName' },
                { data: 'LastName' },
                { data: 'Locked' },
                { data: 'WhenLastAuthenticated' },
                {
                    data: 'Id',
                    visible: false,
                    searchable: false
                },
            ],
        });

        $tableElement.on('click', 'tbody tr', (event) => contextThis.rowClicked(event));
    }
}
$(document).ready(function() { console.log('here')});
new UsersList();