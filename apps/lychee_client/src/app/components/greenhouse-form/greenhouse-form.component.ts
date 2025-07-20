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
import { SensorFormComponent } from "@components/sensor-form/sensor-form.component";
import { TuiCard, TuiCardLarge, TuiCell } from "@taiga-ui/layout";
import { TuiSwipeActions, TuiSwipeActionsAutoClose } from "@taiga-ui/addon-mobile";
import { Sensor } from "@interfaces/sensor.interface";
import { Crop } from "@interfaces/crop.interface";
import { ScanQrCodeComponent } from "@components/scan-qr-code/scan-qr-code.component";

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
  ],
  templateUrl: "./greenhouse-form.component.html",
  styleUrl: "./greenhouse-form.component.scss",
})
export class GreenhouseFormComponent {
  private fb = inject(FormBuilder);
  private translateService = inject(TranslateService);
  private readonly alerts = inject(TuiAlertService);
  private readonly cdr = inject(ChangeDetectorRef);

  protected readonly step = signal(0);

  protected readonly greenhouseForm: FormGroup;

  public sensors: Sensor[] = [{ id: 1, sourceAddress: "1234567890", name: "Sensor 1" }];
  public editingSensor: Sensor | null = null;

  public crops: Crop[] = [];

  public isScanning = false;

  get step0(): FormGroup {
    return this.greenhouseForm.get("step0") as FormGroup;
  }
  get step1(): FormGroup {
    return this.greenhouseForm.get("step1") as FormGroup;
  }
  get step2(): FormGroup {
    return this.greenhouseForm.get("step2") as FormGroup;
  }

  @ViewChild("sensorForm") sensorForm?: { open: () => void };

  constructor() {
    this.greenhouseForm = this.fb.group({
      step0: this.fb.group({
        name: ["", Validators.required],
        location: [null, Validators.required],
      }),
      step1: this.fb.group({
        crops: [[]],
      }),
      step2: this.fb.group({
        sensor: [[], Validators.required],
      }),
    });
  }

  // Navigation
  protected next(): void {
    this.step.update(i => i + 1);
  }

  protected previous(): void {
    this.step.update(i => i - 1);
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
  /**
   * Ajoute ou met à jour un capteur.
   * @param sensor Le capteur à ajouter ou modifier.
   */
  public upsertSensor(sensor: Sensor): void {
    if (!sensor.sourceAddress?.trim()) return;

    const duplicate = this.hasDuplicateSourceAddress(sensor);
    if (duplicate) {
      this.alerts
        .open(this.translateService.instant("components.greenhouse-form.alert.duplicate-sensor"), {
          appearance: "warning",
          label: this.translateService.instant("components.ui.alert.warning"),
        })
        .subscribe();
      return;
    }

    const index = this.sensors.findIndex(s => s.id === sensor.id);

    // Capteur à modifier
    if (index !== -1) {
      this.sensors[index] = { ...this.sensors[index], ...sensor };

      this.alerts
        .open(this.translateService.instant("components.greenhouse-form.alert.sensor-updated"), {
          appearance: "success",
          label: this.translateService.instant("components.ui.alert.success"),
        })
        .subscribe();
    }
    // Nouveau capteur
    else {
      const newSensor: Sensor = {
        id: this.generateSensorId(),
        name: sensor.name || this.getDefaultSensorName(),
        sourceAddress: sensor.sourceAddress,
      };

      this.sensors.push(newSensor);

      this.alerts
        .open(this.translateService.instant("components.greenhouse-form.alert.sensor-added"), {
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
        this.translateService.instant("components.greenhouse-form.alert.remove-sensor") +
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

  /**
   * Vérifie s’il existe un doublon de sourceAddress (sauf pour le capteur en cours de modification).
   */
  private hasDuplicateSourceAddress(sensor: Sensor): boolean {
    return this.sensors.some(s => s.sourceAddress === sensor.sourceAddress && s.id !== sensor.id);
  }

  /**
   * Génère un nouvel ID pour un capteur.
   */
  private generateSensorId(): number {
    return this.sensors.length ? Math.max(...this.sensors.map(s => s.id)) + 1 : 1;
  }

  /**
   * Retourne un nom par défaut localisé pour un capteur.
   */
  private getDefaultSensorName(): string {
    const label = this.translateService.instant("components.greenhouse-form.sensor");
    return `${label} ${this.sensors.length + 1}`;
  }
}
