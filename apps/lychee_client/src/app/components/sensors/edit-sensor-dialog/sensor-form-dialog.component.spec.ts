import { ComponentFixture, TestBed } from "@angular/core/testing";

import { SensorFormDialogComponent } from "./sensor-form-dialog.component";

describe("EditSensorDialogComponent", () => {
  let component: SensorFormDialogComponent;
  let fixture: ComponentFixture<SensorFormDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SensorFormDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SensorFormDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
