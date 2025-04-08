import { Routes } from '@angular/router';
import { RegisterFormComponent } from './modules/pages/register-form/register-form.component';

export const routes: Routes = [
    {
        path: "",
        title: "DOC - Brasil",
        component: RegisterFormComponent
    },
    {
        path: "formulario-de-denuncia",
        title: "DOC - Denuncia",
        loadComponent: () => import("./modules/pages/report-form/report-form.component").then(m => m.ReportFormComponent)
    },
    {
        path: "painel",
        title: "DOC - Associados",
        loadComponent: () => import("./modules/pages/home/home.component").then(m => m.HomeComponent)
    }
];
