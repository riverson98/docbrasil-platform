import { Routes } from '@angular/router';
import { RegisterFormComponent } from './modules/pages/register-form/register-form.component';

export const routes: Routes = [
    {
        path: "",
        title: "DOC - Brasil",
        component: RegisterFormComponent
    },
    {
        path: "painel",
        title: "DOC - Painel",
        loadComponent: () => import("./modules/pages/home/home.component").then(m => m.HomeComponent),
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
];
