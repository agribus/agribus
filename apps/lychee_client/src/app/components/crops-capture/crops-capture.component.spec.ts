import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CropsCaptureComponent } from "./crops-capture.component";

describe("CropsCaptureComponent", () => {
  let component: CropsCaptureComponent;
  let fixture: ComponentFixture<CropsCaptureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CropsCaptureComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CropsCaptureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
