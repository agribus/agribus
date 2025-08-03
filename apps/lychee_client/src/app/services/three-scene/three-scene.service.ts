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
  private smoothReset = false;

  private greenhouseModel: THREE.Object3D;
  private spawnPoints: THREE.Object3D[] = [];

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

    this.controls.addEventListener("start", this.onStart);
    this.controls.addEventListener("end", this.onEnd);

    this.loader.load(this.modelPath, gltf => {
      this.greenhouseModel = gltf.scene;

      const model = this.greenhouseModel;

      const box = new THREE.Box3().setFromObject(model);
      const center = box.getCenter(new THREE.Vector3());
      const size = box.getSize(new THREE.Vector3());

      model.position.sub(center);
      this.scene.add(model);

      model.traverse(child => {
        if (child.name.startsWith("spawn_")) {
          this.spawnPoints.push(child);
        }
      });

      this.spawnPoints.forEach(point => {
        const helper = new THREE.AxesHelper(0.2);
        point.add(helper);
      });

      if (this.spawnPoints.length > 0) {
        this.spawnPoints.forEach(point => {
          const carrotGeometry = new THREE.BoxGeometry(0.1, 0.1, 0.1);
          const carrotMaterial = new THREE.MeshStandardMaterial({ color: 0xff0000 });
          const carrotMesh = new THREE.Mesh(carrotGeometry, carrotMaterial);

          carrotMesh.position.copy(point.getWorldPosition(new THREE.Vector3()));

          carrotMesh.rotation.y = Math.random() * Math.PI * 2;
          this.scene.add(carrotMesh);
        });
      }

      const maxDim = Math.max(size.x, size.y, size.z);
      const fov = this.camera.fov * (Math.PI / 180);
      let cameraZ = maxDim / 2 / Math.tan(fov / 2);
      cameraZ *= 3;

      this.camera.position.set(0, 0, cameraZ);
      this.camera.lookAt(0, 0, 0);

      this.controls.target.set(0, 0, 0);
      this.controls.update();
    });

    this.animate();
    this.onResize();
    window.addEventListener("resize", this.onResize);
  }

  private onStart = () => {
    this.controls.minAzimuthAngle = -Infinity;
    this.controls.maxAzimuthAngle = Infinity;
    this.controls.minPolarAngle = 0;
    this.controls.maxPolarAngle = Math.PI;
    this.smoothReset = false;
  };

  private onEnd = () => {
    this.smoothReset = true;
  };

  private doSmoothReset = () => {
    const alpha = this.controls.getAzimuthalAngle();
    const beta = this.controls.getPolarAngle() - Math.PI / 2;

    const snappedAlpha = Math.abs(alpha) < 0.001 ? 0 : 0.95 * alpha;
    const snappedBeta = Math.abs(beta) < 0.001 ? 0 : 0.95 * beta;

    this.controls.minAzimuthAngle = snappedAlpha;
    this.controls.maxAzimuthAngle = snappedAlpha;

    this.controls.minPolarAngle = Math.PI / 2 + snappedBeta;
    this.controls.maxPolarAngle = this.controls.minPolarAngle;

    if (snappedAlpha === 0 && snappedBeta === 0) {
      this.onStart();
    }
  };

  private animate = () => {
    requestAnimationFrame(this.animate);
    if (this.smoothReset) this.doSmoothReset();
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
