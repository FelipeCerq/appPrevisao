import { Routes } from '@angular/router';

import { Login } from './pages/login/login';
import { Cadastro } from './pages/cadastro/cadastro';
import { Home } from './pages/home/home';
import { Favoritos } from './pages/favoritos/favoritos';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'home'
  },
  {
    path: 'home',
    component: Home
  },
  {
    path: 'login',
    component: Login
  },
  {
    path: 'cadastro',
    component: Cadastro
  },
  {
    path: 'favoritos',
    component: Favoritos
  },
  {
    path: '**',
    redirectTo: 'home'
  }
];
