export interface ServiceResult {
    result?: Object;
    error?: ServiceError;
}

export interface ServiceError {
    code: number;
    message?: string;
    data?: object;
}