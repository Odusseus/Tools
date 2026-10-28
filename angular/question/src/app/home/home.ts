import { Component } from '@angular/core';
import { Menu } from '../menu/menu';

@Component({
  imports: [Menu],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {}