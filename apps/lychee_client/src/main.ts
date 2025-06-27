import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './common/bootstrap/app.config';
import { BootstrapApp } from './common/bootstrap/app';

bootstrapApplication(BootstrapApp, appConfig).catch((err) => console.error(err));
