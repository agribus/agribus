import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnsupportedPlatformComponent } from './unsupported-platform.component';

describe('UnsupportedPlatformComponent', () => {
  let component: UnsupportedPlatformComponent;
  let fixture: ComponentFixture<UnsupportedPlatformComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UnsupportedPlatformComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UnsupportedPlatformComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
