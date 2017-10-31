import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class DeploymentsService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listDeployments(environment: string, token: string, subscriptionId: string, resourceGroupName: string): Promise<Deployment[]> {
        return new Promise<Deployment[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/deployments';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let deployments = serviceResult.result as Deployment[];
                    resolve(deployments);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    createDeployment(environment: string, token: string, subscriptionId: string, resourceGroupName: string, deploymentName: string, properties: DeploymentProperties): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/deployments/' + deploymentName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.put(uri, properties, options).toPromise().then(
                successResponse => { resolve() },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    getDeployment(environment: string, token: string, subscriptionId: string, resourceGroupName: string, deploymentName: string): Promise<Deployment> {
        return new Promise<Deployment>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/deployments/' + deploymentName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let deployment = serviceResult.result as Deployment;
                    resolve(deployment);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }
}

export class Deployment {
    public name: string;
    public properties: DeploymentProperties;

    constructor(name: string, properties: DeploymentProperties) {
        this.name = name;
        this.properties = properties;
    }
}

export class DeploymentProperties {
    public mode: string;
    public parameters: object;
    public template: TemplateLink;

    constructor(mode: string, parameters: object, template: TemplateLink) {
        this.mode = mode;
        this.parameters = parameters;
        this.template = template;
    }
}

export class TemplateLink {
    public uri: string;

    constructor(uri: string) {
        this.uri = uri;
    }
}