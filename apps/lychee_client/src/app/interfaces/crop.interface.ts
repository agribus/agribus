import { CropGrowthConditions } from "@interfaces/crop-growth-conditions.interface";

export interface Crop {
  commonName: string;
  commonNames: string[];
  scientificName: string;
  date_plantation?: Date;
  score: number;
  quantity?: number;
  imageUrl?: string;
  cropGrowthConditions?: CropGrowthConditions;
}
