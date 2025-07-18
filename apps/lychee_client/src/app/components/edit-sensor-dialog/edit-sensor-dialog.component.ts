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
  selector: "app-edit-sensor-dialog",
  imports: [ReactiveFormsModule, TuiButton, FormsModule, TuiTextfield, TranslatePipe],
  templateUrl: "./edit-sensor-dialog.component.html",
  styleUrl: "./edit-sensor-dialog.component.scss",
})
export class EditSensorDialogComponent implements OnInit {
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
