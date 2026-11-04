import { Service } from '@angular/core';
import { Person } from '../../model/person';

@Service()
export class PersonsStorage {
    private readonly storageKey = 'persons';

    save(persons: Person[]): void {
        localStorage.setItem(this.storageKey, JSON.stringify(persons));
    }

    load(): Person[] {
        const data = localStorage.getItem(this.storageKey);
        if (!data) {
            return [];
        }

        const parsedData = JSON.parse(data) as Array<{
            id: number;
            name: string;
            selectedCount: number;
            isLastSelected: boolean
        }>;

        return parsedData.map(
            item => new Person(item.id, item.name, item.selectedCount, item.isLastSelected)
        );
    }
}

