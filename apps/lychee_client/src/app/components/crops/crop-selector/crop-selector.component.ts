import { Component, EventEmitter, inject, Output, signal, computed } from "@angular/core";
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
import { CropsService } from "@services/crops/crops.service";

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

  public crops = signal<Crop[]>([]);
  public openSheet = signal<boolean>(false);
  public loading = signal<boolean>(false);

  @Output() cropSelected = new EventEmitter<Crop>();

  public optionsSheet = computed<Partial<TuiSheetDialogOptions>>(() => ({
    label: this.translateService.instant("shared.dialog.create.crop"),
    closeable: true,
  }));

  public open(): void {
    this.loading.set(true);
    this.crops.set([]);

    this.cropsService.identifyPlantFromCamera().subscribe({
      next: (crops: Crop[]) => {
        this.crops.set(crops);
        this.loading.set(false);
      },
      error: () => {
        this.crops.set([]);
        this.loading.set(false);
      },
    });

    this.openSheet.set(true);
  }

  private reset(): void {
    this.crops.set([]);
    this.loading.set(false);
  }

  public selectCrop(crop: Crop): void {
    this.cropSelected.emit(crop);
    this.openSheet.set(false);
  }
}
