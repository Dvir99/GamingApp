import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { TestErrorComponent } from './error/test-error/test-error.component';
import { NotFoundComponent } from './error/not-found/not-found.component';
import { ServerErrorComponent } from './error/server-error/server-error.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './_guards/admin.guard';

const routes: Routes = [
  {path: "", component: HomeComponent},
  {path: "", 
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {path: "members", component: MemberListComponent},
      {path: "members/:username", component: MemberDetailsComponent, resolve: {member: MemberDetailedResolver}},
      {path: "member/edit", component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
      {path: "lists", component: ListsComponent},
      {path: "messages", component: MessagesComponent},
      {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]}
    ]
  },
  {path: "errors", component: TestErrorComponent},
  {path: "not-found", component: NotFoundComponent},
  {path: "server-error", component: ServerErrorComponent},
  // {path: "**", component: NotFoundComponent, pathMatch:'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
