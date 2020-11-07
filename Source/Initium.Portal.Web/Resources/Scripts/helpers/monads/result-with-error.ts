﻿import {ResultStatus} from './result-status';

export class ResultWithError<TError> {
    private readonly _error: TError;

    private constructor(private readonly resultStatus: ResultStatus, error: TError = null) {
        this._error = error
    }

    public static Ok<T>(): ResultWithError<T>{
        return new ResultWithError<T>(ResultStatus.Ok)
    }

    public static Fail<T>(error: T): ResultWithError<T>{
        return new ResultWithError<T>(ResultStatus.Fail, error);
    }

    public get isSuccess(): boolean {
        return this.resultStatus === ResultStatus.Ok;
    }

    public get isFailure(): boolean {
        return this.resultStatus === ResultStatus.Fail;
    }

    public get error(): TError {
        if(this.isSuccess) {
            throw new Error('There is no error for success.');
        }
        return this._error
    }
}