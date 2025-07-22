import { Component, inject, OnInit } from "@angular/core";
import { BarcodeScanner } from "@capacitor-mlkit/barcode-scanning";
import { PlatformService } from "@services/platform/platform.service";

@Component({
  selector: "app-scan-qr-code",
  imports: [],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent implements OnInit {
  private readonly platform = inject(PlatformService);

  async ngOnInit() {
    if (this.platform.isMobile()) {
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
}
