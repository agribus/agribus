import { Injectable } from "@angular/core";
import { Capacitor } from "@capacitor/core";

@Injectable({ providedIn: "root" })
export class PlatformService {
  public isNativePlatform(): boolean {
    return Capacitor.isNativePlatform();
  }

  public isBrowser(): boolean {
    return !Capacitor.isNativePlatform();
  }

  public isMobile(): boolean {
    return Capacitor.isNativePlatform() || this.isMobileWeb();
  }

  private isMobileWeb(): boolean {
    if (typeof window === "undefined") return false;

    const userAgent = navigator.userAgent;
    const mobileRegex = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i;

    // User agent check
    if (mobileRegex.test(userAgent)) return true;

    // Screen size + touch capability check
    const maxDimension = Math.max(window.screen.width, window.screen.height);
    const minDimension = Math.min(window.screen.width, window.screen.height);
    const hasTouch = "ontouchstart" in window || navigator.maxTouchPoints > 0;

    return minDimension <= 768 && hasTouch && maxDimension <= 1024;
  }

  public isTablet(): boolean {
    if (Capacitor.isNativePlatform()) {
      return window.screen.width >= 768;
    }

    if (typeof window === "undefined") return false;

    const userAgent = navigator.userAgent;
    const isTabletUA = /ipad|tablet/i.test(userAgent) && !/mobile/i.test(userAgent);

    if (isTabletUA) return true;

    // Detection by screen size
    const minDimension = Math.min(window.screen.width, window.screen.height);
    const maxDimension = Math.max(window.screen.width, window.screen.height);
    const hasTouch = "ontouchstart" in window;

    return minDimension >= 768 && maxDimension >= 1024 && hasTouch;
  }

  public isDesktop(): boolean {
    return this.isBrowser() && !this.isMobile() && !this.isTablet();
  }

  /**
   * [DEBUG] Get Device Info
   */
  public getDeviceInfo(): any {
    return {
      isNativePlatform: this.isNativePlatform(),
      isBrowser: this.isBrowser(),
      isMobile: this.isMobile(),
      isMobileWeb: this.isMobileWeb(),
      isTablet: this.isTablet(),
      isDesktop: this.isDesktop(),
      userAgent: navigator.userAgent,
      screenWidth: typeof window !== "undefined" ? window.screen.width : null,
      screenHeight: typeof window !== "undefined" ? window.screen.height : null,
      hasTouch: typeof window !== "undefined" ? "ontouchstart" in window : false,
      maxTouchPoints: navigator.maxTouchPoints || 0,
      capacitorPlatform: Capacitor.getPlatform(),
    };
  }
}
