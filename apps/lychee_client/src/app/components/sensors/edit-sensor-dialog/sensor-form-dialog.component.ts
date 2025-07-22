import { Component, inject, OnInit } from "@angular/core";
import { injectContext } from "@taiga-ui/polymorpheus";
import { TuiButton, TuiDialogContext, TuiTextfield } from "@taiga-ui/core";
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { Sensor } from "@interfaces/sensor.interface";
import { TranslatePipe } from "@ngx-translate/core";

@Component({
  selector: "app-sensor-form-dialog",
  imports: [ReactiveFormsModule, TuiButton, FormsModule, TuiTextfield, TranslatePipe],
  templateUrl: "./sensor-form-dialog.component.html",
  styleUrl: "./sensor-form-dialog.component.scss",
})
export class SensorFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  public readonly context = injectContext<TuiDialogContext<Sensor, null>>();

  public form!: FormGroup;

  ngOnInit(): void {
    const { sourceAddress, name } = this.context.data;
    this.form = this.fb.group({
      sourceAddress: [sourceAddress, Validators.required],
      name: [name, Validators.required],
    });
  }

  save(): void {
    if (this.form.valid) {
      this.context.completeWith(this.form.value);
    }
  }
}
