import { Inject, Injectable } from '@angular/core';
import { Http, RequestOptionsArgs } from '@angular/http';
import { OauthToken } from "../oauthtoken";
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class DirectorService {
    public properties: DirectorProperties;
    private http: Http;
    private baseUrl: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    getProperties(address: string, token: OauthToken): Promise<DirectorProperties> {
        return new Promise<DirectorProperties>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/opsmanager/director';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token }),
                'parameters': { 'OpsManagerFqdn': address }
            };
            this.http.post(uri, options).toPromise()
                .then(response => {
                    let serviceResult = response.json() as ServiceResult;
                    if (serviceResult.hasOwnProperty('error')) {
                        let error = serviceResult.error;
                        reject(error);
                    }
                    else {
                        this.properties = serviceResult.result as DirectorProperties;
                        resolve(this.properties);
                    }
                });
        });
    }
}

export interface DirectorProperties {
    iaas_configuration: IaasConfiguration,
    director_configuration: DirectorConfiguration,
    security_configuration: SecurityConfiguration,
    syslog_configuration: SyslogConfiguration
}

interface IaasConfiguration    {
    subscription_id: string,
    tenant_id: string,
    client_id: string,
    client_secret: string,
    resource_group_name: string,
    bosh_storage_account_name: string,
    default_security_group: string,
    ssh_public_key: string,
    ssh_private_key: string,
    cloud_storage_type: string,
    storage_account_type: string,
    environment: string
}

interface DirectorConfiguration {
    ntp_servers_string: string,
    metrics_ip: string,
    resurrector_enabled: boolean,
    max_threads?: number,
    database_type: string,
    blobstore_type: string
}

interface SecurityConfiguration {
    trusted_certificates: string,
    generate_vm_passwords: boolean
}

interface SyslogConfiguration {
    enabled: boolean
};
