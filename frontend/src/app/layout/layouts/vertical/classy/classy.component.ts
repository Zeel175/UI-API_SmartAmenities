// import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
// import { MatButtonModule } from '@angular/material/button';
// import { MatIconModule } from '@angular/material/icon';
// import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
// import { FuseFullscreenComponent } from '@fuse/components/fullscreen';
// import { FuseLoadingBarComponent } from '@fuse/components/loading-bar';
// import {
//     FuseNavigationService,
//     FuseVerticalNavigationComponent,
// } from '@fuse/components/navigation';
// import { FuseMediaWatcherService } from '@fuse/services/media-watcher';
// import { NavigationService } from 'app/core/navigation/navigation.service';
// import { Navigation } from 'app/core/navigation/navigation.types';
// import { UserService } from 'app/core/user/user.service';
// import { User } from 'app/core/user/user.types';
// import { LanguagesComponent } from 'app/layout/common/languages/languages.component';
// import { MessagesComponent } from 'app/layout/common/messages/messages.component';
// import { NotificationsComponent } from 'app/layout/common/notifications/notifications.component';
// import { QuickChatComponent } from 'app/layout/common/quick-chat/quick-chat.component';
// import { SearchComponent } from 'app/layout/common/search/search.component';
// import { ShortcutsComponent } from 'app/layout/common/shortcuts/shortcuts.component';
// import { UserComponent } from 'app/layout/common/user/user.component';
// import { Subject, takeUntil } from 'rxjs';

// @Component({
//     selector: 'classy-layout',
//     templateUrl: './classy.component.html',
//     encapsulation: ViewEncapsulation.None,
//     imports: [
//         FuseLoadingBarComponent,
//         FuseVerticalNavigationComponent,
//         // NotificationsComponent,
//         UserComponent,
//         MatIconModule,
//         MatButtonModule,
//         // LanguagesComponent,
//         FuseFullscreenComponent,
//         SearchComponent,
//         // ShortcutsComponent,
//         // MessagesComponent,
//         RouterOutlet,
//         // QuickChatComponent,
//     ],
// })
// export class ClassyLayoutComponent implements OnInit, OnDestroy {
//     isScreenSmall: boolean;
//     navigation: Navigation;
//     user: User;
//         displayName: string;
//     email: string;
//     avatar: string;
//     private _unsubscribeAll: Subject<any> = new Subject<any>();

//     /**
//      * Constructor
//      */
//     constructor(
//         private _activatedRoute: ActivatedRoute,
//         private _router: Router,
//         private _navigationService: NavigationService,
//         private _userService: UserService,
//         private _fuseMediaWatcherService: FuseMediaWatcherService,
//         private _fuseNavigationService: FuseNavigationService
//     ) {}

//     // -----------------------------------------------------------------------------------------------------
//     // @ Accessors
//     // -----------------------------------------------------------------------------------------------------

//     /**
//      * Getter for current year
//      */
//     get currentYear(): number {
//         return new Date().getFullYear();
//     }

//     // -----------------------------------------------------------------------------------------------------
//     // @ Lifecycle hooks
//     // -----------------------------------------------------------------------------------------------------

//     /**
//      * On init
//      */
//     ngOnInit(): void {
//         debugger
//         // Subscribe to navigation data
//         //  const user = JSON.parse(localStorage.getItem('user') || '{}');
//         // this.displayName = user.displayName || '';
//         // this.email = user.email || '';
//         // this.avatar = user.avatar || '';
//         const storedUser = JSON.parse(localStorage.getItem('user') || '{}');
//         this.displayName = storedUser.displayName || '';
//         this.email = storedUser.email || '';
//         this.avatar = storedUser.avatar || '';
//         this.user = storedUser as User;
//         this._navigationService.navigation$
//             .pipe(takeUntil(this._unsubscribeAll))
//             .subscribe((navigation: Navigation) => {
//                 this.navigation = navigation;
//             });

//         // Subscribe to the user service
//         this._userService.user$
//             .pipe(takeUntil(this._unsubscribeAll))
//             .subscribe((user: User) => {
//                 this.user = user;
//             });

//         // Subscribe to media changes
//         this._fuseMediaWatcherService.onMediaChange$
//             .pipe(takeUntil(this._unsubscribeAll))
//             .subscribe(({ matchingAliases }) => {
//                 // Check if the screen is small
//                 this.isScreenSmall = !matchingAliases.includes('md');
//             });
//     }

//     /**
//      * On destroy
//      */
//     ngOnDestroy(): void {
//         // Unsubscribe from all subscriptions
//         this._unsubscribeAll.next(null);
//         this._unsubscribeAll.complete();
//     }

