import { Routes } from '@angular/router';
import { Home } from './home/home';
import { PersonMaintenance } from './person-maintenance/person-maintenance';
import { WheelOfFortune } from './wheel-of-fortune/wheel-of-fortune';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'persons', component: PersonMaintenance },
  { path: 'wheel-of-fortune', component: WheelOfFortune }
];
