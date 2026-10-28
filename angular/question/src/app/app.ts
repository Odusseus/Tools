import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PersonMaintenance } from './person-maintenance/person-maintenance';

@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  template: '<router-outlet></router-outlet>'
})
export class App {  
}
