

interface JQuery {
    stick_in_parent(args: any): JQuery;
}

// tslint:disable-next-line:no-namespace
declare namespace DataTables {
    interface Api {
        processing(state: boolean)
    }
}