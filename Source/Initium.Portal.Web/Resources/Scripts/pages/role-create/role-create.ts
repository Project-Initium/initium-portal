import 'gijgo';
import { ValidationProvider } from '../../providers';


export class RoleCreate {
    private tree: Types.Tree;
    private form: HTMLFormElement;
    private validator: ValidationProvider;
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

         this.form = document.querySelector('form#role-create') as HTMLFormElement;
         this.validator = new ValidationProvider(this.form, false);

         this.form.addEventListener('submit', (e) => { contextThis.formSubmit(e); });
    }

     private formSubmit(event: Event): void {
        if (this.validator.validate().isValid) {
            this.tree.find('input[type=checkbox]:checked').each((index, value) => {
                const checkbox = $(value);
                checkbox.attr('name', 'pagemodel.roles');

                const parentLi = checkbox.closest('li');
                checkbox.val(parentLi.data('id'));
            });
        } else {
            event.preventDefault();
        }
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
            }
        );
    }
}

// tslint:disable-next-line:no-unused-expression
new RoleCreate();