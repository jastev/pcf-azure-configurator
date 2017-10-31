import { Component, Inject } from '@angular/core';
import { OauthToken } from '../../services/oauthtoken';
import { ActiveDirectoryService } from '../../services/azure/activedirectory.service';
import { SubscriptionsService, Subscription } from '../../services/azure/subscriptions.service';
import { ResourceGroupsService, ResourceGroup } from '../../services/azure/resourcegroups.service';
import { LocationsService, Location } from '../../services/azure/locations.service';
import { StorageService, StorageAccount, BlobContainer, Blob, Table } from '../../services/azure/storage.service';
import { DeploymentsService, Deployment } from '../../services/azure/deployments.service';

@Component({
    selector: 'azuresetup',
    templateUrl: './azuresetup.component.html',
    providers: [
        ActiveDirectoryService,
        SubscriptionsService,
        ResourceGroupsService,
        LocationsService,
        StorageService,
        DeploymentsService
    ]
})
export class AzureSetupComponent {
    public environments: string[] = ['AzureCloud'];
    public environment: string = 'AzureCloud';
    public tenantId: string;
    public clientId: string;
    public clientSecret: string;
    public token: OauthToken = { 'access_token': '' };
    public subscriptions: Subscription[];
    public subscription: string;
    public resourceGroups: ResourceGroup[];
    public resourceGroup: string;
    public newResourceGroup: string;
    public locations: Location[];
    public location: string;
    public storageAccounts: StorageAccount[];
    public storageAccount: string;
    public newStorageAccount: string;
    public connectionString: string;
    public containers: BlobContainer[];
    public container: string;
    public blobs: Blob[];
    public blob: Blob | null;
    public tables: Table[];
    public deployments: Deployment[];
    public deployment: string;

    constructor(private activeDirectoryService: ActiveDirectoryService,
        private subscriptionsService: SubscriptionsService,
        private resourceGroupsService: ResourceGroupsService,
        private locationsService: LocationsService,
        private storageService: StorageService,
        private deploymentsService: DeploymentsService) { }

    public updateToken() {
        if (this.environment && this.tenantId && this.clientId && this.clientSecret) {
            this.activeDirectoryService.getToken(this.environment, this.tenantId, this.clientId, this.clientSecret).then(
                (token: OauthToken) => {
                    this.token = token;
                    this.updateSubscriptionList();
                },
                () => {
                    this.token = { 'access_token': '' };
                    this.updateSubscriptionList();
                }
            );
        } else if (this.token.access_token) {
            this.token = { 'access_token': '' };
            this.updateSubscriptionList();
        }
    }

    public updateSubscriptionList() {
        if (this.environment && this.token.access_token) {
            this.subscriptionsService.listSubscriptions(this.environment, this.token.access_token).then(
                (subscriptions: Subscription[]) => {
                    this.subscriptions = subscriptions;
                    if (this.subscriptions.length == 1) {
                        this.subscription = subscriptions[0].subscriptionId;
                        this.updateSubscription();
                    }
                    else if (!this.subscriptions.find((s) => { return s.subscriptionId == this.subscription })) {
                        this.subscription = '';
                        this.updateSubscription();
                    }
                },
                () => {
                    this.subscriptions = [];
                    this.subscription = '';
                    this.updateSubscription();
                }
            );
        } else {
            this.subscriptions = [];
            this.subscription = '';
            this.updateSubscription();
        }
    }

    public updateSubscription() {
        this.updateResourceGroupsList();
        this.updateLocationsList();
    }

    public updateResourceGroupsList(): Promise<void> {
        if (this.environment && this.token.access_token && this.subscription) {
            return this.resourceGroupsService.listResourceGroups(this.environment, this.token.access_token, this.subscription).then(
                (resourceGroups: ResourceGroup[]) => {
                    this.resourceGroups = resourceGroups;
                    if (this.resourceGroups.length == 1) {
                        this.resourceGroup = resourceGroups[0].name;
                        this.updateResourceGroup();
                    }
                    else if (!this.resourceGroups.find((rg) => { return rg.name == this.resourceGroup })) {
                        this.resourceGroup = '';
                        this.updateResourceGroup();
                    }
                },
                () => {
                    this.resourceGroups = [];
                    this.resourceGroup = '';
                    this.updateResourceGroup();
                }
            );
        } else {
            return new Promise<void>(
                (resolve, reject) => {
                    this.resourceGroups = [];
                    this.resourceGroup = '';
                    this.updateResourceGroup();
                    resolve();
                }
            );
        }
    }

