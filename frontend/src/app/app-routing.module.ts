// import { NgModule } from '@angular/core';
// import { RouterModule, Routes } from '@angular/router';
// import { ExampleComponent } from './modules/admin/example/example.component';

// const routes: Routes = [
//   {
//     path: 'signed-in-redirect',
//     redirectTo: 'user/list',
//     pathMatch: 'full'
//   },
//    { path: 'example', component: ExampleComponent },
//   { path: '', redirectTo: 'user/list', pathMatch: 'full' },
//   { path: 'user', loadChildren: () => import('./modules/user/user.module').then(m => m.UserModule) },
//   { path: '', redirectTo: 'role/list', pathMatch: 'full' },
//   { path: 'role', loadChildren: () => import('./modules/role/role.module').then(m => m.RoleModule) },
//   { path: '', redirectTo: 'designation/list', pathMatch: 'full' },
//   { path: 'designation', loadChildren: () => import('./modules/designation/designation.module').then(m => m.DesignationModule) },
//   { path: '', redirectTo: 'projectmaster/list', pathMatch: 'full' },
//   { path: 'projectmaster', loadChildren: () => import('./modules/projectmaster/projectmaster.module').then(m => m.ProjectMasterModule) },
//   { path: '', redirectTo: 'location/list', pathMatch: 'full' },
//   { path: 'location', loadChildren: () => import('./modules/location/location.module').then(m => m.LocationModule) },
//   { path: '', redirectTo: 'service-category/list', pathMatch: 'full' },
//   { path: 'service-category', loadChildren: () => import('./modules/service-category/service-category.module').then(m => m.ServiceCategoryModule) },
//   { path: '', redirectTo: 'groupandlineitem/list', pathMatch: 'full' },
//   { path: 'groupandlineitem', loadChildren: () => import('./modules/groupandlineitem/groupandlineitem.module').then(m => m.GroupLineItemModule) },
//   { path: '', redirectTo: 'workflow/list', pathMatch: 'full' },
//   { path: 'workflow', loadChildren: () => import('./modules/workflow/workflow.module').then(m => m.WorkflowModule) },
//   { path: '', redirectTo: 'projectassignment/list', pathMatch: 'full' },
//   { path: 'projectassignment', loadChildren: () => import('./modules/projectassignment/projectassignment.module').then(m => m.ProjectAssignmentModule) },
//   { path: '', redirectTo: 'budget-entry-screen/list', pathMatch: 'full' },
//   { path: 'projectassignment', loadChildren: () => import('./modules/budget-entry-screen/budget-entry-screen.module').then(m => m.BudgetEntryScreenModule) },



// ];

// @NgModule({
//   imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
//   exports: [RouterModule]
// })
// export class AppRoutingModule { }