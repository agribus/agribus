import { Component, EventEmitter, inject, Output } from "@angular/core";
import { BarcodeScanner } from "@capacitor-mlkit/barcode-scanning";
import { PlatformService } from "@services/platform/platform.service";
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { BarcodeFormat } from "@zxing/library";

@Component({
  selector: "app-scan-qr-code",
  imports: [ZXingScannerModule],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent {
  private readonly platform = inject(PlatformService);

  public readonly isNativePlatform = this.platform.isNativePlatform();
  public readonly allowedFormats = [BarcodeFormat.QR_CODE];
  public scannerEnabled = false;

  @Output() codeScanned = new EventEmitter<string>();

  public activateScanner() {
    this.scannerEnabled = true;

    if (this.isNativePlatform) {
      BarcodeScanner.scan()
        .then(({ barcodes }) => {
          if (barcodes.length > 0 && barcodes[0].rawValue) {
            this.handleDetectedCode(barcodes[0].rawValue);
          }
        })
        .catch(err => console.error(err));
    }
  }

  public handleScanResult(result: string) {
    this.handleDetectedCode(result);
    this.scannerEnabled = false;
  }

  public handleDetectedCode(result: string) {
    this.codeScanned.emit(result);
  }

  public deactivateScanner() {
    this.scannerEnabled = false;
  }
}
