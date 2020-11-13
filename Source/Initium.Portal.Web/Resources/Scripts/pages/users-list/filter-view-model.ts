import ko from 'knockout';
import { BaseFilterViewModel } from '../base-filter-view-model';
import { ICustomQuery, ISimpleStateData, IStateData } from '../../providers';

interface IRequest {
    verified: boolean;
    unverified: boolean;
    locked: boolean;
    unlocked: boolean;
    admin: boolean;
    nonAdmin: boolean;
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
    nonAdmin: boolean;
}

export class FilterViewModel extends BaseFilterViewModel {
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
            count ++;
        }
        if(this.unverified()) {
            count ++;
        }
        if(this.locked()) {
            count ++;
        }
        if(this.unlocked()) {
            count ++;
        }
        if(this.admin()) {
            count ++;
        }
        if(this.nonAdmin()) {
            count ++;
        }
        return count;
    });

    private customQuery: ICustomQuery;

    public getFilter(): ICustomQuery {
        if (!this.customQuery) {
            this.createInternalRequest();
        }
        return this.customQuery;
    }
    constructor() {
        super();
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
        userStateData.search = this.searchTerm();
        return {stateData: userStateData, simpleStateData: userSimpleStateData};
    }

    public hydrateFromState(userStateData: IUserStateData) {
        this.verified(userStateData.verified);
        this.unverified(userStateData.unverified);
        this.locked(userStateData.locked);
        this.unlocked(userStateData.unlocked);
        this.admin(userStateData.admin);
        this.nonAdmin(userStateData.nonAdmin);
        this.searchTerm(userStateData.search);
    }

    public reset() {
        this.verified(false);
        this.unverified(false);
        this.locked(false);
        this.unlocked(false);
        this.admin(false);
        this.nonAdmin(false);
    }

    public createInternalRequest() {
        const request: IRequest = {
            verified: this.verified(),
            unverified: this.unverified(),
            locked: this.locked(),
            unlocked: this.unlocked(),
            admin: this.admin(),
            nonAdmin: this.nonAdmin()
        };

        this.customQuery = {
            requestData: request,
            searchParam: this.searchTerm()
        };
    }
}