import { TestBed } from "@angular/core/testing";
import { CanActivateFn } from "@angular/router";

import { mobileOnlyGuard } from "./mobile-only.guard";

describe("mobileOnlyGuard", () => {
  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => mobileOnlyGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it("should be created", () => {
    expect(executeGuard).toBeTruthy();
  });
});
