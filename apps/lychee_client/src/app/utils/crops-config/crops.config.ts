export interface CropConfig {
  name: string;
  modelPath: string;
}

export const CROPS: Record<string, CropConfig> = {
  "daucus carota": { name: "carrot", modelPath: "./models/carrot.glb" },
  "lactuca sativa": { name: "lettuce", modelPath: "./models/lettuce.glb" },
  "fragaria × ananassa": { name: "strawberry", modelPath: "./models/strawberry.glb" },
  "solanum lycopersicum": { name: "tomato", modelPath: "./models/tomato.glb" },
  "allium cepa": { name: "onion", modelPath: "./models/onion.glb" },
  "solanum tuberosum": { name: "potato", modelPath: "./models/potato.glb" },
  leaf: { name: "leaf", modelPath: "./models/leaf.glb" },
};
