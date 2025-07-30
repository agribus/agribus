import { ElementRef, Injectable } from "@angular/core";
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GLTFLoader } from "three/addons/loaders/GLTFLoader.js";

@Injectable({ providedIn: "root" })
export class ThreeSceneService {
  private scene: THREE.Scene;
  private camera: THREE.PerspectiveCamera;
  private renderer: THREE.WebGLRenderer;
  private controls: OrbitControls;
  private loader = new GLTFLoader();
  private container: HTMLElement;

  private modelPath = "./models/greenhouse.glb";

  createScene(container: ElementRef): void {
    this.container = container.nativeElement;

    this.scene = new THREE.Scene();

    this.camera = new THREE.PerspectiveCamera(60, innerWidth / innerHeight);
    this.camera.position.set(0, 0, 0);
    this.camera.lookAt(this.scene.position);

    this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
    this.renderer.setSize(innerWidth, innerHeight);
    this.container.appendChild(this.renderer.domElement);

    const ambientLight = new THREE.AmbientLight("white", 0.9);
    this.scene.add(ambientLight);

    const light = new THREE.DirectionalLight("white", 0.9);
    light.position.set(1, 1, 1);
    this.scene.add(light);

    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.update();

    this.loader.load(
      this.modelPath,
      gltf => {
        const model = gltf.scene;
        this.scene.add(model);

        const box = new THREE.Box3().setFromObject(model);
        const size = box.getSize(new THREE.Vector3());
        const center = box.getCenter(new THREE.Vector3());

        model.position.sub(center);

        const maxDim = Math.max(size.x, size.y, size.z);
        const fov = this.camera.fov * (Math.PI / 180);
        let cameraZ = maxDim / 2 / Math.tan(fov / 2);
        cameraZ *= 3;

        this.camera.position.set(0, 0, cameraZ);
        this.camera.lookAt(0, 0, 0);

        this.controls.target.set(0, 0, 0);
        this.controls.update();
      },
      undefined,
      error => {
        console.error("Error while loading GLTF :", error);
      }
    );

    this.animate();
    this.onResize();
    window.addEventListener("resize", this.onResize);
  }

  private animate = () => {
    requestAnimationFrame(this.animate);
    this.controls.update();
    this.renderer.render(this.scene, this.camera);
  };

  private onResize = () => {
    if (!this.container) return;
    this.camera.aspect = innerWidth / innerHeight;
    this.camera.updateProjectionMatrix();
    this.renderer.setSize(innerWidth, innerHeight);
  };
}
