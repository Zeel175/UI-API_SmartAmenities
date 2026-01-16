// import { NgModule } from '@angular/core';
// import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
// import { EventService } from './event';
// import {
//     UserAuthService, BaseService,
//     ListService, AccountService, NotificationService
// } from './service';
// import { AuthGuard, PageAuthGuard } from './guards';

// @NgModule({ imports: [], providers: [
//         EventService,
//         UserAuthService,
//         BaseService,
//         ListService,
//         AccountService,
//         AuthGuard,
//         NotificationService,
//         PageAuthGuard,
//         provideHttpClient(withInterceptorsFromDi())
//     ] })
// export class CoreModule { }
// src/app/core/core.module.ts
import { NgModule } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor }              from './auth/auth.interceptor';
import { EventService }                 from './event';
import { BaseService, ListService,
         AccountService, NotificationService } from './service';
import { AuthGuard, PageAuthGuard }     from './guards';

@NgModule({
  providers: [
    // your other core-level services
    EventService,
    BaseService,
    ListService,
    AccountService,
    NotificationService,
    AuthGuard,
    PageAuthGuard,

    // <-- register our interceptor here:
    provideHttpClient(
      withInterceptors([ authInterceptor ])
    )
  ]
})
export class CoreModule {}
