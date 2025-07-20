import { Component, EventEmitter, inject, Input, OnInit, Output } from "@angular/core";
import { TuiSheetDialog, TuiSheetDialogOptions } from "@taiga-ui/addon-mobile";
import { FormBuilder, FormGroup, FormsModule } from "@angular/forms";
import { TuiButton, TuiTextfield } from "@taiga-ui/core";
import { TuiFiles, TuiInputDate, TuiInputNumber } from "@taiga-ui/kit";
import { Crop } from "@interfaces/crop.interface";

@Component({
  selector: "app-crop-form",
  imports: [
    FormsModule,
    TuiButton,
    TuiTextfield,
    TuiInputNumber,
    TuiInputDate,
    TuiFiles,
    TuiSheetDialog,
  ],
  templateUrl: "./crop-form.component.html",
  styleUrl: "./crop-form.component.scss",
})
export class CropFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  public openSheet = false;

  // Form data
  form = {
    commonName: "",
    scientificName: "",
    plantingDate: null as Date | null,
    quantity: 1,
    image: [] as File[],
  };

  cropForm: FormGroup = this.fb.group({});
  @Input() label: string = "";
  @Input() crop: Crop | null = null;

  @Output() cropCreated = new EventEmitter<any>();

  public optionsSheet: Partial<TuiSheetDialogOptions> = {
    label: this.label,
    closeable: true,
  };

  ngOnInit() {
    this.optionsSheet = {
      label: this.label,
      closeable: true,
    };

    this.cropForm = this.fb.group({
      commonName: this.crop?.commonName || "",
      scientificName: this.crop?.scientificName || "",
      plantingDate: this.crop?.date_plantation,
      quantity: this.crop?.quantity,
      image: [] as File[],
    });
  }

  open() {
    this.openSheet = true;
  }

  submit() {
    if (!this.form.commonName.trim()) return;

    const crop = {
      ...this.form,
      image: this.form.image[0] || null,
    };

    this.cropCreated.emit(crop);
    this.reset();
  }

  reset() {
    this.form = {
      commonName: "",
      scientificName: "",
      plantingDate: null,
      quantity: 1,
      image: [],
    };
    this.openSheet = false;
  }
}
