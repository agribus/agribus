import { TestBed } from "@angular/core/testing";

import { GreenhouseSceneService } from "./greenhouse-scene.service";

describe("GreenhouseSceneService", () => {
  let service: GreenhouseSceneService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GreenhouseSceneService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
