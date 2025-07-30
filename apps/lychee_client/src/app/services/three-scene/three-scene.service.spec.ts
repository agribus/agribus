import { TestBed } from "@angular/core/testing";

import { ThreeSceneService } from "./three-scene.service";

describe("ThreeSceneService", () => {
  let service: ThreeSceneService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ThreeSceneService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });
});
