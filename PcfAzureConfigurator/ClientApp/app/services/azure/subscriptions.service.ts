import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult, ServiceError } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class SubscriptionsService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listSubscriptions(environment: string, token: string): Promise<Subscription[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let subscriptions = serviceResult.result as Subscription[];
                return subscriptions;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }
}

export class Subscription {
    constructor(public displayName: string, public subscriptionId: string) { }
}