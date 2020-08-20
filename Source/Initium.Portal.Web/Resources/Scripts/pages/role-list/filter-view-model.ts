import 'knockout';
import {ICustomQuery, ISimpleStateData, IStateData} from '../../providers';
import {BaseFilterViewModel} from '../base-list';

interface IRequest {
    hasResources: boolean;
    hasNoResources: boolean;
    hasUsers: boolean;
    hasNoUsers: boolean;
}

interface IRoleSimpleStateData extends ISimpleStateData{
    fhr: string;
    fhnr: string;
    fhu: string;
    fhnu: string;
}

interface IRoleStateData extends IStateData {
    hasResources: boolean;
    hasNoResources: boolean;
    hasUsers: boolean;
    hasNoUsers: boolean;
}

export class FilterViewModel extends BaseFilterViewModel {
    public hasResources = ko.observable<boolean>(true);
    public hasNoResources = ko.observable<boolean>(true);
    public hasUsers = ko.observable<boolean>(true);
    public hasNoUsers = ko.observable<boolean>(true);
    public searchTerm = ko.observable<string>('');
    public filterCount = ko.computed<number>(() => {
        let count = 0;
        if(this.hasResources()) {
            count ++
        }
        if(this.hasNoResources()) {
            count ++
        }
        if(this.hasUsers()) {
            count ++
        }
        if(this.hasNoUsers()) {
            count ++
        }
        return count;
    });

    private customQuery: ICustomQuery;

    constructor() {
        super()
    }

    createInternalRequest() {
        const request: IRequest = {
            hasResources: this.hasResources(),
            hasNoResources: this.hasNoResources(),
            hasUsers: this.hasUsers(),
            hasNoUsers: this.hasNoUsers()
        };

        this.customQuery = {
            requestData: request,
            searchParam: this.searchTerm()
        }
    }

    generateStateData(stateData: IRoleStateData, simpleStateData: IRoleSimpleStateData): { stateData: IStateData; simpleStateData: ISimpleStateData } {
        stateData.hasResources = this.hasResources();
        if (stateData.hasResources) {
            simpleStateData.fhr = '1';
        }
        stateData.hasNoResources = this.hasNoResources();
        if (stateData.hasNoResources) {
            simpleStateData.fhnr = '1';
        }
        stateData.hasUsers = this.hasUsers();
        if (stateData.hasUsers) {
            simpleStateData.fhu = '1';
        }
        stateData.hasNoUsers = this.hasNoUsers();
        if (stateData.hasNoUsers) {
            simpleStateData.fhnu = '1';
        }
        stateData.search = this.searchTerm();
        return {stateData: stateData, simpleStateData: simpleStateData}
    }

    getFilter(): ICustomQuery {
        if (!this.customQuery) {
            this.createInternalRequest()
        }
        return this.customQuery;
    }

    hydrateFromParams(params: URLSearchParams) {
        this.hasResources(params.has('fhr') && params.get('fhr') === '1');
        this.hasNoResources(params.has('fhnr') && params.get('fhnr') === '1');
        this.hasUsers(params.has('fhu') && params.get('fhu') === '1');
        this.hasNoUsers(params.has('fhnu') && params.get('fhnu') === '1');
    }

    hydrateFromState(stateData: IRoleStateData) {
        this.hasResources(stateData.hasResources);
        this.hasNoResources(stateData.hasNoResources);
        this.hasUsers(stateData.hasUsers);
        this.hasNoUsers(stateData.hasNoUsers);
        this.searchTerm(stateData.search);
    }

    reset() {
        this.hasResources(false);
        this.hasNoResources(false);
        this.hasUsers(false);
        this.hasNoUsers(false);
    }

}