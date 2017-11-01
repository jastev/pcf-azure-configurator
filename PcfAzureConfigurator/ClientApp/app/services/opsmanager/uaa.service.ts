import { Inject, Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { OauthToken } from "../oauthtoken";
import { ServiceResult, ServiceError } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class UaaService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    getToken(opsManagerFqdn: string, username: string, password: string): Promise<OauthToken | ServiceError> {
        let uri = this.baseUrl + 'api/v0/opsmanager/uaa/token';
        let body = { 'OpsManagerFqdn': opsManagerFqdn, 'Username': username, 'Password': password };
        return this.http.post(uri, body).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let subscriptions = serviceResult.result as OauthToken;
                return subscriptions;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }
}