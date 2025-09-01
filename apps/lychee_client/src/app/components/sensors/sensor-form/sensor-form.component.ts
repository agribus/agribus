import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Sensor } from "@interfaces/sensor.interface";
import { Component, EventEmitter, inject, Input, OnInit, Output } from "@angular/core";
import { TuiSheetDialog, TuiSheetDialogOptions } from "@taiga-ui/addon-mobile";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TuiButton, TuiTextfield } from "@taiga-ui/core";

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

  public form!: FormGroup;

  protected readonly optionsSheet: Partial<TuiSheetDialogOptions> = {
    label: this.translateService.instant("shared.dialog.create.sensor"),
    closeable: true,
  };

  private _sensor: Sensor | null = null;

  @Input()
  set sensor(value: Sensor | null) {
    this._sensor = value;
    if (this.form) {
      this.patchForm();
    }
  }

  get sensor(): Sensor | null {
    return this._sensor;
  }

  ngOnInit(): void {
    this.initForm();
    if (this.sensor) {
      this.patchForm();
    }
  }

  private initForm(): void {
    this.form = this.fb.group({
      id: [null],
      sourceAddress: ["", Validators.required],
      name: ["", Validators.required],
      sensorModel: "RuuviTag",
      isActive: true,
    });
  }

  private patchForm(): void {
    this.form.patchValue({
      id: this.sensor?.id ?? null,
      sourceAddress: this.sensor?.sourceAddress ?? "",
      name: this.sensor?.name ?? "",
      sensorModel: "RuuviTag",
      isActive: true,
    });
  }

  public open(): void {
    if (!this.sensor) {
      this.form.reset({
        id: null,
        sourceAddress: "",
        name: "",
      });
    } else {
      this.patchForm();
    }

    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.openSheet = true;
  }

  public save(): void {
    if (this.form.valid) {
      this.sensorSubmitted.emit(this.form.value);
      this.openSheet = false;
    }
  }

  private resetForm(): void {
    this.sensor = null;
    this.initForm();
  }
}
