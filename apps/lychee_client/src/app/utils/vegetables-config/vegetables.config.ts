export interface VegetableConfig {
  name: string;
  modelPath: string;
}

export const VEGETABLES: Record<string, VegetableConfig> = {
  carrot: { name: "carrot", modelPath: "./models/carrot.glb" },
};
