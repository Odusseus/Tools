import { TestBed } from '@angular/core/testing';
import { PersonsStorage } from './persons-storage';

describe('PersonsStorage', () => {
  let service: PersonsStorage;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PersonsStorage);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
