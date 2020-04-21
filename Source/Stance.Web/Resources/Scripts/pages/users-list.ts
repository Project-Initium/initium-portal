import 'datatables.net'
import 'datatables.net-bs4'
import * as moment from 'moment';
import {CustomizedDataTable, IODataRequest, ISimpleStateData, IStateData} from '../services/customized-data-table'
import 'gijgo'




export class UsersList {
    private tableApi: DataTables.Api;
    private detailsUrl: string;
    private searchFacets: HTMLFormElement;
    private customizedDataTable: CustomizedDataTable;
    private endpoint: string;
    
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
        const $tableElement = $('#users');
        this.searchFacets = document.getElementById('search-facets') as HTMLFormElement;
        
        this.detailsUrl = $tableElement.data('details');
        this.endpoint =  $tableElement.data('route');

        this.customizedDataTable = new CustomizedDataTable($tableElement, {
            route: this.endpoint,
        }, {
            columns: [
                { data: 'EmailAddress' },
                { data: 'FirstName' },
                { data: 'LastName' },
                { 
                    data: 'IsLocked',
                    searchable: false,
                    render: function (data, type, full, meta) {
                        if (data === true) {
                            return '<i class="fa fa-check" aria-hidden="true"></i>';
                        }
                        return '<i class="fa fa-times" aria-hidden="true"></i>';
                    }
                },
                { 
                    data: 'WhenLastAuthenticated',
                    searchable: false,
                    render: function (data, type, full, meta) {
                        const m = moment(data);
                        if(m.isValid)
                        {
                            const parsed = m.format('DD/MM/YYYY HH:mm')
                            if (parsed !== 'Invalid date')
                            {
                                return parsed;
                            }
                        }
                        return '';
                        
                    }
                },
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
new UsersList();
