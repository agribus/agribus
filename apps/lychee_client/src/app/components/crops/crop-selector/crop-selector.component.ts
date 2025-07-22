import { Component, EventEmitter, inject, Output } from "@angular/core";
import { DecimalPipe } from "@angular/common";
import { FormsModule } from "@angular/forms";
import {
  TuiButton,
  TuiLabel,
  TuiLoader,
  TuiTextfield,
  TuiTextfieldComponent,
} from "@taiga-ui/core";
import {
  TuiChevron,
  TuiDataListWrapperComponent,
  TuiInputNumberDirective,
  TuiInputNumberStep,
  TuiProgress,
  TuiProgressBar,
  TuiSelectDirective,
} from "@taiga-ui/kit";
import { TuiSheetDialog, TuiSheetDialogOptions } from "@taiga-ui/addon-mobile";
import { Crop } from "@interfaces/crop.interface";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { CropsService } from "@services/crops.service";

@Component({
  selector: "app-crop-selector",
  imports: [
    DecimalPipe,
    FormsModule,
    TuiButton,
    TuiChevron,
    TuiDataListWrapperComponent,
    TuiInputNumberDirective,
    TuiInputNumberStep,
    TuiLabel,
    TuiLoader,
    TuiProgressBar,
    TuiSelectDirective,
    TuiSheetDialog,
    TuiTextfieldComponent,
    TranslatePipe,
    TuiTextfield,
    TuiProgress,
  ],
  templateUrl: "./crop-selector.component.html",
  styleUrl: "./crop-selector.component.scss",
})
export class CropSelectorComponent {
  private readonly translateService = inject(TranslateService);
  private readonly cropsService = inject(CropsService);

  public crops: Crop[] = [];
  private _openSheet = false;
  public loading = false;

  @Output() cropSelected = new EventEmitter<Crop>();

  public get optionsSheet(): Partial<TuiSheetDialogOptions> {
    return {
      label: this.translateService.instant("components.crops.crop-selector.dialogLabel"),
      closeable: !this.loading && this.crops.length === 0,
    };
  }

  public get openSheet(): boolean {
    return this._openSheet;
  }

  public set openSheet(value: boolean) {
    if (!value && this._openSheet) {
      this.reset();
    }
    this._openSheet = value;
  }

  public open(): void {
    this.loading = true;
    this.crops = [];

    this.cropsService.identifyPlantFromCamera().subscribe({
      next: (crops: Crop[]) => {
        this.crops = crops;
        this.loading = false;
      },
      error: () => {
        this.crops = [];
        this.loading = false;
      },
    });

    this.openSheet = true;
  }

  private reset(): void {
    this.crops = [];
    this.loading = false;
  }

  public selectCrop(crop: Crop): void {
    this.cropSelected.emit(crop);
    this.openSheet = false;
  }
}
