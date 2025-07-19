import { AfterViewChecked, AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { ZXingScannerComponent, ZXingScannerModule } from "@zxing/ngx-scanner";
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
export class ScanQrCodeComponent implements AfterViewInit {
  allowedFormats = [BarcodeFormat.QR_CODE];

  public hasPermission = false;
  @ViewChild(ZXingScannerComponent) scanner!: ZXingScannerComponent;
  public readonly context = injectContext<TuiDialogContext<string>>();

  async ngAfterViewInit() {
    const permission = await Camera.requestPermissions();
    this.hasPermission = permission.camera === "granted";

    if (this.hasPermission) {
      this.scanner.camerasFound.subscribe(async (devices: MediaDeviceInfo[]) => {
        alert("camera found");
      });
    } else {
      alert("Permission caméra refusée. Impossible de scanner un QR code.");
    }
  }

  handleQrCodeResult(result: string): void {
    this.context.completeWith(result);
  }
}
