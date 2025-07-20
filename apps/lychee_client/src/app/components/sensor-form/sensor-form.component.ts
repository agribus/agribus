import { Component, EventEmitter, inject, Input, OnInit, Output } from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { TuiButton, TuiTextfield } from "@taiga-ui/core";
import { Sensor } from "@interfaces/sensor.interface";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TuiSheetDialog, TuiSheetDialogOptions } from "@taiga-ui/addon-mobile";
import { Crop } from "@interfaces/crop.interface";

@Component({
  selector: "app-sensor-form",
  imports: [
    FormsModule,
    TuiTextfield,
    ReactiveFormsModule,
    TranslatePipe,
    TuiSheetDialog,
    TuiButton,
  ],
  templateUrl: "./sensor-form.component.html",
  styleUrl: "./sensor-form.component.scss",
})
export class SensorFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private translateService = inject(TranslateService);

  @Input() sensor: Sensor | null = null;
  @Output() sensorSubmitted = new EventEmitter<Sensor>();

  private _openSheet = false;
  public get openSheet() {
    return this._openSheet;
  }
  public set openSheet(value: boolean) {
    if (!value && this._openSheet) {
      this.resetForm();
    }
    this._openSheet = value;
  }

  public form: FormGroup = this.fb.group({
    id: [null],
    sourceAddress: ["", Validators.required],
    name: ["", Validators.required],
  });

  protected readonly optionsSheet: Partial<TuiSheetDialogOptions> = {
    label: this.translateService.instant("components.crop-selector.label"),
    closeable: true,
  };

  ngOnInit(): void {
    this.initForm();
  }

  private initForm(): void {
    this.form = this.fb.group({
      id: [this.sensor?.id ?? null],
      sourceAddress: [this.sensor?.sourceAddress ?? "", Validators.required],
      name: [this.sensor?.name ?? "", Validators.required],
    });
  }

  public open(): void {
    this.openSheet = true;
  }

  public save(): void {
    if (this.form.valid) {
      this.sensorSubmitted.emit(this.form.value);
      this.openSheet = false;
    }
  }

  private resetForm(): void {
    this.initForm();
    this.form.markAsPristine();
    this.form.markAsUntouched();
  }
}
