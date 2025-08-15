import { TestBed } from "@angular/core/testing";

import { HeaderStateService } from "./header-state.service";

describe("HeaderStateService", () => {
  let service: HeaderStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HeaderStateService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
