export interface ServiceResult {
    result: Object;
}

export interface ServiceError {
    code: number;
    message?: string;
    data?: object;
}