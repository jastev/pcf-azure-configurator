﻿import { Inject, Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { ServiceResult } from "../serviceresult";
import 'rxjs/add/operator/toPromise';

@Injectable()
export class LocationsService {
    private http: Http;
    private baseUrl: string;
    public locations: Location[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        this.http = http;
        this.baseUrl = baseUrl;
    }

    listLocations(environment: string, token: string, subscriptionId: string): Promise<Location[]> {
        return new Promise<Location[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/locations';
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise()
                .then(response => {
                    let serviceResult = response.json() as ServiceResult;
                    if (serviceResult.hasOwnProperty('error')) {
                        let error = serviceResult.error;
                        reject(error);
                    }
                    else {
                        this.locations = serviceResult.result as Location[];
                        resolve(this.locations);
                    }
                });
        });
    }

    listVmSizes(environment: string, token: string, subscriptionId: string, location: string): Promise<VmSize[]> {
        return new Promise<VmSize[]>((resolve, reject) => {
            let uri = this.baseUrl + 'api/v0/azure/' + environment + '/subscriptions/' + subscriptionId + '/locations/' + location;
            let options = {
                'headers': new Headers({ 'Authorization': "Bearer " + token })
            };
            this.http.get(uri, options).toPromise()
                .then(response => {
                    let serviceResult = response.json() as ServiceResult;
                    if (serviceResult.hasOwnProperty('error')) {
                        let error = serviceResult.error;
                        reject(error);
                    }
                    else {
                        let vmSizes = serviceResult.result as VmSize[];
                        resolve(vmSizes);
                    }
                });
        });
    }
}

export class Location {
    public name: string;
    public displayName: string;

    constructor(name: string, displayName: string) {
        this.name = name;
        this.displayName = displayName;
    }
}

export class VmSize {
    public name: string;
    public numberOfCores: number;
    public osDiskSizeInMB: number;
    public resourceDiskSizeInMB: number;
    public memoryInMB: number;
    public maxDataDiskCount: number;

    constructor(name: string, numberOfCores: number, osDiskSizeInMB: number, resourceDiskSizeInMB: number, memoryInMB: number, maxDataDiskCount: number) {
        this.name = name;
        this.numberOfCores = numberOfCores;
        this.osDiskSizeInMB = osDiskSizeInMB;
        this.resourceDiskSizeInMB = resourceDiskSizeInMB;
        this.memoryInMB = memoryInMB;
        this.maxDataDiskCount = maxDataDiskCount;
    }
}
