import { ResultStatus } from './result-status';

export class Result<TValue, TError> {

    private constructor(
        private readonly resultStatus: ResultStatus,
        private readonly _value: TValue = null,
        private readonly _error: TError = null) {

    }

    public static Ok<TValue, TError>(value: TValue): Result<TValue, TError>{
        return new Result<TValue, TError>(ResultStatus.Ok, value);
    }

    public static Fail<TValue, TError>(error: TError): Result<TValue, TError>{
        return new Result<TValue, TError>(ResultStatus.Fail, null, error);
    }

    public get isSuccess(): boolean {
        return this.resultStatus === ResultStatus.Ok;
    }

    public get isFailure(): boolean {
        return this.resultStatus === ResultStatus.Fail;
    }

    public get value(): TValue {
        if(this.isFailure) {
            throw new Error('There is no value for failure.');
        }
        return this._value;
    }

    public get error(): TError {
        if(this.isSuccess) {
            throw new Error('There is no error for success.');
        }
        return this._error;
    }
}