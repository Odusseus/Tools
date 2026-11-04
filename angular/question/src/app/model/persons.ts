import { Person } from './person';
import { PersonsStorage } from '../shared/services/persons-storage';

export class Persons {
    private persons: Person[] = [];

    constructor(private storage: PersonsStorage) {
        this.load();
    }

    private load(): void {
        this.persons = this.storage.load();
    }

    save(): void {
        this.storage.save(this.persons);
    }

    getAll(): Person[] {
        return this.persons;
    }

    add(person: Person): void {
        this.persons.push(person);
        this.renummerateIds();
        this.save();
    }

    remove(id: number): void {
        this.persons = this.persons.filter(person => person.id !== id);
        this.renummerateIds();
        this.save();
    }

    update(person: Person, isSave: boolean = true): void {
        const index = this.persons.findIndex(p => p.id === person.id);
        if (index !== -1) {
            this.persons[index] = person;
            if (isSave) {
                this.save();
            }
        }
    }

    removeAll(): void {
        this.persons = [];
        this.save();
    }

    renummerateIds(): void {
        this.persons.forEach((person, index) => {
            person.id = index + 1;
            person.selectedCount = 0;
            person.isLastSelected = false;
        });
    }
}