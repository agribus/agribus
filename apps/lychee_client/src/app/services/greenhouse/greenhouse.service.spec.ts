import { TestBed } from "@angular/core/testing";

import { GreenhouseService } from "./greenhouse.service";

describe("GreenhouseService", () => {
  let service: GreenhouseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GreenhouseService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
