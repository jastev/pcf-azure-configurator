﻿import { Inject, Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { OauthToken } from "../oauthtoken";
import { ServiceResult, ServiceError } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ActiveDirectoryService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    getToken(environment: string, tenantId: string, clientId: string, clientSecret: string): Promise<OauthToken | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/activedirectory/' + tenantId + '/client/' + clientId + '/authenticate';
        let body = { 'clientSecret': clientSecret };
        return this.http.post(uri, body).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let token = serviceResult.result as OauthToken;
                return token;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }
}