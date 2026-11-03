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
  timer3w: any = null;
  isSpinning = false;
  isSpinning3w = false;
  totalSelections = 0;
  winners: string[] = [];

  maxPersons = 3;
  minRounds = 10;
  maxRounds = 30;
  randomRounds = 0;
  personCount = 0;
  roundCount = 0;

  constructor(private cdr: ChangeDetectorRef) { }

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

  getRandomPerson3w(): void {

    if (this.persons.getAll().length === 0) {
      this.selectedName = 'No persons available.';
      return;
    }

    const randomIndex = Math.floor(Math.random() * this.persons.getAll().length + 1);

    for (const person of this.persons.getAll()) {
      person.isLastSelected = false;
      if (person.id === randomIndex) {
        person.incrementSelectedCount();
        person.isLastSelected = true;
        this.totalSelections++;
        this.roundCount++;
        if (this.roundCount == this.randomRounds) {
          if (this.winners.includes(person.name)) {
            this.roundCount = 0;
            this.roundCount--;
          } else {
            this.roundCount = 0;
            this.winners.push(person.name);
            this.personCount++;
          }
        }
      }
      this.cdr.markForCheck();
    }
    if (this.personCount == this.maxPersons) {
      this.stopSpinning();
    }
  }
  startSpinning(): void {
    if (this.isSpinning) {
      return;
    }

    this.isSpinning = true;
    this.isSpinning3w = false;
    this.tick();
    this.winners = [];
  }

  startSpinning3w(): void {
    if (this.isSpinning) {
      return;
    }

    this.isSpinning = false;
    this.isSpinning3w = true;
    this.maxPersons = 3;
    this.minRounds = 10;
    this.maxRounds = 30;
    this.personCount = 0;
    this.randomRounds = Math.floor(Math.random() * (this.maxRounds - this.minRounds + 1)) + this.minRounds;
    this.tick3w();
    this.winners = [];
  }

  tick(): void {
    if (!this.isSpinning) {
      return;
    }
    this.getRandomPerson();
    this.timer = setTimeout(() => this.tick(), 300);
  }

  tick3w(): void {
    if (!this.isSpinning3w) {
      return;
    }
    this.getRandomPerson3w();
    this.timer3w = setTimeout(() => this.tick3w(), 300);
  }

  stopSpinning(): void {
    this.isSpinning = false;
    this.isSpinning3w = false;

    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }
    this.persons.save();
  }

}