    public createResourceGroup() {
        if (this.environment && this.token.access_token && this.subscription && this.newResourceGroup && this.location) {
            this.resourceGroupsService.createResourceGroup(this.environment, this.token.access_token, this.subscription, this.newResourceGroup, this.location).then(
                () => { // New RG creation request succeeded
                    this.updateResourceGroupsList().then(
                        () => { // RG list update succeeded
                            this.resourceGroup = this.newResourceGroup;
                            this.newResourceGroup = '';
                        }
                    );
                },
                () => { // New RG creation request failed
                    // TODO alert the user to the error
                }
            );
        }
    }

    public updateResourceGroup() {
        this.updateStorageAccountList();
    }

    public updateLocationsList() {
        if (this.environment && this.token.access_token && this.subscription) {
            return this.locationsService.listLocations(this.environment, this.token.access_token, this.subscription).then(
                (locations: Location[]) => {
                    this.locations = locations;
                    if (!this.locations.find((l) => { return l.name == this.location })) {
                        this.location = '';
                    }
                },
                () => {
                    this.locations = [];
                    this.location = '';
                }
            );
        } else {
            return new Promise<void>(
                (resolve, reject) => {
                    this.locations = [];
                    this.location = '';
                    resolve();
                }
            );
        }
    }

    public updateStorageAccountList() {
        if (this.environment && this.token.access_token && this.subscription && this.resourceGroup) {
            this.storageService.listAccounts(this.environment, this.token.access_token, this.subscription, this.resourceGroup).then(
                (storageAccounts: StorageAccount[]) => {
                    this.storageAccounts = storageAccounts;
                    if (!this.storageAccounts.find((sa) => { return sa.name == this.storageAccount })) {
                        this.storageAccount = '';
                        this.updateStorageAccount();
                    }
                },
                () => {
                    this.storageAccounts = [];
                    this.storageAccount = '';
                    this.updateStorageAccount();
                }
            );
        } else {
            this.storageAccounts = [];
            this.storageAccount = '';
            this.updateStorageAccount();
        }
    }

    public updateStorageAccount() {
        if (this.storageAccount) {
            this.storageService.getConnectionString(this.environment, this.token.access_token, this.subscription, this.resourceGroup, this.storageAccount).then(
                (connectionString: string) => {
                    this.connectionString = connectionString;
                    this.updateStorageContainerList();
                    this.updateStorageTableList();
                },
                () => {
                    this.connectionString = '';
                    this.updateStorageContainerList();
                    this.updateStorageTableList();
                }
            );
        }
    }

    public updateStorageContainerList() {
        if (this.environment && this.connectionString && this.subscription && this.resourceGroup && this.storageAccount) {
            this.storageService.listContainers(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
                (containers: BlobContainer[]) => {
                    this.containers = containers;
                    this.updateBlobList();
                },
                () => {
                    this.containers = [];
                    this.updateBlobList();
                }
            );
        } else {
            this.containers = [];
            this.updateBlobList();
        }
    }

    public updateBlobList() {
        if (this.environment && this.connectionString && this.subscription && this.resourceGroup && this.storageAccount && this.containers
            && this.containers.find((container: BlobContainer) => { return container.name == 'opsman-image'; })) {
            this.storageService.listBlobs(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image').then(
                (blobs: Blob[]) => {
                    this.blobs = blobs;
                    this.getBlob();
                },
                () => {
                    this.blobs = [];
                    this.getBlob();
                }
            );
        } else {
            this.blobs = [];
            this.getBlob();
        }
    }

    public getBlob() {
        if (this.blobs.find((blob: Blob) => { return blob.name == 'image.vhd'; })) {
            this.storageService.getBlob(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image', 'image.vhd').then(
                (blob: Blob) => {
                    this.blob = blob;
                    this.validateStorage();
                },
                () => {
                    this.blob = null;
                    this.validateStorage();
                }
            );
        }
        else {
            this.blob = null;
            this.validateStorage();
        }
    }

    public updateStorageTableList() {
        if (this.environment && this.connectionString && this.subscription && this.resourceGroup && this.storageAccount) {
            this.storageService.listTables(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
                (tables: Table[]) => {
                    this.tables = tables;
                    this.validateStorage();
                },
                () => {
                    this.tables = [];
                    this.validateStorage();
                }
            );
        } else {
            this.tables = [];
            this.validateStorage();
        }
    }

    public validateStorage() {
        if (
            this.containers.find((container: BlobContainer) => { return container.name == 'opsman-image'; }) &&
            this.containers.find((container: BlobContainer) => { return container.name == 'vhds'; }) &&
            this.containers.find((container: BlobContainer) => { return container.name == 'opsmanager'; }) &&
            this.containers.find((container: BlobContainer) => { return container.name == 'bosh'; }) &&
            this.containers.find((container: BlobContainer) => { return container.name == 'stemcell'; }) &&
            this.blob && this.blob.copyState.status == 'Success' &&
            this.tables.find((table: Table) => { return table.name == 'stemcells'; })
        ) {

        }
    }
}
