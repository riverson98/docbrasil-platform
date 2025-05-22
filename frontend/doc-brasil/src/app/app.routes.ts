import { Routes } from '@angular/router';
import { AuthLoginComponent } from './modules/pages/auth-login/auth-login.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: "",
        title: "DOC - Brasil",
        component: AuthLoginComponent,
        canActivate: [authGuard]
    },
    {
        path: "painel",
        title: "DOC - Painel",
        loadComponent: () => import("./modules/pages/home/home.component").then(m => m.HomeComponent),
        canActivate: [authGuard],
        children: [
            { 
                path: '', redirectTo: 'associados', pathMatch: 'full' 
            },
            {
                path: "membros",
                title: "DOC - Membros",
                loadComponent: () => import("./modules/pages/home/members/members.component").then(m => m.MembersComponent)
            },
            {
                path: "associados",
                title: "DOC - Associados",
                loadComponent: () => import("./modules/pages/home/associates/associates.component").then(m => m.AssociatesComponent)
            },
            {
                path: "senha",
                title: "DOC - Segurança",
                loadComponent: () => import("./modules/pages/home/password/password.component").then(m => m.PasswordComponent)
            },
            {
                path: "perfil",
                title: "DOC - Perfil",
                loadComponent: () => import("./modules/pages/home/profile/profile.component").then(m => m.ProfileComponent)
            }
        ]
    },
    { path: '**', redirectTo: '/' }
];
