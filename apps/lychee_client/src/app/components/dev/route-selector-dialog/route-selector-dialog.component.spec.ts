import { ComponentFixture, TestBed } from "@angular/core/testing";

import { RouteSelectorDialogComponent } from "./route-selector-dialog.component";

describe("RouteSelectorDialogComponent", () => {
  let component: RouteSelectorDialogComponent;
  let fixture: ComponentFixture<RouteSelectorDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RouteSelectorDialogComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(RouteSelectorDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
