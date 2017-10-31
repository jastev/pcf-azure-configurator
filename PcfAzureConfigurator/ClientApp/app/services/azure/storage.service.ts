import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class StorageService {
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listAccounts(environment: string, token: string, subscriptionId: string, resourceGroupName: string): Promise<StorageAccount[]> {
        return new Promise<StorageAccount[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let accounts = serviceResult.result as StorageAccount[];
                    resolve(accounts);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    createAccount(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string, account: StorageAccount): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.put(uri, account, options).toPromise().then(
                successResponse => { resolve() },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    getAccount(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<StorageAccount> {
        return new Promise<StorageAccount>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let account = serviceResult.result as StorageAccount;
                    resolve(account);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    getConnectionString(environment: string, token: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<string> {
        return new Promise<string>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let connectionString = serviceResult.result as string;
                    resolve(connectionString);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    listContainers(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<BlobContainer[]> {
        return new Promise<BlobContainer[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let containers = serviceResult.result as BlobContainer[];
                    resolve(containers);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    createContainer(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.put(uri, null, options).toPromise().then(
                successResponse => { resolve() },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    listBlobs(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string): Promise<Blob[]> {
        return new Promise<Blob[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let blobs = serviceResult.result as Blob[];
                    resolve(blobs);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    copyBlob(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string, blobName: string, sourceUri: string): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs/' + blobName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.put(uri, sourceUri, options).toPromise().then(
                successResponse => { resolve() },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    getBlob(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, containerName: string, blobName: string): Promise<Blob> {
        return new Promise<Blob>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/containers/' + containerName + '/blobs/' + blobName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let blob = serviceResult.result as Blob;
                    resolve(blob);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    listTables(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string): Promise<Table[]> {
        return new Promise<Table[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourceGroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/tables';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.get(uri, options).toPromise().then(
                successResponse => {
                    let serviceResult = successResponse.json() as ServiceResult;
                    let tables = serviceResult.result as Table[];
                    resolve(tables);
                },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
        });
    }

    createTable(environment: string, connectionString: string, subscriptionId: string, resourceGroupName: string, accountName: string, tableName: string): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/resourcegroups/' + resourceGroupName + '/storageaccounts/' + accountName + '/tables/' + tableName;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + connectionString })
            };
            this.http.put(uri, null, options).toPromise().then(
                successResponse => { resolve() },
                errorResponse => {
                    let serviceResult = errorResponse.json() as ServiceResult;
                    reject(serviceResult.error);
                });
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