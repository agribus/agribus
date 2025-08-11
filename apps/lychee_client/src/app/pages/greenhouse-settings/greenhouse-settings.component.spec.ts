import { ComponentFixture, TestBed } from "@angular/core/testing";

import { GreenhouseSettingsComponent } from "./greenhouse-settings.component";

describe("GreenhouseSettingsComponent", () => {
  let component: GreenhouseSettingsComponent;
  let fixture: ComponentFixture<GreenhouseSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreenhouseSettingsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GreenhouseSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
