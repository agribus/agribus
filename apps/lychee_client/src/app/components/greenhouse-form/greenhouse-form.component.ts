import { Component, inject, signal } from "@angular/core";
import { TuiStepper } from "@taiga-ui/kit";
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TuiButton, TuiTextfield } from "@taiga-ui/core";

@Component({
  selector: "app-greenhouse-form",
  imports: [TuiStepper, ReactiveFormsModule, TuiTextfield, TuiButton],
  templateUrl: "./greenhouse-form.component.html",
  styleUrl: "./greenhouse-form.component.scss",
})
export class GreenhouseFormComponent {
  protected readonly step = signal(0);

  protected readonly greenhouseForm: FormGroup;
  private fb = inject(FormBuilder);

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
}
