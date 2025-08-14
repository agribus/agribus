import { ComponentFixture, TestBed } from "@angular/core/testing";

import { GreenhouseCreateComponent } from "./greenhouse-create.component";

describe("GreenhouseCreateComponent", () => {
  let component: GreenhouseCreateComponent;
  let fixture: ComponentFixture<GreenhouseCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreenhouseCreateComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GreenhouseCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
