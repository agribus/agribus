import { TestBed } from "@angular/core/testing";

import { CropsService } from "./crops.service";

describe("CropsService", () => {
  let service: CropsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CropsService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
