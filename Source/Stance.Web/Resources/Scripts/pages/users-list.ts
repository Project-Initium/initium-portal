import 'datatables.net'
import 'datatables.net-bs4'
import * as moment from 'moment';
import {
    CustomizedDataTable,
    ICustomQuery,
    IODataRequest,
    ISimpleStateData,
    IStateData
} from '../services/customized-data-table'
import * as ko from 'knockout'
import * as KnockoutSecureBinding from 'knockout-secure-binding'
import 'gijgo'

interface IRequest {
    verified: boolean;
    unverified: boolean;
    locked: boolean;
    unlocked: boolean;
    admin: boolean;
    nonAdmin: boolean
}

interface IUserSimpleStateData extends ISimpleStateData{
    fv: string;
    fuv: string;
    fl: string;
    ful: string;
    fa: string;
    fna: string;
}

interface IUserStateData extends IStateData {
    verified: boolean;
    unverified: boolean;
    locked: boolean;
    unlocked: boolean;
    admin: boolean;
    nonAdmin: boolean
}

class FilterViewModel {
    public verified = ko.observable<boolean>(true);
    public unverified = ko.observable<boolean>(true);
    public locked = ko.observable<boolean>(true);
    public unlocked = ko.observable<boolean>(true);
    public admin = ko.observable<boolean>(true);
    public nonAdmin = ko.observable<boolean>(true);
    public searchTerm = ko.observable<string>('');
    public filterCount = ko.computed<number>(() => {
        let count = 0;
        if(this.verified()) {
            count ++
        }
        if(this.unverified()) {
            count ++
        }
        if(this.locked()) {
            count ++
        }
        if(this.unlocked()) {
            count ++
        }
        if(this.admin()) {
            count ++
        }
        if(this.nonAdmin()) {
            count ++
        }
        return count;
    })
    
    public getFilter(): ICustomQuery {
        const request: IRequest = {
            verified: this.verified(),
            unverified: this.unverified(),
            locked: this.locked(),
            unlocked: this.unlocked(),
            admin: this.admin(),
            nonAdmin: this.nonAdmin()
        }
        
        return {
            requestData: request,
            method: 'POST',
            actionUrl: this.searchUrl
        }
    }
    constructor(private searchUrl: string) {
    }
    
    public hydrateFromParams(params: URLSearchParams) {
        this.verified(params.has('fv') && params.get('fv') === '1');
        this.unverified(params.has('fuv') && params.get('fuv') === '1');
        this.locked(params.has('fl') && params.get('fl') === '1');
        this.unlocked(params.has('ful') && params.get('ful') === '1');
        this.admin(params.has('fa') && params.get('fa') === '1');
        this.nonAdmin(params.has('fna') && params.get('fna') === '1');        
    }
    
    public generateStateData(userStateData: IUserStateData, userSimpleStateData: IUserSimpleStateData)
        : { stateData: IStateData, simpleStateData: ISimpleStateData} {

        userStateData.verified = this.verified();
        if (userStateData.verified) {            
            userSimpleStateData.fv = '1';
        }
        userStateData.unverified = this.unverified();
        if (userStateData.unverified) {
            userSimpleStateData.fuv = '1';
        }
        userStateData.locked = this.locked();
        if (userStateData.locked) {
            userSimpleStateData.fl = '1';
        }
        userStateData.unlocked = this.unlocked();
        if (userStateData.unlocked) {
            userSimpleStateData.ful = '1';
        }
        userStateData.admin = this.admin();
        if (userStateData.admin) {
            userSimpleStateData.fa = '1';
        }
        userStateData.nonAdmin = this.nonAdmin();
        if (userStateData.nonAdmin) {            
            userSimpleStateData.fna = '1';
        }
        return {stateData: userStateData, simpleStateData: userSimpleStateData}
    }
    
    public hydrateFromState(userStateData: IUserStateData) {
        this.verified(userStateData.verified);
        this.unverified(userStateData.unverified);
        this.locked(userStateData.locked);
        this.unlocked(userStateData.unlocked);
        this.admin(userStateData.admin);
        this.nonAdmin(userStateData.nonAdmin);
        this.searchTerm(userStateData.search.search);
    }

