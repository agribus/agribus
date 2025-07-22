import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CropFormComponent } from "./crop-form.component";

describe("CropFormComponent", () => {
  let component: CropFormComponent;
  let fixture: ComponentFixture<CropFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CropFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CropFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
