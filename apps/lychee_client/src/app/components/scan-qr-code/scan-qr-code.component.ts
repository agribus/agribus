import { Component, OnInit } from "@angular/core";
import { ZXingScannerModule } from "@zxing/ngx-scanner";
import { BarcodeFormat } from "@zxing/library";
import { TuiDialogContext } from "@taiga-ui/core";
import { injectContext } from "@taiga-ui/polymorpheus";
import { Camera } from "@capacitor/camera";

@Component({
  selector: "app-scan-qr-code",
  imports: [ZXingScannerModule],
  templateUrl: "./scan-qr-code.component.html",
  styleUrl: "./scan-qr-code.component.scss",
})
export class ScanQrCodeComponent implements OnInit {
  allowedFormats = [BarcodeFormat.QR_CODE];

  public readonly context = injectContext<TuiDialogContext<string>>();

  async ngOnInit() {
    await Camera.requestPermissions();
  }

  handleQrCodeResult(result: string): void {
    this.context.completeWith(result);
  }
}
