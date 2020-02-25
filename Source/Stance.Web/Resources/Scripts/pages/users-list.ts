import * as $ from 'jquery'
import 'datatables.net'
import 'datatables.net-bs4'

import {DataTablesODataProvider} from '../services/datatables-odata-provider'

export class UsersList {
    private actions: JQuery;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', e => this.init());
        }
    }

    init() {
        var contextThis = this;
        this.actions = $($('#actions').html());

        $('#users').DataTable({
            processing: true,
            serverSide: true,
            ajax: DataTablesODataProvider.providerFunction('/odata/user'),
            columns: [
                { data: "EmailAddress" },
                { data: "Username" },
                {
                    data: "Id",
                    render: (data: any, type: any, row: any, meta: DataTables.CellMetaSettings) => contextThis.renderActions(data, type, row, meta)
                }             
            ],
        });
    }

    renderActions(data: any, type: any, row: any, meta: DataTables.CellMetaSettings): any {
        const editAnchor = this.actions.find('a[data-edit]')
        editAnchor.attr('href', editAnchor.attr('href').replace('__ID__', data));
        return this.actions[0].innerHTML;
    }

}

new UsersList();