import { NgIf } from '@angular/common';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { FuseFullscreenComponent } from '@fuse/components/fullscreen';
import { FuseLoadingBarComponent } from '@fuse/components/loading-bar';
import {
    FuseNavigationService,
    FuseVerticalNavigationComponent,
} from '@fuse/components/navigation';
import { FuseMediaWatcherService } from '@fuse/services/media-watcher';
import { NavigationService } from 'app/core/navigation/navigation.service';
import { Navigation } from 'app/core/navigation/navigation.types';
import { UserService } from 'app/core/user/user.service';
import { User } from 'app/core/user/user.types';
import { LanguagesComponent } from 'app/layout/common/languages/languages.component';
import { MessagesComponent } from 'app/layout/common/messages/messages.component';
import { NotificationsComponent } from 'app/layout/common/notifications/notifications.component';
import { QuickChatComponent } from 'app/layout/common/quick-chat/quick-chat.component';
import { SearchComponent } from 'app/layout/common/search/search.component';
import { ShortcutsComponent } from 'app/layout/common/shortcuts/shortcuts.component';
import { UserComponent } from 'app/layout/common/user/user.component';
import { filter, Subject, takeUntil } from 'rxjs';

@Component({
    selector: 'classy-layout',
    templateUrl: './classy.component.html',
    encapsulation: ViewEncapsulation.None,
    imports: [
        FuseLoadingBarComponent,
        FuseVerticalNavigationComponent,
        // NotificationsComponent,
        UserComponent,
        MatIconModule,
        MatButtonModule,
        // LanguagesComponent,
        FuseFullscreenComponent,
        SearchComponent,
        // ShortcutsComponent,
        // MessagesComponent,
        RouterOutlet,
        NgIf,
        // QuickChatComponent,
    ],
})
export class ClassyLayoutComponent implements OnInit, OnDestroy {
    isScreenSmall: boolean;
    navigation: Navigation;
    user: User; // This will be populated by the UserService
    displayName: string = ''; // Initialize
    email: string = ''; // Initialize
    avatar: string = ''; // Initialize
    private _unsubscribeAll: Subject<any> = new Subject<any>();

    /**
     * Constructor
     */
    constructor(
        private _activatedRoute: ActivatedRoute,
        private _router: Router,
        private _navigationService: NavigationService,
        private _userService: UserService,
        private _fuseMediaWatcherService: FuseMediaWatcherService,
        private _fuseNavigationService: FuseNavigationService,
        private _changeDetectorRef: ChangeDetectorRef
    ) { }

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for current year
     */
    get currentYear(): number {
        return new Date().getFullYear();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    // ngOnInit(): void {
    //     this._userService
    //         .get()            // ← this loads the user into the ReplaySubject
    //         .subscribe();

    //     // Subscribe to navigation data
    //     this._navigationService.navigation$
    //         .pipe(takeUntil(this._unsubscribeAll))
    //         .subscribe((navigation: Navigation) => {
    //             this.navigation = navigation;
    //         });
    //     debugger
    //     // Subscribe to the user service
    //     this._userService.user$
    //         .pipe(
    //             filter((u): u is User => !!u),
    //             takeUntil(this._unsubscribeAll)
    //         )
    //         .subscribe(user => {
    //             this.user = user;
    //             // use displayName || name
    //             this.displayName = user.displayName ?? user.name;
    //             this.email = user.email;
    //             this.avatar = user.avatar;
    //             this._changeDetectorRef.markForCheck();
    //         });


    //     // Subscribe to media changes
    //     this._fuseMediaWatcherService.onMediaChange$
    //         .pipe(takeUntil(this._unsubscribeAll))
    //         .subscribe(({ matchingAliases }) => {
    //             // Check if the screen is small
    //             this.isScreenSmall = !matchingAliases.includes('md');
    //         });
    // }
    ngOnInit(): void {
        // 1) Rehydrate the user from the API into the ReplaySubject
        this._userService.get().subscribe();

        // 2) Then subscribe as usual
        this._userService.user$
            .pipe(
                filter((u): u is User => !!u),
                takeUntil(this._unsubscribeAll)
            )
            .subscribe(user => {
                this.user = user;
                this.displayName = user.displayName ?? user.name;
                this.email = user.email;
                this.avatar = user.avatar;
                this._changeDetectorRef.markForCheck();
            });

        // 3) (rest of your existing ngOnInit: navigation + media watchers)
        this._navigationService.navigation$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe(nav => this.navigation = nav);

        this._fuseMediaWatcherService.onMediaChange$
            .pipe(takeUntil(this._unsubscribeAll))
            .subscribe(({ matchingAliases }) => {
                this.isScreenSmall = !matchingAliases.includes('md');
            });
    }


    /**
     * On destroy
     */
    ngOnDestroy(): void {
        // Unsubscribe from all subscriptions
        this._unsubscribeAll.next(null);
        this._unsubscribeAll.complete();
    }


    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Toggle navigation
     *
     * @param name
     */
    toggleNavigation(name: string): void {
        // Get the navigation
        const navigation =
            this._fuseNavigationService.getComponent<FuseVerticalNavigationComponent>(
                name
            );

        if (navigation) {
            // Toggle the opened status
            navigation.toggle();
        }
    }
}
