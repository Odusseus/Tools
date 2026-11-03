import { Component } from '@angular/core';
import { Persons } from '../model/persons';
import { Person } from '../model/person';
import { FormsModule } from '@angular/forms';
import { Menu } from '../menu/menu';

@Component({
  selector: 'app-person-maintenance',
  standalone: true,
  imports: [FormsModule, Menu],
  templateUrl: './person-maintenance.html',
  styleUrl: './person-maintenance.css',
})
export class PersonMaintenance {
  persons = new Persons();
  name = '';
  editingId: number | null = null;

  addPerson(): void {
    const trimmedName = this.name.trim();
    if (!trimmedName) {
      return;
    }
    
    this.persons.add(new Person(0, trimmedName, 0, false));    
    this.name = '';
    this.persons.save();
  }

  deletePerson(id: number): void {
    this.persons.remove(id);
    this.persons.save();
  }

  editPerson(person: Person): void {
    this.editingId = person.id;
    this.name = person.name;
  }

  savePerson(): void {

    if (this.editingId === null) {
      return;
    }

    const person = this.persons.getAll().find(p => p.id === this.editingId);
    if (!person) {
      return;
    }
    person.name = this.name.trim();
    this.persons.save();

    this.clearForm();
  }

  clearForm(): void {
    this.editingId = null;
    this.name = '';
  }

  removeAll(): void {
    const confirmation = confirm('Are you sure you want to remove all persons?');
    if (confirmation) {
      this.persons.removeAll();
      this.persons.save();
    }
  }

  resetSelectedCounts(): void {
    const confirmation = confirm('Are you sure you want to reset the selected counts for all persons?');
    if (confirmation) {
      this.persons.getAll().forEach(person => {
        person.selectedCount = 0;
        this.persons.update(person, false);
      });
      this.persons.save();
    }
  }

}