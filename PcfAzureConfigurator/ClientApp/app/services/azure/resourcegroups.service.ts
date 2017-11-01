import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult, ServiceError } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ResourceGroupsService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listResourceGroups(environment: string, token: string, subscriptionId: string): Promise<ResourceGroup[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let resourceGroups = serviceResult.result as ResourceGroup[];
                return resourceGroups;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    createResourceGroup(environment: string, token: string, subscriptionId: string, resourceGroupName: string, location: string): Promise<void | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        let body = new ResourceGroup(resourceGroupName, location);
        return this.http.put(uri, body, options).toPromise().then(
            successResponse => { },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }
}

export class ResourceGroup {
    public name: string;
    public location: string;
    public properties: ResourceGroupProperties | undefined;

    constructor(name: string, location: string, properties?: ResourceGroupProperties) {
        this.name = name;
        this.location = location;
        this.properties = properties;
    }
}

export class ResourceGroupProperties {
    public provisioningState: string;

    constructor(provisioningState: string) {
        this.provisioningState = provisioningState;
    }
}