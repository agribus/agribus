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
  ],
  templateUrl: "./crop-selector.component.html",
  styleUrl: "./crop-selector.component.scss",
})
export class CropSelectorComponent {
  private readonly translateService = inject(TranslateService);
  private readonly cropsService = inject(CropsService);

  public crops: Crop[] = [];
  public openSheet = false;
  public loading = false;

  @Output() cropSelected = new EventEmitter<Crop>();

  protected readonly optionsSheet: Partial<TuiSheetDialogOptions> = {
    label: this.translateService.instant("components.crop-selector.label"),
    closeable: !this.loading && !this.crops.length ? true : false,
  };

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

  public selectCrop(crop: Crop): void {
    this.cropSelected.emit(crop);
    this.openSheet = false;
  }
}
