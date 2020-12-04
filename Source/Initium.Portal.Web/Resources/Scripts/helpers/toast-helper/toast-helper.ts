import Swal from 'sweetalert2';

export class ToastHelper {
    public static showSuccessToast(message: string) {
        Swal.fire({
            icon: 'success',
            text: message,
            toast: true,
            position: 'top-end',
            timer: 4500,
            showConfirmButton: false
        }).then();
    }

    public static showFailureToast(message: string) {
        Swal.fire({
            icon: 'error',
            text: message,
            toast: true,
            position: 'top-end',
            timer: 4500,
            showConfirmButton: false
        }).then();
    }
}