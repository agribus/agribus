import { Component, EventEmitter, inject, Input, OnInit, Output } from "@angular/core";
import { TuiSheetDialog, TuiSheetDialogOptions } from "@taiga-ui/addon-mobile";
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { TuiButton, TuiTextfield } from "@taiga-ui/core";
import { TuiInputDate, TuiInputNumber } from "@taiga-ui/kit";
import { Crop } from "@interfaces/crop.interface";

@Component({
  selector: "app-crop-form",
  imports: [
    FormsModule,
    TuiButton,
    TuiTextfield,
    TuiInputNumber,
    TuiInputDate,
    TuiSheetDialog,
    ReactiveFormsModule,
  ],
  templateUrl: "./crop-form.component.html",
  styleUrl: "./crop-form.component.scss",
})
export class CropFormComponent implements OnInit {
  private fb = inject(FormBuilder);

  private _openSheet = false;
  public get openSheet() {
    return this._openSheet;
  }
  public set openSheet(value: boolean) {
    if (!value && this._openSheet) {
      this.reset();
    }
    this._openSheet = value;
  }

  @Input() label: string = "Ajouter une culture";

  @Input()
  set crop(value: Crop | null) {
    this._crop = value;
    if (this.form) {
      this.patchForm();
    }
  }
  get crop(): Crop | null {
    return this._crop;
  }
  private _crop: Crop | null = null;

  @Output() cropSubmitted = new EventEmitter<Crop>();

  public form!: FormGroup;
  public optionsSheet: Partial<TuiSheetDialogOptions> = {
    label: this.label,
    closeable: true,
  };

  ngOnInit(): void {
    console.log(this.crop);
    this.initForm();
    if (this.crop) {
      this.patchForm();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      commonName: ["", Validators.required],
      scientificName: [""],
      plantingDate: [null],
      quantity: [1, [Validators.min(1)]],
      image: [null],
    });
  }

  private patchForm(): void {
    this.form.patchValue({
      commonName: this.crop?.commonName ?? "",
      scientificName: this.crop?.scientificName ?? "",
      plantingDate: this.crop?.date_plantation ?? null,
      quantity: this.crop?.quantity ?? 1,
      image: null,
    });
  }

  public open(): void {
    if (!this.crop) {
      this.form.reset({
        commonName: "",
        scientificName: "",
        plantingDate: null,
        quantity: 1,
        image: null,
      });
    } else {
      this.patchForm();
    }
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.openSheet = true;
  }

  public submit(): void {
    if (this.form.valid) {
      this.cropSubmitted.emit(this.form.value);
      this.openSheet = false;
    }
  }

  public reset(): void {
    this.form.reset({
      commonName: "",
      scientificName: "",
      plantingDate: null,
      quantity: 1,
      image: null,
    });
  }
}
