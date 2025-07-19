import { Component, inject, OnInit } from "@angular/core";
import { TuiDialogContext } from "@taiga-ui/core";
import { injectContext } from "@taiga-ui/polymorpheus";
import { BarcodeScanner } from "@capacitor-mlkit/barcode-scanning";
import { PlatformService } from "@services/platform/platform.service";

@Component({
  selector: "app-scan-qr-code",
  imports: [],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent implements OnInit {
  public readonly context = injectContext<TuiDialogContext<string>>();

  private platformService = inject(PlatformService);

  ngOnInit() {
    if (this.platformService.isMobile()) {
      this.startScan();
    }
  }

  isScanning = false;
  error: string | null = null;

  async startScan() {
    this.isScanning = true;
    this.error = "coucou";

    try {
      const { barcodes } = await BarcodeScanner.scan();

      if (barcodes.length > 0 && barcodes[0].rawValue) {
        this.context.completeWith(barcodes[0].rawValue);
      } else {
        this.error = "Aucun QR code détecté.";
      }
    } catch (err) {
      this.error = "Erreur lors du scan.";
      console.error(err);
    } finally {
      this.isScanning = false;
    }
  }
}
