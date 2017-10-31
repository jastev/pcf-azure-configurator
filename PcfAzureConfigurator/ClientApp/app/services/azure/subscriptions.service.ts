import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class SubscriptionsService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listSubscriptions(environment: string, token: string): Promise<Subscription[]> {
        return new Promise<Subscription[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let subscriptions = serviceResult.result as Subscription[];
                    resolve(subscriptions);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }
}

export class Subscription {
    constructor(public displayName: string, public subscriptionId: string) { }
}