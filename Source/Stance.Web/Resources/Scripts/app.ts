import 'bootstrap';
import 'metismenu';

class App {
    init() {
        $("#menu1").metisMenu();
    }
    
}

let app = new App();
$(() => {
    app.init();
});