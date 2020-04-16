import 'datatables.net'
import 'datatables.net-bs4'
import * as moment from 'moment';
import {CustomizedDataTable, IODataRequest, ISimpleStateData, IStateData} from '../services/customized-data-table'
import * as ko from 'knockout';
import * as KnockoutSecureBindingProvider from 'knockout-secure-binding'
import 'gijgo'

interface IUserSimpleStateData extends ISimpleStateData {
    ls: string;
    vs: string;
    as: string;
}
interface IUserStateData extends IStateData{
    lockedStatus: string;
    verifiedStatus: string;
    adminStatus: string;
}

interface IRequest {
    IsLocked?: boolean;
    IsVerified?: boolean
    IsAdmin?: boolean
}

class SearchViewModel {
    public lockedStatus = ko.observable('all');
    public verifiedStatus = ko.observable('all');
    public adminStatus = ko.observable('all');
    public fromDate = ko.observable('');
    public toDate = ko.observable('');
    constructor() {
        
    }
    
    public getSearchFilter(): IRequest {
        const req: IRequest = {};
        let status = this.lockedStatus();
        if (status !== 'all') {
            req.IsLocked =  status === 'locked'
        }
        status = this.verifiedStatus();
        if (status !== 'all') {
            req.IsVerified =  status === 'verified'
        }
        status = this.adminStatus();
        if (status !== 'all') {
            req.IsAdmin =  status === 'admin'
        }
        return req;
    }
    
    public hydrateFromParams(params: URLSearchParams) {
        if(params.has('ls')) {
            this.lockedStatus(params.get('ls') === 'l' ? 'locked' : 'unlocked')
        } else {
            this.lockedStatus('all')
        }

        if(params.has('vs')) {
            this.verifiedStatus(params.get('vs') === 'v' ? 'verified' : 'unverified')
        } else {
            this.verifiedStatus('all')
        }

        if(params.has('as')) {
            this.adminStatus(params.get('as') === 'a' ? 'admin' : 'nonadmin')
        } else {
            this.adminStatus('all')
        }
    }
    
    public generateStateData(stateData: IUserStateData, simpleStateData: IUserSimpleStateData)
        : {data: IStateData, simpleStateData: ISimpleStateData} {
        let status = this.lockedStatus();
        if (status !== 'all') {
            simpleStateData.ls = status === 'locked' ? 'l' : 'u';
            stateData.lockedStatus =  status;
        }

        status = this.verifiedStatus();
        if (status !== 'all') {
            simpleStateData.ls = status === 'verified' ? 'v' : 'u';
            stateData.lockedStatus =  status;
        }

        status = this.adminStatus();
        if (status !== 'all') {
            simpleStateData.ls = status === 'admin' ? 'a' : 'n';
            stateData.lockedStatus =  status;
        }
        
        return {data: stateData, simpleStateData: simpleStateData};
    } 
    
    public hydrateFromState(state: IUserStateData) {
        if (state.lockedStatus) {
            this.lockedStatus(state.lockedStatus);
        } else {
            this.lockedStatus('all');
        }

        if (state.verifiedStatus) {
            this.verifiedStatus(state.verifiedStatus);
        } else {
            this.verifiedStatus('all');
        }

        if (state.adminStatus) {
            this.adminStatus(state.adminStatus);
        } else {
            this.adminStatus('all');
        }
    }
    
}

export class UsersList {
    private tableApi: DataTables.Api;
    private detailsUrl: string;
    private searchFacets: HTMLFormElement;
    private customizedDataTable: CustomizedDataTable;
    private endpoint: string;
    private searchVM: SearchViewModel;
    
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
    
    private externalFilter(filterQuery: IODataRequest): Promise<any> {
        const getUrl = new URL(`${this.endpoint}/search`, document.location.origin);
        Object.keys(filterQuery).forEach(key => getUrl.searchParams.append(key, filterQuery[key]));
        const req: IRequest = this.searchVM.getSearchFilter();
        return  fetch(getUrl.toString(), {
            method: 'POST',
            mode: 'same-origin',
            cache: 'no-cache',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(req),
            redirect: 'follow',
            referrerPolicy: 'no-referrer',
        }).then((response) => {
            return response.json()
        });
    }
    private externalHydration(params: URLSearchParams): void {
        this.searchVM.hydrateFromParams(params);
        
       // this.updateControls()
    }
    private externalState (stateData: IUserStateData, simpleStateData: IUserSimpleStateData) :
        {data: IStateData, simpleStateData: ISimpleStateData} {
        return this.searchVM.generateStateData(stateData, simpleStateData);
    }
    private externalStateManager (state: IUserStateData)  {
        this.searchVM.hydrateFromState(state)        
    }

    init() {
        //var options: KnockoutSecureBindingOptions =;
        ko.bindingProvider.instance = new KnockoutSecureBindingProvider( {
            attribute: "data-bind",        // default "data-sbind"
            globals: window,               // default {}
            bindings: ko.bindingHandlers,  // default ko.bindingHandlers
            noVirtualElements: false       // default true
        });
        this.searchVM = new SearchViewModel();
        ko.applyBindings(this.searchVM);

        const contextThis = this;
        const $tableElement = $('#users');
        this.searchFacets = document.getElementById('search-facets') as HTMLFormElement;
        
        this.searchFacets.addEventListener('submit',(event) => contextThis.submitSearch(event));
        this.detailsUrl = $tableElement.data('details');
        this.endpoint =  $tableElement.data('route');

        this.customizedDataTable = new CustomizedDataTable($tableElement, {
            route: this.endpoint,
            externalFilter: (filterQuery) => contextThis.externalFilter(filterQuery),
            externalHydration: (params) => contextThis.externalHydration(params),
            externalState: (stateData: IUserStateData, simpleStateData: IUserSimpleStateData) => contextThis.externalState(stateData, simpleStateData),
            externalStateManager:(state: IUserStateData) => contextThis.externalStateManager(state)
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

    submitSearch(event: Event): any {
        event.preventDefault();
        this.customizedDataTable.tableApi.draw();
    }
}
$(document).ready(function() { console.log('here')});
new UsersList();

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var dp = $(element).datetimepicker({ 
            footer: true, 
            modal: true});
        dp.value(valueAccessor())

        ko.utils.registerEventHandler(element, "change", function (event) {

        });

//         ko.utils.registerEventHandler(element, "blur", function(event) {
// debugger
//         });
    }
};