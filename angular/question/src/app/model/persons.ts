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
                isLastSelected: boolean;
            }>;

            this.persons = parsed.map(
                person => new Person( person.id, person.name, person.selectedCount, person.isLastSelected )
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