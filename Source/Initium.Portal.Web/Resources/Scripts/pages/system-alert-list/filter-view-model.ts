import ko from 'knockout';
import {ICustomQuery, ISimpleStateData, IStateData} from '../../providers';
import {BaseFilterViewModel} from '../base-list';

interface ISystemAlertSimpleStateData extends ISimpleStateData {
}

interface ISystemAlertStateData extends IStateData {
}

export class FilterViewModel extends BaseFilterViewModel {
    public searchTerm = ko.observable<string>('');
    private customQuery: ICustomQuery;


    constructor() {
        super()
    }

    createInternalRequest() {
        this.customQuery = {
            requestData: null,
            searchParam: this.searchTerm()
        }
    }

    generateStateData(stateData: ISystemAlertStateData, simpleStateData: ISystemAlertSimpleStateData):
        { stateData: IStateData; simpleStateData: ISimpleStateData } {
        stateData.search = this.searchTerm();
        return {stateData: stateData, simpleStateData: simpleStateData}
    }

    getFilter(): ICustomQuery {
        return {
            searchParam: '',
            requestData: {
            }
        }
    }

    hydrateFromParams(params: URLSearchParams) {
    }

    hydrateFromState(userStateData: IStateData) {
        this.searchTerm(userStateData.search);
    }

    reset() {
        return;
    }
}
