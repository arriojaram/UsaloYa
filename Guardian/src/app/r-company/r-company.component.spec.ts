import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RCompanyComponent } from './r-company.component';

describe('RCompanyComponent', () => {
  let component: RCompanyComponent;
  let fixture: ComponentFixture<RCompanyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RCompanyComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RCompanyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
