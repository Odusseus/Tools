import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PersonMaintenance } from './person-maintenance';

describe('PersonMaintenance', () => {
  let component: PersonMaintenance;
  let fixture: ComponentFixture<PersonMaintenance>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonMaintenance],
    }).compileComponents();

    fixture = TestBed.createComponent(PersonMaintenance);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
