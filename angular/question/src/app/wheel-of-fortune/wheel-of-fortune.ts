//import { ChangeDetectorRef, Component } from '@angular/core';
import { Component, inject, signal } from '@angular/core';
import { PersonsStorage } from '../shared/services/persons-storage';
import { Persons } from '../model/persons';
import { Menu } from '../menu/menu';

@Component({
  standalone: true,
  imports: [Menu],
  selector: 'app-wheel-of-fortune',
  styleUrls: ['./wheel-of-fortune.css'],
  templateUrl: './wheel-of-fortune.html',
})
export class WheelOfFortune {
  persons: Persons = new Persons(inject(PersonsStorage));
  selectedName = signal('');
  timer: any = null;
  timer3w: any = null;
  isSpinning = signal(false);
  isSpinning3w = signal(false);
  totalSelections = signal(0);
  winners = signal<string[]>([]);

  maxPersons = 3;
  minRounds = 10;
  maxRounds = 30;
  randomRounds = 0;
  personCount = 0;
  roundCount = 0;

  //constructor(private cdr: ChangeDetectorRef) { }
  constructor() { }

  getRandomPerson(): void {
    const persons = this.persons.getAll();

    if (persons.length === 0) {
      this.selectedName.set('No persons available.');
      return;
    }

    const randomIndex = Math.floor(Math.random() * persons.length);
    const person = persons[randomIndex];

    this.selectedName.set(person.name);
    person.incrementSelectedCount();

    this.persons.update(person, false);
    this.totalSelections.update(value => value + 1);
  }

  getRandomPerson3w(): void {

    if (this.persons.getAll().length === 0) {
      this.selectedName.set('No persons available.');
      return;
    }

    const randomIndex = Math.floor(Math.random() * this.persons.getAll().length + 1);

    for (const person of this.persons.getAll()) {
      person.isLastSelected = false;
      if (person.id === randomIndex) {
        person.incrementSelectedCount();
        person.isLastSelected = true;
        this.totalSelections.update(value => value + 1);
        this.roundCount++;
        if (this.roundCount == this.randomRounds) {
          if (this.winners().includes(person.name)) {
            this.roundCount = 0;
            this.roundCount--;
          } else {
            this.roundCount = 0;
            this.winners.update(value => [...value, person.name]);
            this.personCount++;
          }
        }
      }
      // this.cdr.markForCheck();
    }
    if (this.personCount == this.maxPersons) {
      this.stopSpinning();
    }
  }
  startSpinning(): void {
    if (this.isSpinning()) {
      return;
    }

    this.isSpinning.set(true);
    this.isSpinning3w.set(false);
    this.tick();
    this.winners.set([]);
  }

  startSpinning3w(): void {
    if (this.isSpinning()) {
      return;
    }

    this.isSpinning.set(false);
    this.isSpinning3w.set(true);
    this.maxPersons = 3;
    this.minRounds = 10;
    this.maxRounds = 30;
    this.personCount = 0;
    this.randomRounds = Math.floor(Math.random() * (this.maxRounds - this.minRounds + 1)) + this.minRounds;
    this.tick3w();
    this.winners.set([]);
  }

  tick(): void {
    if (!this.isSpinning()) {
      return;
    }
    this.getRandomPerson();
    this.timer = setTimeout(() => this.tick(), 300);
  }

  tick3w(): void {
    if (!this.isSpinning3w()) {
      return;
    }
    this.getRandomPerson3w();
    this.timer3w = setTimeout(() => this.tick3w(), 300);
  }

  stopSpinning(): void {
    this.isSpinning.set(false);
    this.isSpinning3w.set(false);

    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }
    this.persons.save();
  }

}
