import { ComponentFixture, TestBed } from '@angular/core/testing';
import { WheelOfFortune } from './wheel-of-fortune';

describe('WheelOfFortune', () => {
  let component: WheelOfFortune;
  let fixture: ComponentFixture<WheelOfFortune>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WheelOfFortune],
    }).compileComponents();

    fixture = TestBed.createComponent(WheelOfFortune);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
