import { Component, Inject } from '@angular/core';
import { ServiceError } from '../../services/serviceresult';
import { OauthToken } from '../../services/oauthtoken';
import { ActiveDirectoryService } from '../../services/azure/activedirectory.service';
import { SubscriptionsService, Subscription } from '../../services/azure/subscriptions.service';
import { ResourceGroupsService, ResourceGroup } from '../../services/azure/resourcegroups.service';
import { LocationsService, Location } from '../../services/azure/locations.service';
import { StorageService, StorageAccount, AccountSku, BlobContainer, Blob, Table } from '../../services/azure/storage.service';
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
    //public container: string;
    public blobs: Blob[];
    public blob: Blob | null;
    public copyBlobFrom: string;
    public tables: Table[];
    public deployments: Deployment[];
    public deployment: string;

    constructor(private activeDirectoryService: ActiveDirectoryService,
        private subscriptionsService: SubscriptionsService,
        private resourceGroupsService: ResourceGroupsService,
        private locationsService: LocationsService,
        private storageService: StorageService,
        private deploymentsService: DeploymentsService) { }

    public updateToken(): Promise<void> {
        return this.activeDirectoryService.getToken(this.environment, this.tenantId, this.clientId, this.clientSecret).then(
            (token: OauthToken) => {
                this.token = token;
                this.updateSubscriptionList();
            },
            (error: ServiceError) => {
                this.token = { 'access_token': '' };
                this.updateSubscriptionList();
            }
        );
    }

    public updateSubscriptionList(): Promise<void> {
        return this.subscriptionsService.listSubscriptions(this.environment, this.token.access_token).then(
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
            (error: ServiceError) => {
                this.subscriptions = [];
                this.subscription = '';
                this.updateSubscription();
            }
        );
    }

    public updateSubscription(): Promise<void[]> {
        return Promise.all([this.updateResourceGroupsList(), this.updateLocationsList()]);
    }

    public updateResourceGroupsList(): Promise<void> {
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
            (error: ServiceError) => {
                this.resourceGroups = [];
                this.resourceGroup = '';
                this.updateResourceGroup();
            }
        );
    }

    public createResourceGroup(): Promise<void> {
        return this.resourceGroupsService.createResourceGroup(this.environment, this.token.access_token, this.subscription, this.newResourceGroup, this.location)
            .then(this.updateResourceGroupsList)
            .then(() => { // Success
                this.resourceGroup = this.newResourceGroup;
                this.newResourceGroup = '';
                })
            .catch(
                (error: ServiceError) => {
                    // TODO resource group was not created, or could not update resource group list
                }
        );
    }

    public updateResourceGroup(): Promise<void> {
        return this.updateStorageAccountList();
    }

    public updateLocationsList(): Promise<void> {
        return this.locationsService.listLocations(this.environment, this.token.access_token, this.subscription).then(
            (locations: Location[]) => {
                this.locations = locations;
                if (!this.locations.find((l) => { return l.name == this.location })) {
                    this.location = '';
                }
            },
            (error: ServiceError) => {
                this.locations = [];
                this.location = '';
            }
        );
    }

    public updateStorageAccountList(): Promise<void> {
        return this.storageService.listAccounts(this.environment, this.token.access_token, this.subscription, this.resourceGroup).then(
            (storageAccounts: StorageAccount[]) => {
                this.storageAccounts = storageAccounts;
                if (!this.storageAccounts.find((sa) => { return sa.name == this.storageAccount })) {
                    this.storageAccount = '';
                    this.updateStorageAccount();
                }
            },
            (error: ServiceError) => {
                this.storageAccounts = [];
                this.storageAccount = '';
                this.updateStorageAccount();
            }
        );
    }

    public createStorageAccount(): Promise<void> {
        let account = new StorageAccount();
        account.name = this.newStorageAccount;
        account.kind = 'Storage';
        account.location = 'westus'; //this.resourceGroup.location;
        account.sku = new AccountSku();
        account.sku.name = 'Standard_LRS';
        return this.storageService.createAccount(this.environment, this.token.access_token, this.subscription, this.resourceGroup, this.newStorageAccount, account).then(
                this.updateStorageAccountList
                )
.catch(            (error: ServiceError) => {
                // TODO alert the user?
            }
        );
    }

    public updateStorageAccount(): Promise<void> {
        return this.storageService.getConnectionString(this.environment, this.token.access_token, this.subscription, this.resourceGroup, this.storageAccount).then(
            (connectionString: string) => {
                this.connectionString = connectionString;
                this.updateStorageContainerList();
                this.updateStorageTableList();
            },
            (error: ServiceError) => {
                this.connectionString = '';
                this.updateStorageContainerList();
                this.updateStorageTableList();
            }
        );
    }

    public updateStorageContainerList(): Promise<void> {
        return this.storageService.listContainers(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
            (containers: BlobContainer[]) => {
                this.containers = containers;
                this.updateBlobList();
            },
            (error: ServiceError) => {
                this.containers = [];
                this.updateBlobList();
            }
        );
    }

    public createContainer(name: string): Promise<void> {
            return this.storageService.createContainer(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, name).then(
                () => {
                    this.updateStorageContainerList();
                },
                (error: ServiceError) => {
                    // TODO alert the user?
                }
            );
    }

    public updateBlobList() { // Gets the blobs in the opsman-image container
        return this.storageService.listBlobs(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image').then(
            (blobs: Blob[]) => {
                this.blobs = blobs;
                this.getBlob();
            },
            (error: ServiceError) => {
                this.blobs = [];
                this.getBlob();
            }
        );
    }

    public copyBlob() { // Copy the blob to opsman-image/image.vhd
        return this.storageService.copyBlob(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image', 'image.vhd', this.copyBlobFrom).then(
            () => {
                this.updateStorageContainerList();
            },
            (error: ServiceError) => {
                // TODO alert the user?
            }
        )
    }

    public getBlob(): Promise<void> { // Gets the image.vhd blob in the opsman-image container
        return this.storageService.getBlob(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image', 'image.vhd').then(
            (blob: Blob) => {
                this.blob = blob;
                this.validateStorage();
            },
            (error: ServiceError) => {
                this.blob = null;
                this.validateStorage();
            }
        );
    }

    public updateStorageTableList(): Promise<void> {
        return this.storageService.listTables(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
            (tables: Table[]) => {
                this.tables = tables;
                this.validateStorage();
            },
            (error: ServiceError) => {
                this.tables = [];
                this.validateStorage();
            }
        );
    }

    public createTable() { // Create the 'stemcells' table
        return this.storageService.createTable(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'stemcells').then(
            () => {
                this.updateStorageTableList();
            },
            (error: ServiceError) => {
                // TODO alert the user?
            }
        )
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
