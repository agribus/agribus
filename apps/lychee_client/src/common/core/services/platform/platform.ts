import { Injectable } from '@angular/core';
import { Capacitor } from '@capacitor/core';

@Injectable({ providedIn: 'root' })
export class Platform {
  isMobile(): boolean {
    return Capacitor.isNativePlatform(); // iOS/Android
  }

  isBrowser(): boolean {
    return !Capacitor.isNativePlatform(); // Web
  }
}
