import { ChangeDetectorRef, Component } from '@angular/core';
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
  totalSelections = 0;

  constructor(private cdr: ChangeDetectorRef) {}

  getRandomPerson(): void {
    const persons = this.persons.getAll();

    if (persons.length === 0) {
      this.selectedName = 'No persons available.';
      return;
    }

    const randomIndex = Math.floor(Math.random() * persons.length);
    const person = persons[randomIndex];

    this.selectedName = person.name;
    person.incrementSelectedCount();

    this.persons.update(person, false);
    this.totalSelections++;
    this.cdr.markForCheck();
  }
  
  startSpinning(): void {
    if (this.isSpinning) {
      return;
    }

    this.isSpinning = true;
    this.tick();
  }

  tick(): void {
    if (!this.isSpinning) {
      return;
    }
    this.getRandomPerson();
    this.timer = setTimeout(() => this.tick(), 300);
  }

  stopSpinning(): void {
    this.isSpinning = false;

    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }
    this.persons.save();
  }

}
