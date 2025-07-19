import { Component, inject } from "@angular/core";
import { Camera, CameraResultType } from "@capacitor/camera";
import { CropsService } from "@services/crops.service";
import { NgForOf, PercentPipe } from "@angular/common";

@Component({
  selector: "app-crops-capture",
  imports: [PercentPipe, NgForOf],
  templateUrl: "./crops-capture.component.html",
  styleUrl: "./crops-capture.component.scss",
})
export class CropsCaptureComponent {}
