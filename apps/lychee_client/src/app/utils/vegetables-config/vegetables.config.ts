export interface CropConfig {
  name: string;
  modelPath: string;
}

export const CROPS: Record<string, CropConfig> = {
  carrot: { name: "carrot", modelPath: "./models/carrot.glb" },
};
