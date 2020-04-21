import 'datatables.net'
import 'datatables.net-bs4'

//import {DataTablesODataProvider} from '../services/datatables-odata-provider'

export class RoleList {
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
        window.location.href = this.detailsUrl.replace('__ID__', (<any>this.tableApi.row(event.currentTarget).data()).Id);
    }

    private init() {
        const contextThis = this;
        const $tableElement = $('#roles')
        this.detailsUrl = $tableElement.data('details')
        this.tableApi = $tableElement.DataTable({
            processing: true,
            serverSide: true,
            //ajax: DataTablesODataProvider.providerFunction($tableElement.data('route')),
            columns: [
                { data: 'Name' },
                { data: 'ResourceCount' },
                {
                    data: 'Id',
                    visible: false,
                    searchable: false                   
                }             
            ],
        });

        $tableElement.on('click', 'tbody tr', (event) => contextThis.rowClicked(event));
    }

}
new RoleList();
