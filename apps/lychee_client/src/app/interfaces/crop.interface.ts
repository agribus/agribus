import { CropGrowthConditions } from "@interfaces/crop-growth-conditions.interface";

export interface Crop {
  commonName: string;
  commonNames: string[];
  scientificName: string;
  plantingDate?: Date;
  score: number;
  quantity?: number;
  imageUrl?: string;
  idealConditions?: CropGrowthConditions;
}
