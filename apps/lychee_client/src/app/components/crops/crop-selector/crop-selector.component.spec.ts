import { ComponentFixture, TestBed } from "@angular/core/testing";

import { CropSelectorComponent } from "./crop-selector.component";

describe("CropSelectorComponent", () => {
  let component: CropSelectorComponent;
  let fixture: ComponentFixture<CropSelectorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CropSelectorComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CropSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
