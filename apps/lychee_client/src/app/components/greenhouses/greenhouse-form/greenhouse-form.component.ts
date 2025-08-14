import { ChangeDetectorRef, Component, inject, signal, ViewChild } from "@angular/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TuiAvatar, TuiStepper } from "@taiga-ui/kit";
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import {
  TuiAlertService,
  TuiAppearance,
  TuiButton,
  TuiIcon,
  TuiTextfield,
  TuiTitle,
} from "@taiga-ui/core";
import { SensorFormComponent } from "@components/sensors/sensor-form/sensor-form.component";
import { TuiCard, TuiCardLarge, TuiCell } from "@taiga-ui/layout";
import { TuiSwipeActions, TuiSwipeActionsAutoClose } from "@taiga-ui/addon-mobile";
import { Sensor } from "@interfaces/sensor.interface";
import { Crop } from "@interfaces/crop.interface";
import { ScanQrCodeComponent } from "@components/ui/mobile/scan-qr-code/scan-qr-code.component";
import { CropFormComponent } from "@components/crops/crop-form/crop-form.component";
import { CropSelectorComponent } from "@components/crops/crop-selector/crop-selector.component";
import { PlatformService } from "@services/platform/platform.service";
import { Router } from "@angular/router";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Greenhouse } from "@interfaces/greenhouse.interface";

