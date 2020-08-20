export class ArrayHelpers {
    public static coerceToArrayBuffer(data: string | Uint8Array | ArrayBuffer) {
        if (typeof data === 'string') {
            data = data.replace(/-/g, '+').replace(/_/g, '/');

            const str = window.atob(data);
            const bytes = new Uint8Array(str.length);
            for (let i = 0; i < str.length; i++) {
                bytes[i] = str.charCodeAt(i);
            }
            data = bytes;
        }

        if (Array.isArray(data)) {
            data = new Uint8Array(data);
        }

        if (data instanceof Uint8Array) {
            data = data.buffer;
        }

        if (!(data instanceof ArrayBuffer)) {
            throw new TypeError('could not coerce data to ArrayBuffer');
        }

        return data;
    };


    public static coerceToBase64Url(data: Uint8Array | ArrayBuffer | string) {
        if (Array.isArray(data)) {
            data = Uint8Array.from(data);
        }

        if (data instanceof ArrayBuffer) {
            data = new Uint8Array(data);
        }

        if (data instanceof Uint8Array) {
            let str = '';
            const len = data.byteLength;

            for (let i = 0; i < len; i++) {
                str += String.fromCharCode(data[i]);
            }
            data = window.btoa(str);
        }

        if (typeof data !== 'string') {
            throw new Error('could not coerce to string');
        }

        data = data.replace(/\+/g, '-').replace(/\//g, '_').replace(/=*$/g, '');

        return data;
    }
}