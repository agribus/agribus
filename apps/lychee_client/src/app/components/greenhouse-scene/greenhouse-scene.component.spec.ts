import { ComponentFixture, TestBed } from "@angular/core/testing";

import { GreenhouseSceneComponent } from "./greenhouse-scene.component";

describe("GreenhouseSceneComponent", () => {
  let component: GreenhouseSceneComponent;
  let fixture: ComponentFixture<GreenhouseSceneComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GreenhouseSceneComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GreenhouseSceneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
