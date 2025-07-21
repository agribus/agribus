import { Component, inject, OnInit } from "@angular/core";
import { BarcodeScanner } from "@capacitor-mlkit/barcode-scanning";
import { BarcodeFormat } from "@zxing/library";
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { PlatformService } from "@services/platform/platform.service";

@Component({
  selector: "app-scan-qr-code",
  imports: [ZXingScannerModule],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent implements OnInit {
  private readonly platform = inject(PlatformService);

  public isMobile = false;
  allowedFormats = [BarcodeFormat.QR_CODE];
  scanResult: string | null = null;
  scanning = false;

  async ngOnInit() {
    this.isMobile = this.platform.isMobile();

    if (this.isMobile) {
      try {
        const { barcodes } = await BarcodeScanner.scan();

        if (barcodes.length > 0 && barcodes[0].rawValue) {
          console.log(barcodes[0].rawValue);
        }
      } catch (err) {
        console.error(err);
      }
    }
  }

  public handleQrCodeResult(result: string): void {
    this.scanResult = result;
    console.log("QR Code Result:", result);
  }
}
