import { Component, Inject } from '@angular/core';
import { OauthToken } from '../../services/oauthtoken';
import { UaaService } from '../../services/opsmanager/uaa.service';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html',
    providers:[UaaService]
})
export class FetchDataComponent {
    public address: string;
    public username: string;
    public password: string;
    public token: OauthToken;

    constructor(private uaaService: UaaService) { }

    public getToken() {
        this.uaaService.getToken(this.address, this.username, this.password).then(token => { this.token = token; });
    }
}
