import { Component, inject, Injector, signal } from "@angular/core";
import { TuiAvatar, TuiStepper } from "@taiga-ui/kit";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import {
  TuiAppearance,
  TuiButton,
  TuiDialogContext,
  TuiDialogService,
  TuiDialogSize,
  TuiIcon,
  TuiTextfield,
  TuiTitle,
} from "@taiga-ui/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { ScanQrCodeComponent } from "@components/scan-qr-code/scan-qr-code.component";
import { PolymorpheusComponent, PolymorpheusContent } from "@taiga-ui/polymorpheus";
import { take } from "rxjs";
import { Sensor } from "@interfaces/sensor.interface";
import { TuiCardLarge, TuiCell, TuiItemGroup } from "@taiga-ui/layout";
import { TuiSwipeActions } from "@taiga-ui/addon-mobile";
import { AsyncPipe } from "@angular/common";
import { SensorFormDialogComponent } from "@components/edit-sensor-dialog/sensor-form-dialog.component";

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
    TuiItemGroup,
    TuiIcon,
  ],
  templateUrl: "./greenhouse-form.component.html",
  styleUrl: "./greenhouse-form.component.scss",
})
export class GreenhouseFormComponent {
  protected readonly step = signal(0);

  protected readonly greenhouseForm: FormGroup;
  private fb = inject(FormBuilder);
  private readonly dialogs = inject(TuiDialogService);
  private readonly translate = inject(TranslateService);
  private injector = inject(Injector);

  public sensors: Sensor[] = [{ id: 1, sourceAddress: "1234567890", name: "Sensor 1" }];

  constructor() {
    this.greenhouseForm = this.fb.group({
      step0: this.fb.group({
        name: ["", Validators.required],
        location: [null, Validators.required],
      }),
      step1: this.fb.group({
        crops: this.fb.control([], Validators.required),
      }),
      step2: this.fb.group({
        file: [null],
      }),
    });
    this.greenhouseForm.valueChanges.subscribe(value => console.log(value));
  }

  get step0(): FormGroup {
    return this.greenhouseForm.get("step0") as FormGroup;
  }
  get step1(): FormGroup {
    return this.greenhouseForm.get("step1") as FormGroup;
  }
  get step2(): FormGroup {
    return this.greenhouseForm.get("step2") as FormGroup;
  }

  // Navigation
  protected next(): void {
    this.step.update(i => i + 1);
  }

  protected previous(): void {
    this.step.update(i => i - 1);
  }

  protected isCurrentStepValid(): boolean {
    switch (this.step()) {
      case 0:
        return (
          <boolean>this.greenhouseForm.get("name")?.valid &&
          <boolean>this.greenhouseForm.get("location")?.valid
        );
      // case 1:
      //   return this.selectedVegetables().length > 0;
      // case 2:
      //   return this.availableSensors().some(sensor => sensor.selected);
      default:
        return false;
    }
  }

  public openScanQrCode(
    content: PolymorpheusContent<TuiDialogContext>,
    header: PolymorpheusContent,
    size: TuiDialogSize
  ) {
    this.dialogs
      .open(content, {
        header,
        size,
      })
      .pipe(take(1))
      .subscribe(result => {
        this.addSensor(result, null);
      });
  }

  public addSensor(sourceAddress: string | void, name: string | null): void {
    if (!sourceAddress) return;

    const exists = this.sensors.some(sensor => sensor.sourceAddress === sourceAddress);
    if (exists) {
      console.error(`Sensor with sourceAddress "${sourceAddress}" already exists.`);
      return;
    }

    const id = this.sensors.length + 1;

    if (!name) {
      const sensorLabel = this.translate.instant("components.greenhouse-form.sensor");
      name = `${sensorLabel} ${id}`;
    }

    this.sensors.push({
      id,
      sourceAddress,
      name: name,
    });
  }

  manualAddSensor(): void {
    this.dialogs
      .open(new PolymorpheusComponent(SensorFormDialogComponent, this.injector), {
        data: {
          sourceAddress: null,
          name: null,
        },
        label: "Ajouter un capteur",
        size: "fullscreen",
        closeable: true,
      })
      .pipe(take(1))
      .subscribe(result => {
        const r = result as unknown as Sensor;
        this.addSensor(r.sourceAddress, r.name);
      });
  }

  removeCapteur(index: number): void {
    console.log(index);
    this.sensors.splice(index, 1);
  }

  onEdit(sensor: Sensor): void {
    this.dialogs
      .open(new PolymorpheusComponent(SensorFormDialogComponent, this.injector), {
        data: {
          sourceAddress: sensor.sourceAddress,
          name: sensor.name,
        },
        label: "Modifier un capteur",
        size: "fullscreen",
        closeable: true,
      })
      .pipe(take(1))
      .subscribe(result => {
        const r = result as unknown as Sensor;
        sensor.sourceAddress = r.sourceAddress;
        sensor.name = r.name;
      });
  }
}
