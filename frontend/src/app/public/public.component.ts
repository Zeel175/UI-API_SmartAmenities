import { Component, HostBinding } from '@angular/core';
// import { CommonUtility } from '@app-core';
import{ CommonUtility } from 'app/core';
import { ActivatedRoute } from '@angular/router';
import { OverlayContainer } from '@angular/cdk/overlay';
// import { List } from '@app-models';
import { List } from 'app/model';
@Component({
    selector: 'public-root',
    templateUrl: './public.component.html',
    styleUrls: ['./public.component.scss']
})
export class PublicComponent {
    @HostBinding('class') componentCssClass : any;
    theme = 'default-theme';
    themeData: any;
    constructor(private activatedRoute: ActivatedRoute, public overlayContainer: OverlayContainer,) {
        this.getOnlineThemeData();
    }
    private getOnlineThemeData() {

        this.activatedRoute.data.subscribe(({ themeData }) => {
            if (CommonUtility.isNotEmpty(themeData)) {
                this.themeData = themeData;
                this.overlayContainer.getContainerElement().classList.add(this.theme);
                this.componentCssClass = this.themeData.onlineTheme;
               // console.log(this.themeData.onlineTheme);
            }

        });

    }
}
