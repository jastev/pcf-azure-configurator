import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult, ServiceError } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class StorageService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listAccounts(environment: string, token: string, subscriptionId: string, resourceGroupName: string): Promise<StorageAccount[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let accounts = serviceResult.result as StorageAccount[];
                return accounts;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    createAccount(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string, account: StorageAccount): Promise<void | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.put(uri, account, options).toPromise().then(
            successResponse => { },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                return serviceResult.result as ServiceError;
            });
    }

    getAccount(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<StorageAccount | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let account = serviceResult.result as StorageAccount;
                return account;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    getConnectionString(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<string | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/connectionstring';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + token })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let connectionString = serviceResult.result as string;
                return connectionString;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    listContainers(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<BlobContainer[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let containers = serviceResult.result as BlobContainer[];
                return containers;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    createContainer(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string): Promise<void | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.put(uri, null, options).toPromise().then(
            successResponse => { },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    listBlobs(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string): Promise<Blob[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let blobs = serviceResult.result as Blob[];
                return blobs;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    copyBlob(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string, blobName: string, sourceUri: string): Promise<void | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs/' + blobName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.put(uri, { uri: sourceUri }, options).toPromise().then(
            successResponse => { },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    getBlob(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string, blobName: string): Promise<Blob | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs/' + blobName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let blob = serviceResult.result as Blob;
                return blob;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    listTables(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<Table[] | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/tables';
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.get(uri, options).toPromise().then(
            successResponse => {
                let serviceResult = successResponse.json() as ServiceResult;
                let tables = serviceResult.result as Table[];
                return tables;
            },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }

    createTable(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, tableName: string): Promise<void | ServiceError> {
        let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/tables/' + tableName;
        let options = {
            'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
        };
        return this.http.put(uri, null, options).toPromise().then(
            successResponse => { },
            errorResponse => {
                let serviceResult = errorResponse.json() as ServiceResult;
                let error = serviceResult.result as ServiceError;
                return error;
            });
    }
}

export class StorageAccount {
    public kind: string;
    public location: string;
    public name: string;
    public sku: AccountSku;
    public properties: AccountProperties;
}

export class AccountSku {
    public name: string;
    public tier: string;
}

export class AccountProperties {
    public ProvisioningState: string;
}

export class BlobContainer {
    public name: string;
}

export class Blob {
    public name: string;
    public copyState: BlobCopyState;
    }

export class BlobCopyState {
    public source: string;
    public status: string;
    public bytesCopied: number;
    public totalBytes: number;
}

export class Table {
    public name: string;
}