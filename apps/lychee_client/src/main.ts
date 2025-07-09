import { bootstrapApplication } from "@angular/platform-browser";
import { appConfig } from "./app/app.config";
import { BootstrapApp } from "./app/app.component";

bootstrapApplication(BootstrapApp, appConfig).catch(err => console.error(err));
