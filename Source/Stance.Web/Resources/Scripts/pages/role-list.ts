import 'datatables.net'
import 'datatables.net-bs4'

import {DataTablesODataProvider} from '../services/datatables-odata-provider'

export class RoleList {
    private tableApi: DataTables.Api;
    private detailsUrl: string;


    private rowClicked(event: JQuery.ClickEvent): void {
        window.location.href = this.detailsUrl.replace('__ID__', (<any>this.tableApi.row(event.currentTarget).data()).Id);
    }

    init() {
        var contextThis = this;
        const $tableElement = $('#roles')
        this.detailsUrl = $tableElement.data('details')
        this.tableApi = $tableElement.DataTable({
            processing: true,
            serverSide: true,
            ajax: DataTablesODataProvider.providerFunction($tableElement.data('route')),
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
var roleList = new RoleList();
$(document).ready(() => roleList.init());