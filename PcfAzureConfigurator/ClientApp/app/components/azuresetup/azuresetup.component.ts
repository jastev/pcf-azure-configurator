import { Component, Inject } from '@angular/core';
import { ServiceError } from '../../services/serviceresult';
import { OauthToken } from '../../services/oauthtoken';
import { ActiveDirectoryService } from '../../services/azure/activedirectory.service';
import { SubscriptionsService, Subscription } from '../../services/azure/subscriptions.service';
import { ResourceGroupsService, ResourceGroup } from '../../services/azure/resourcegroups.service';
import { LocationsService, Location } from '../../services/azure/locations.service';
import { StorageService, StorageAccount, AccountSku, BlobContainer, Blob, Table } from '../../services/azure/storage.service';
import { DeploymentsService, Deployment, DeploymentProperties, TemplateLink } from '../../services/azure/deployments.service';

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
    public hasContainerOpsmanImage: boolean;
    public hasContainerVhds: boolean;
    public hasContainerOpsmanager: boolean;
    public hasContainerBosh: boolean;
    public hasContainerStemcell: boolean;
    public blobs: Blob[];
    public blob: Blob | null;
    public hasBlobImageVhd: boolean;
    public copyPercent: number;
    public copyBlobFrom: string;
    public tables: Table[];
    public hasTableStemcells: boolean;
    public adminSshKey: string;
    public deployments: Deployment[];
    public deployment: string;

    constructor(private activeDirectoryService: ActiveDirectoryService,
        private subscriptionsService: SubscriptionsService,
        private resourceGroupsService: ResourceGroupsService,
        private locationsService: LocationsService,
        private storageService: StorageService,
        private deploymentsService: DeploymentsService) { }

    private sleep(time: number) {
        return new Promise((resolve) => setTimeout(resolve, time));
    }

    public updateToken(): Promise<void> {
        if (!this.environment || !this.tenantId || !this.clientId || !this.clientSecret) {
            return new Promise<void>((resolve, reject) => { });
        }
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
        if (!this.environment || !this.token.access_token) {
            return new Promise<void>((resolve, reject) => {
                this.subscriptions = [];
                this.subscription = '';
                this.updateSubscription();
                resolve();
            });
        }
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
        if (!this.environment || !this.token.access_token || !this.subscription) {
            return new Promise<void>((resolve, reject) => {
                this.resourceGroups = [];
                this.resourceGroup = '';
                this.updateResourceGroup();
                resolve();
            });
        }
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
        if (!this.environment || !this.token.access_token || !this.subscription || !this.newResourceGroup || !this.location) {
            return new Promise<void>((resolve, reject) => { });
        }
        return this.resourceGroupsService.createResourceGroup(this.environment, this.token.access_token, this.subscription, this.newResourceGroup, this.location)
            .then(() => { this.sleep(2000) })
            .then(() => { this.updateResourceGroupsList() })
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
        if (!this.environment || !this.token.access_token || !this.subscription) {
            return new Promise<void>((resolve, reject) => {
                this.locations = [];
                this.location = '';
                resolve();
            });
        }
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
        if (!this.environment || !this.token.access_token || !this.subscription || !this.resourceGroup) {
            return new Promise<void>((resolve, reject) => {
                this.storageAccounts = [];
                this.storageAccount = '';
                this.updateStorageAccount();
                resolve();
            });
        }
        return this.storageService.listAccounts(this.environment, this.token.access_token, this.subscription, this.resourceGroup).then(
            (storageAccounts: StorageAccount[]) => {
                this.storageAccounts = storageAccounts;
                if (this.storageAccounts.length == 1) {
                    this.storageAccount = storageAccounts[0].name;
                    this.updateStorageAccount();
                }
                else if (!this.storageAccounts.find((sa) => { return sa.name == this.storageAccount })) {
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
        if (!this.environment || !this.token.access_token || !this.subscription || !this.resourceGroup || !this.newStorageAccount) {
            return new Promise<void>((resolve, reject) => { });
        }
        let rg = this.resourceGroups.find((rg) => { return rg.name == this.resourceGroup });
        if (rg == null) throw new Error;  //TODO better error
        let account = new StorageAccount();
        account.name = this.newStorageAccount;
        account.kind = 'Storage';
        account.location = rg.location;
        account.sku = new AccountSku();
        account.sku.name = 'Standard_LRS';
        return this.storageService.createAccount(this.environment, this.token.access_token, this.subscription, this.resourceGroup, this.newStorageAccount, account)
            .then(() => { this.sleep(5000) })
            .then(() => { this.updateStorageAccountList() })
            .then(() => { // Success
                this.storageAccount = this.newStorageAccount;
                this.newStorageAccount = '';
            })
            .catch((error: ServiceError) => {
                    // TODO alert the user?
                }
            );
    }

    public updateStorageAccount(): Promise<void> {
        if (!this.environment || !this.token.access_token || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => {
                this.connectionString = '';
                this.updateStorageContainerList();
                this.updateStorageTableList();
                resolve();
            });
        }
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
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => {
                this.containers = [];

                this.hasContainerOpsmanImage = false;
                this.hasContainerVhds = false;
                this.hasContainerOpsmanager = false;
                this.hasContainerBosh = false;
                this.hasContainerStemcell = false;

                this.updateBlobList();
                resolve();
            });
        }
        return this.storageService.listContainers(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
            (containers: BlobContainer[]) => {
                this.containers = containers;

                this.hasContainerOpsmanImage = (containers.find((c) => { return c.name == 'opsman-image' }) != undefined);
                this.hasContainerVhds = (containers.find((c) => { return c.name == 'vhds' }) != undefined);
                this.hasContainerOpsmanager = (containers.find((c) => { return c.name == 'opsmanager' }) != undefined);
                this.hasContainerBosh = (containers.find((c) => { return c.name == 'bosh' }) != undefined);
                this.hasContainerStemcell = (containers.find((c) => { return c.name == 'stemcell' }) != undefined);

                this.updateBlobList();
            },
            (error: ServiceError) => {
                this.containers = [];

                this.hasContainerOpsmanImage = false;
                this.hasContainerVhds = false;
                this.hasContainerOpsmanager = false;
                this.hasContainerBosh = false;
                this.hasContainerStemcell = false;

                this.updateBlobList();
            }
        );
    }

    public createContainer(name: string): Promise<void> {
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount || !name) {
            return new Promise<void>((resolve, reject) => { });
        }
        return this.storageService.createContainer(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, name)
            .then(() => { this.sleep(2000) })
            .then(() => { this.updateStorageContainerList() })
            .catch(
                (error: ServiceError) => {
                    // TODO 
                }
            );
    }

    public updateBlobList(): Promise<void> { // Gets the blobs in the opsman-image container
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => {
                this.blobs = [];
                this.blob = null;
                this.hasBlobImageVhd = false;
                resolve();
            });
        }
        return this.storageService.listBlobs(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image').then(
            (blobs: Blob[]) => {
                this.blobs = blobs;
                this.getBlob();
            },
            (error: ServiceError) => {
                this.blobs = [];
                this.blob = null;
                this.hasBlobImageVhd = false;
            }
        );
    }

    public copyBlob(): Promise<void> { // Copy the blob to opsman-image/image.vhd
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => {
                this.blob = null;
                this.hasBlobImageVhd = false;
            });
        }
        return this.storageService.copyBlob(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image', 'image.vhd', this.copyBlobFrom)
            .then(() => { this.sleep(2000) })
            .then(() => { this.updateBlobList() })
            .catch(
                (error: ServiceError) => {
                    // TODO 
                }
            );
    }

    public getBlob(): Promise<void> { // Gets the image.vhd blob in the opsman-image container
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount || !this.hasContainerOpsmanImage) {
            return new Promise<void>((resolve, reject) => {
                this.blob = null;
                this.hasBlobImageVhd = false;
            });
        }
        return this.storageService.getBlob(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'opsman-image', 'image.vhd').then(
            (blob: Blob) => {
                this.blob = blob;
                if (blob.copyState.status == '1') {  // Pending
                    this.copyPercent = blob.copyState.bytesCopied / blob.copyState.totalBytes * 100;
                    this.sleep(5000).then(() => { this.getBlob() });
                }
                this.hasBlobImageVhd = (this.blob != null && this.blob.copyState.status == '2'); // Success
                this.copyPercent = Math.trunc(blob.copyState.bytesCopied / blob.copyState.totalBytes * 1000);  // TODO this is wrong, but 100 doesn't work either
            },
            (error: ServiceError) => {
                this.blob = null;
                this.hasBlobImageVhd = false;
            }
        );
    }

    public updateStorageTableList(): Promise<void> {
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => {
                this.tables = [];
                this.hasTableStemcells = false;
                resolve();
            });
        }
        return this.storageService.listTables(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount).then(
            (tables: Table[]) => {
                this.tables = tables;
                this.hasTableStemcells = (tables.find((t) => { return t.name == 'stemcells' }) != undefined);
            },
            (error: ServiceError) => {
                this.tables = [];
                this.hasTableStemcells = false;
            }
        );
    }

    public createTable(): Promise<void> { // Create the 'stemcells' table
        if (!this.environment || !this.connectionString || !this.subscription || !this.resourceGroup || !this.storageAccount) {
            return new Promise<void>((resolve, reject) => { });
        }
        return this.storageService.createTable(this.environment, this.connectionString, this.subscription, this.resourceGroup, this.storageAccount, 'stemcells').then(
            () => {
                this.updateStorageTableList();
            },
            (error: ServiceError) => {
                // TODO alert the user?
            }
        )
    }

    public updateDeploymentsList(): Promise<void> {
        if (!this.environment || !this.token.access_token || !this.subscription || !this.resourceGroup) {
            return new Promise<void>((resolve, reject) => { });
        }
        return this.deploymentsService.listDeployments(this.environment, this.token.access_token, this.subscription, this.resourceGroup).then(
            (deployments: Deployment[]) => {
                this.deployments = deployments;
            },
            (error: ServiceError) => {
                this.deployments = [];
            }
        );
    }

    public createDeployment(): Promise<void> { // Create the 'stemcells' table
        if (!this.environment || !this.token.access_token || !this.subscription || !this.resourceGroup) {
            return new Promise<void>((resolve, reject) => { });
        }
        let rg = this.resourceGroups.find((rg) => { return rg.name == this.resourceGroup });
        if (rg == null) throw new Error;  //TODO better error
        let templateUri = "https://raw.githubusercontent.com/pivotal-cf/pcf-azure-arm-templates/master/azure-deploy.json";
        let parameters = {
            Environment: { "value": this.environment },
            Location: { "value": rg.location },
            OpsManVHDStorageAccount: { "value": this.storageAccount },
            BlobStorageContainer: { "value": "opsman-image" },
            AdminSSHKey: { "value": this.adminSshKey },
        };
        let properties = new DeploymentProperties("Incremental", parameters, templateUri);
        return this.deploymentsService.createDeployment(this.environment, this.token.access_token, this.subscription, this.resourceGroup, "pcf", properties).then(
            () => {
                this.updateDeploymentsList();
            },
            (error: ServiceError) => {
                // TODO alert the user?
            }
        )
    }

}
