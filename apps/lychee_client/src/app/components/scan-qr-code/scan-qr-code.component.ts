import { Component, Inject } from "@angular/core";
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { BarcodeFormat } from "@zxing/library";
import { TuiDialogContext } from "@taiga-ui/core";
import { injectContext } from "@taiga-ui/polymorpheus";

@Component({
  selector: "app-scan-qr-code",
  imports: [ZXingScannerModule],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent {
  allowedFormats = [BarcodeFormat.QR_CODE];

  public readonly context = injectContext<TuiDialogContext<number>>();

  handleQrCodeResult(result: string): void {
    console.log("QR Code Result:", result);
    // Supposons que le QR code contient un chiffre
    const number = parseInt(result, 10);
    if (!isNaN(number)) {
      console.log("Nombre détecté :", number);
    } else {
      console.warn("QR code ne contient pas un nombre valide");
    }

    this.context.completeWith(number);
  }
}
