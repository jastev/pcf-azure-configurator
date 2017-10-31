﻿import { Inject, Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { OauthToken } from "../oauthtoken";
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ActiveDirectoryService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    getToken(environment: string, tenantId: string, clientId: string, clientSecret: string): Promise<OauthToken> {
        return new Promise<OauthToken>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/activedirectory/' + tenantId + '/client/' + clientId + '/authenticate';
            let body = { 'clientSecret': clientSecret };
            this.http.post(uri, body).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let token = serviceResult.result as OauthToken;
                    resolve(token);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }
}