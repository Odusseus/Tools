export class Person {
    id: number;
    name: string;
    selectedCount: number;
    isLastSelected: boolean;

    constructor(
        id: number,
        name: string,
        selectedCount: number,
        isLastSelected: boolean
    ) {
        this.id = id;
        this.name = name;
        this.selectedCount = selectedCount;
        this.isLastSelected = isLastSelected;
    }

    incrementSelectedCount() {
        this.selectedCount++;
    }
}