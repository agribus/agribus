import { ComponentFixture, TestBed } from "@angular/core/testing";

import { GreenhouseFormComponent } from "./greenhouse-form.component";

describe("GreenhouseFormComponent", () => {
  let component: GreenhouseFormComponent;
  let fixture: ComponentFixture<GreenhouseFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreenhouseFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GreenhouseFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