    reset() {
        this.verified(false);
        this.unverified(false);
        this.locked(false);
        this.unlocked(false);
        this.admin(false);
        this.nonAdmin(false);
    }

    
}


export class UsersList {
    private tableApi: DataTables.Api;
    private detailsUrl: string;
    private searchFacets: HTMLDivElement;
    private customizedDataTable: CustomizedDataTable;
    private exportUrl: string;
    private filterVM: FilterViewModel;
    private readonly tableOptions: DataTables.Settings = {
        columns: [
            {
                data: 'EmailAddress',
                type: 'string',
                title: 'Email Address'
            },
            {
                data: 'FirstName',
                type: 'string',
                title: 'First Name'
            },
            {
                data: 'LastName',
                type: 'string',
                title: 'Last Name',
            },
            {
                data: 'IsLocked',
                type: 'boolean',
                render: function (data, type, full, meta) {
                    if (data === true) {
                        return '<i class="fa fa-check" aria-hidden="true"></i>';
                    }
                    return '<i class="fa fa-times" aria-hidden="true"></i>';
                },
                title: 'Locked'
            },
            {
                data: 'WhenLastAuthenticated',
                type: 'date',
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
                },
                title: 'Last Login'
            },
            {
                data: 'Id',
                type: 'uuid',
                visible: false
            },
        ],
        dom: 'rt<"table-information"lpi>'
    };
    private filterToggleIcon: HTMLSpanElement;
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
                
        this.detailsUrl = $tableElement.data('details');

        this.filterVM = new FilterViewModel($tableElement.data('routeFiltered'))
        
        this.customizedDataTable = new CustomizedDataTable($tableElement, {
            route: $tableElement.data('route'),
            externalHydration: (params: URLSearchParams) => contextThis.filterVM.hydrateFromParams(params),
            externalState: (userStateData: IUserStateData, userSimpleStateData: IUserSimpleStateData) => contextThis.filterVM.generateStateData(userStateData, userSimpleStateData),
            externalStateManager: (userStateData: IUserStateData) => contextThis.filterVM.hydrateFromState(userStateData),
            externalFilter: () => contextThis.filterVM.getFilter()
            
        }, this.tableOptions);
        const filterForm = document.getElementById('filters') as HTMLFormElement;
        filterForm.addEventListener('reset', (event) => contextThis.filterVM.reset())
        filterForm.addEventListener('submit', (event) => contextThis.search(event));
        const toggle = filterForm.querySelector('.filter-toggle');
        toggle.addEventListener('click', (event) => contextThis.toggleFilters(event))
        this.filterToggleIcon = toggle.querySelector('i');
        this.searchFacets = filterForm.querySelector('#filter-options');

        
        
        
        ko.bindingProvider.instance = new KnockoutSecureBinding({
            attribute: 'data-bind',
            globals: window,
            bindings: ko.bindingHandlers,
            noVirtualElements: false
        })
        ko.applyBindings(this.filterVM);

        let exportBtn: HTMLButtonElement = document.querySelector('[data-export]');
        this.exportUrl = exportBtn.dataset.export;
        exportBtn.addEventListener('click', (event) => contextThis.requestExport(event))
        
        $tableElement.on('click', 'tbody tr', (event) => contextThis.rowClicked(event));
    }

    toggleFilters(event: Event): void {
        event.preventDefault();
        this.filterToggleIcon.classList.toggle('fa-caret-down');
        this.filterToggleIcon.classList.toggle('fa-caret-up');
        this.searchFacets.classList.toggle('d-none');
    }

    search(event: Event): any {
        event.preventDefault();
        this.tableApi.search(this.filterVM.searchTerm())
    }
    
    requestExport(event: MouseEvent): void {
        event.preventDefault();
        this.customizedDataTable.generateExport(this.exportUrl);
    }
}
new UsersList();