@Component({
  selector: "app-greenhouse-form",
  imports: [
    TuiStepper,
    ReactiveFormsModule,
    TuiTextfield,
    TuiButton,
    TranslatePipe,
    ScanQrCodeComponent,
    TuiAvatar,
    TuiAppearance,
    TuiSwipeActions,
    TuiCell,
    TuiTitle,
    TuiAppearance,
    TuiAvatar,
    TuiButton,
    TuiCardLarge,
    TuiCell,
    TuiSwipeActions,
    TuiTitle,
    TuiIcon,
    TuiSwipeActionsAutoClose,
    TuiCard,
    FormsModule,
    SensorFormComponent,
    CropFormComponent,
    CropSelectorComponent,
  ],
  templateUrl: "./greenhouse-form.component.html",
  styleUrl: "./greenhouse-form.component.scss",
})
export class GreenhouseFormComponent {
  private fb = inject(FormBuilder);
  private translateService = inject(TranslateService);
  private readonly alerts = inject(TuiAlertService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly platformService = inject(PlatformService);
  private readonly router = inject(Router);
  private readonly greenhouseService = inject(GreenhouseService);

  protected readonly step = signal(0);

  protected readonly greenhouseForm: FormGroup;

  public sensors: Sensor[] = [{ id: 1, sourceAddress: "1234567890", name: "Sensor 1" }];
  public editingSensor: Sensor | null = null;

  public crops: Crop[] = [];
  public editingCrop: Crop | null = null;

  public isScanning = false;
  public isMobile = false;

  @Input() greenhouse?: Greenhouse;
  @Input() isEditMode = false;

  @ViewChild("sensorForm") sensorForm?: { open: () => void };
  @ViewChild("cropForm") cropForm?: { open: () => void };
  @ViewChild("cropSelector") cropSelector?: { open: () => void };

  constructor() {
    this.isMobile = this.platformService.isMobile();

    this.greenhouseForm = this.fb.group({
      step0: this.fb.group({
        name: ["", Validators.required],
        city: ["Paris", Validators.required],
        country: ["France", Validators.required],
      }),
      step1: this.fb.group({
        crops: [[]],
      }),
      step2: this.fb.group({
        sensor: [[], Validators.required],
      }),
    });
  }

  /* ############################ Navigation ############################ */
  protected next(): void {
    this.step.update(i => i + 1);
  }

  protected previous(): void {
    this.step.update(i => i - 1);
  }

  /* ############################ Form ############################ */
  get step0(): FormGroup {
    return this.greenhouseForm.get("step0") as FormGroup;
  }
  get step1(): FormGroup {
    return this.greenhouseForm.get("step1") as FormGroup;
  }
  get step2(): FormGroup {
    return this.greenhouseForm.get("step2") as FormGroup;
  }

  protected isStepValid(): boolean {
    switch (this.step()) {
      case 0:
        return <boolean>(
          (this.greenhouseForm.get("step0.name")?.valid &&
            this.greenhouseForm.get("step0.city")?.valid &&
            this.greenhouseForm.get("step0.country")?.valid)
        );
      case 1:
        return this.crops.length >= 0;
      case 2:
        return this.sensors.length > 0;
      case 3:
        return true;
      default:
        return false;
    }
  }

  public submitForm(): void {
    this.step1.get("crops")?.setValue(this.crops);
    this.step2.get("sensor")?.setValue(this.sensors);

    if (this.greenhouseForm.valid) {
      const step0 = this.greenhouseForm.get("step0")?.value;
      const step1 = this.greenhouseForm.get("step1")?.value;
      const step2 = this.greenhouseForm.get("step2")?.value;

      const name = step0.name;
      const city = "Mulhouse";
      const country = "France";
      const crops = step1.crops;
      const sensors = step2.sensor;

      console.log("✅ Formulaire valide", this.greenhouseForm.value);
      this.alerts
        .open(this.translateService.instant("components.greenhouse-form.alert.create"), {
          appearance: "info",
          label: this.translateService.instant("components.ui.alert.info"),
        })
        .subscribe();
      this.router.navigate(["/home"]);
      this.greenhouseService.createGreenhouse(name, city, country, crops, sensors).subscribe({
        next: response => {
          console.log("Greenhouse created successfully", response);
        },
        error: err => {
          console.error("Error creating greenhouse", err);
        },
      });
      return;
    }

    this.handleInvalidForm();
  }

  private handleInvalidForm(): void {
    const invalidFields = this.getInvalidFields(this.greenhouseForm);
    const translatedLabels = this.translateInvalidFields(invalidFields);

    const message = `${this.translateService.instant("components.ui.alert.form-invalid")}: ${translatedLabels.join(", ")}`;

    this.alerts
      .open(message, {
        label: this.translateService.instant("components.ui.alert.error"),
        appearance: "error",
      })
      .subscribe();

    this.greenhouseForm.markAllAsTouched();
  }

  private getInvalidFields(formGroup: FormGroup): string[] {
    return Object.entries(formGroup.controls).flatMap(([key, control]) => {
      if (control instanceof FormGroup) {
        return this.getInvalidFields(control);
      }
      return control.invalid ? [key] : [];
    });
  }

  private translateInvalidFields(fields: string[]): string[] {
    const labels: Record<string, string> = {
      name: this.translateService.instant("components.greenhouses-form.label.name"),
      location: this.translateService.instant("components.greenhouses-form.label.location"),
      crops: this.translateService.instant("components.greenhouses-form.label.crops"),
      sensor: this.translateService.instant("components.greenhouses-form.label.sensors"),
    };

    return fields.map(field => labels[field] || field);
  }

  /* ############################## CROPS ############################## */
  public openCropForm(crop: Crop | null): void {
    this.editingCrop = crop;
    this.cdr.detectChanges();
    this.cropForm?.open();
  }

  public openCropSelector() {
    this.cropSelector?.open();
  }

  public onCropSelected(crop: Crop): void {
    this.upsertCrop(crop, false);
  }

  public onCropSaved(crop: Crop): void {
    const isEdit = !!this.editingCrop;
    this.upsertCrop(crop, isEdit);
    this.editingCrop = null;
  }

  public removeCrop(index: number): void {
    const crop = this.crops[index];
    if (!crop) return;

    this.alerts
      .open(
        this.translateService.instant("components.greenhouses-form.alert.remove-sensor") +
          crop.commonName,
        {
          appearance: "info",
          label: this.translateService.instant("components.ui.alert.info"),
        }
      )
      .subscribe();

    this.crops.splice(index, 1);
  }

  public upsertCrop(crop: Crop, isEdit: boolean): void {
    if (!crop || !crop.scientificName || !crop.commonName) return;

    const index = this.crops.findIndex(
      c => c.scientificName === crop.scientificName && c.commonName === crop.commonName
    );

    if (index === -1) {
      this.crops.push({ ...crop });
    } else {
      if (isEdit) {
        this.crops[index] = { ...this.crops[index], ...crop };
      } else {
        this.crops[index].quantity = (this.crops[index].quantity || 0) + (crop.quantity || 1);
      }
    }
  }

  /* ############################## SENSORS ############################## */
  public openScanQrCode() {
    this.isScanning = true;
  }

  public openSensorForm(sensor: Sensor | null): void {
    this.editingSensor = sensor;
    this.cdr.detectChanges();
    this.sensorForm?.open();
  }

  public upsertSensor(sensor: Sensor): void {
    if (!sensor.sourceAddress?.trim()) return;

    const duplicate = this.hasDuplicateSourceAddress(sensor);
    if (duplicate) {
      this.alerts
        .open(this.translateService.instant("components.greenhouses-form.alert.duplicate-sensor"), {
          appearance: "warning",
          label: this.translateService.instant("components.ui.alert.warning"),
        })
        .subscribe();
      return;
    }

    const index = this.sensors.findIndex(s => s.id === sensor.id);

    if (index !== -1) {
      this.sensors[index] = { ...this.sensors[index], ...sensor };

      this.alerts
        .open(this.translateService.instant("components.greenhouses-form.alert.sensor-updated"), {
          appearance: "success",
          label: this.translateService.instant("components.ui.alert.success"),
        })
        .subscribe();
    } else {
      const newSensor: Sensor = {
        id: this.generateSensorId(),
        name: sensor.name || this.getDefaultSensorName(),
        sourceAddress: sensor.sourceAddress,
      };

      this.sensors.push(newSensor);

      this.alerts
        .open(this.translateService.instant("components.greenhouses-form.alert.sensor-added"), {
          appearance: "success",
          label: this.translateService.instant("components.ui.alert.success"),
        })
        .subscribe();
    }
  }

  public removeSensor(index: number): void {
    const sensor = this.sensors[index];
    if (!sensor) return;

    this.alerts
      .open(
        this.translateService.instant("components.greenhouses-form.alert.remove-sensor") +
          sensor.name,
        {
          appearance: "info",
          label: this.translateService.instant("components.ui.alert.info"),
        }
      )
      .subscribe();

    this.sensors.splice(index, 1);
  }
  public onSensorSaved(sensor: Sensor): void {
    this.upsertSensor(sensor);
  }

  private hasDuplicateSourceAddress(sensor: Sensor): boolean {
    return this.sensors.some(s => s.sourceAddress === sensor.sourceAddress && s.id !== sensor.id);
  }

  private generateSensorId(): number {
    return this.sensors.length ? Math.max(...this.sensors.map(s => s.id)) + 1 : 1;
  }

  private getDefaultSensorName(): string {
    const label = this.translateService.instant("components.greenhouses-form.sensor");
    return `${label} ${this.sensors.length + 1}`;
  }
}
