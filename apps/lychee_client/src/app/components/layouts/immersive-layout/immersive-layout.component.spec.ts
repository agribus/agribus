import { ComponentFixture, TestBed } from "@angular/core/testing";

import { ImmersiveLayoutComponent } from "./immersive-layout.component";

describe("FullscreenLayoutComponent", () => {
  let component: ImmersiveLayoutComponent;
  let fixture: ComponentFixture<ImmersiveLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ImmersiveLayoutComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ImmersiveLayoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
