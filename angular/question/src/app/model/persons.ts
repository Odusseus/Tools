import { Person } from './person';

export class Persons {
    private persons: Person[] = [];

    constructor() {
        this.load();
    }

    private load(): void {
        const storedPersons = localStorage.getItem('persons');
        if (storedPersons) {
            const parsed = JSON.parse(storedPersons) as Array<{
                id: number;
                name: string;
                selectedCount: number;
            }>;

            this.persons = parsed.map(
                person => new Person(person.id, person.name, person.selectedCount)
            );
        } else {
            this.persons = []
        }
    }

    save(): void {
        localStorage.setItem('persons', JSON.stringify(this.persons));
    }

    getAll(): Person[] {
        return this.persons;
    }

    add(person: Person): void {
        this.persons.push(person);
        this.save();
    }

    remove(id: number): void {
        this.persons = this.persons.filter(person => person.id !== id);
        this.save();
    }

    update(person: Person): void {
        const index = this.persons.findIndex(p => p.id === person.id);
        if (index !== -1) {
            this.persons[index] = person;
            this.save();
        }
    }

    removeAll(): void {
        this.persons = [];
        this.save();
    }
}