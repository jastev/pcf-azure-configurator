import { Inject, Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { OauthToken } from "../oauthtoken";
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class UaaService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    getToken(opsManagerFqdn: string, username: string, password: string): Promise<OauthToken> {
        return new Promise<OauthToken>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/opsmanager/uaa/token';
            let body = { 'OpsManagerFqdn': opsManagerFqdn, 'Username': username, 'Password': password };
            this.http.post(uri, body).toPromise()
                .then(response => {
                    let serviceResult = response.json() as ServiceResult;
                    if (serviceResult.hasOwnProperty('error')) {
                        let error = serviceResult.error;
                        reject(error);
                    }
                    else {
                        let token = serviceResult.result as OauthToken;
                        resolve(token);
                    }
                });
        });
    }
}