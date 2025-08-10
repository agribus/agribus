import type { CapacitorConfig } from "@capacitor/cli";

const config: CapacitorConfig = {
  appId: "com.agribus.app",
  appName: "Agribus",
  webDir: "dist/lychee_client/browser",
  plugins: {
    StatusBar: {
      overlaysWebView: false,
      style: "LIGHT",
      backgroundColor: "#ffffff",
    },
    SplashScreen: {
      showSpinner: false,
      backgroundColor: "#ffffff",
      launchAutoHide: false,
    },
  },
};

export default config;
