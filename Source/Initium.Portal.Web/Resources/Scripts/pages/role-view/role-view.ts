import 'gijgo';

export class RoleView {
    private tree: Types.Tree;
    constructor() {
        if (document.readyState !== 'loading') {
            this.init();
        } else {
            document.addEventListener('DOMContentLoaded', _ => this.init());
        }
    }

    init() {
        const contextThis = this;
        const $tree = $('#tree');
        this.tree = $tree.tree({
            uiLibrary: 'bootstrap4',
            dataSource: $tree.data('route'),
            primaryKey: 'id',
            childrenField: 'simpleResources',
            textField: 'name',
            checkboxes: true,
            dataBound: _ => contextThis.treeOnDataBound()
        });
    }

    private treeOnDataBound(){
        const contextThis = this;

        const resources = document.querySelectorAll('input[type="hidden"][name="pagemodel.resources"]');
        Array.prototype.forEach.call(resources,
            (element: HTMLInputElement) => {
                const node = contextThis.tree.getNodeById(element.value);
                if (node) {
                    contextThis.tree.check(node);
                }
                element.parentElement.removeChild(element);
            });
            this.tree.disableAll();
            this.tree.expandAll();
    }
}

// tslint:disable-next-line:no-unused-expression
new RoleView();