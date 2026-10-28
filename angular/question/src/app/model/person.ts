export class Person {
    id: number;
    name: string;
    selectedCount: number;

    constructor(
        id: number,
        name: string,
        selectedCount: number
    ) {
        this.id = id;
        this.name = name;
        this.selectedCount = selectedCount;
    }

    incrementSelectedCount() {
        this.selectedCount++;
    }
}