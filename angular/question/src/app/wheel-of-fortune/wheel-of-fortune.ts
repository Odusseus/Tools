import { Component } from '@angular/core';
import { Persons } from '../model/persons';
import { Menu } from '../menu/menu';

@Component({
  imports: [Menu],
  selector: 'app-wheel-of-fortune',
  styleUrl: './wheel-of-fortune.css',
  templateUrl: './wheel-of-fortune.html',
})
export class WheelOfFortune {
  persons: Persons = new Persons();
  selectedName: string = '';
  timer: any = null;
  isSpinning = false;

  getRandomPerson(): void {
    const persons = this.persons.getAll();

    if (persons.length === 0) {
      this.selectedName = 'No persons available.';
      return;
    }

    const randomIndex = Math.floor(Math.random() * persons.length);
    const person = persons[randomIndex];

    this.selectedName = person.name;
    person.incrementSelectedCount()

    this.persons.update(persons[randomIndex]);
    this.persons.save();
  
  }
  
  startSpinning(): void {
    if (this.isSpinning) {
      return;
    }

    this.isSpinning = true;

    this.timer = setInterval(() => {
      this.getRandomPerson();
    }, 100);
  }

  stopSpinning(): void {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }

    this.isSpinning = false;
  }
}
