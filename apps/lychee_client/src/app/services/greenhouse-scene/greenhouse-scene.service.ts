import { ElementRef, Injectable } from "@angular/core";
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GLTFLoader } from "three/addons/loaders/GLTFLoader.js";
import { VEGETABLES } from "@utils/vegetables-config/vegetables.config";
import { Greenhouse } from "@interfaces/greenhouse.interface";

interface PlantedVegetable {
  key: string;
  mesh: THREE.Object3D;
  spawnPointIndex: number;
}

interface CameraState {
  position: THREE.Vector3;
  target: THREE.Vector3;
  fov: number;
}

type CameraType = "default" | "inside";

@Injectable({ providedIn: "root" })
export class GreenhouseSceneService {
  private readonly MODEL_PATH = "./models/greenhouse.glb";
  private readonly MAX_PLANTS = 6;
  private readonly DEFAULT_FOV = 60;
  private readonly INSIDE_FOV = 95;
  private readonly ANIMATION_DURATION = 1500;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private controls!: OrbitControls;
  private container!: HTMLElement;

  private readonly loader = new GLTFLoader();
  private readonly raycaster = new THREE.Raycaster();
  private readonly pointer = new THREE.Vector2();

  private cameraType!: CameraType;
  private smoothReset = false;
  private defaultCameraState: CameraState = {
    position: new THREE.Vector3(),
    target: new THREE.Vector3(0, 0, 0),
    fov: this.DEFAULT_FOV,
  };

  private spawnPoints: THREE.Object3D[] = [];
  private plantedVegetables: PlantedVegetable[] = [];

  isInsideView(): boolean {
    return this.cameraType === "inside";
  }

  async createScene(canvaContainer: ElementRef): Promise<void> {
    this.initializeScene(canvaContainer);
    this.setupLighting();
    this.setupCamera();
    this.setupRenderer();
    this.setupControls();
    this.setupEventListeners();
    this.loadModel();
    this.animate();
  }

  async plantVegetablesFromGreenhouse(greenhouse: Greenhouse): Promise<void> {
    greenhouse.crops.forEach(crop => {
      this.plantVegetable(crop.commonName, crop.quantity ?? 1);
    });
  }

  plantVegetable(vegetableKey: string, quantity: number): void {
    const config = VEGETABLES[vegetableKey.toLowerCase()];
    if (!config) {
      console.error(`Vegetable "${vegetableKey}" not found in VEGETABLES.`);
      return;
    }

    this.loader.load(config.modelPath, gltf => {
      const baseModel = gltf.scene;
      for (let i = 0; i < quantity; i++) {
        this.processPlantPlacement(vegetableKey, baseModel);
      }
    });
  }

  returnToDefaultCamera(): void {
    if (this.cameraType === "default" || !this.defaultCameraState.position.length()) {
      return;
    }

    // Exécuter les actions au début de l'animation
    this.cameraType = "default";
    this.controls.enableRotate = true;
    this.smoothReset = true;

    this.animateCamera(this.defaultCameraState);
  }

  private initializeScene(container: ElementRef): void {
    this.container = container.nativeElement;
    this.scene = new THREE.Scene();
  }

  private setupCamera(): void {
    this.cameraType = "default";
    this.camera = new THREE.PerspectiveCamera(
      this.DEFAULT_FOV,
      window.innerWidth / window.innerHeight
    );
    this.camera.position.set(0, 0, 0);
    this.camera.lookAt(this.scene.position);
  }

  private setupRenderer(): void {
    this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
    this.renderer.setSize(window.innerWidth, window.innerHeight);
    this.container.appendChild(this.renderer.domElement);
  }

  private setupLighting(): void {
    const ambientLight = new THREE.AmbientLight("white", 0.9);
    const directionalLight = new THREE.DirectionalLight("white", 0.9);
    directionalLight.position.set(1, 1, 1);

    this.scene.add(ambientLight, directionalLight);
  }

  private setupControls(): void {
    this.controls = new OrbitControls(this.camera, this.renderer.domElement);
    this.controls.enableDamping = true;
    this.controls.enableZoom = false;
    this.controls.enablePan = false;
    this.controls.update();

    this.controls.addEventListener("start", this.onControlsStart);
    this.controls.addEventListener("end", this.onControlsEnd);
  }

  private setupEventListeners(): void {
    window.addEventListener("resize", this.onResize);
    this.renderer.domElement.addEventListener("click", this.onPointerClick.bind(this));
  }

  private loadModel(): void {
    this.loader.load(this.MODEL_PATH, gltf => {
      this.processModelLoad(gltf.scene);
    });
  }

  private processModelLoad(model: THREE.Object3D): void {
    this.positionModel(model);
    this.scene.add(model);
    this.extractSpawnPoints(model);
    this.setupDefaultCamera(model);
  }

  private positionModel(model: THREE.Object3D): void {
    const box = new THREE.Box3().setFromObject(model);
    const center = box.getCenter(new THREE.Vector3());
    model.position.sub(center);
  }

  private extractSpawnPoints(model: THREE.Object3D): void {
    model.traverse(child => {
      if (child.name.startsWith("spawn_")) {
        this.spawnPoints.push(child);
      }
      child.name = "greenhouse_" + child.name;
    });
  }

  private setupDefaultCamera(model: THREE.Object3D): void {
    const box = new THREE.Box3().setFromObject(model);
    const size = box.getSize(new THREE.Vector3());
    const maxDim = Math.max(size.x, size.y, size.z);

    const fov = this.camera.fov * (Math.PI / 180);
    const cameraZ = (maxDim / 2 / Math.tan(fov / 2)) * 3;

    this.defaultCameraState = {
      position: new THREE.Vector3(0, 0, cameraZ),
      target: new THREE.Vector3(0, 0, 0),
      fov: this.DEFAULT_FOV,
    };

    this.applyCameraState(this.defaultCameraState);
  }

  private processPlantPlacement(vegetableKey: string, baseModel: THREE.Object3D): void {
    if (this.plantedVegetables.length < this.MAX_PLANTS) {
      this.addVegetableToScene(vegetableKey, baseModel);
    } else {
      const plantToRemove = this.findPlantToRemove(vegetableKey);
      if (plantToRemove) {
        this.removePlant(plantToRemove);
        this.addVegetableToScene(vegetableKey, baseModel);
      }
    }
  }

  private addVegetableToScene(vegetableKey: string, baseModel: THREE.Object3D): void {
    const availableSpawnIndex = this.findAvailableSpawnPoint();
    const point = this.spawnPoints[availableSpawnIndex];
    const vegClone = baseModel.clone(true);

    vegClone.position.copy(point.getWorldPosition(new THREE.Vector3()));
    vegClone.rotation.y = Math.random() * Math.PI * 2;
    vegClone.position.y += 0.1;

    this.scene.add(vegClone);
    this.plantedVegetables.push({
      key: vegetableKey,
      mesh: vegClone,
      spawnPointIndex: availableSpawnIndex,
    });
  }

  private findAvailableSpawnPoint(): number {
    const usedSpawnPoints = this.plantedVegetables.map(v => v.spawnPointIndex);
    for (let i = 0; i < this.spawnPoints.length; i++) {
      if (!usedSpawnPoints.includes(i)) {
        return i;
      }
    }
    return this.spawnPoints.length - 1;
  }

  private findPlantToRemove(newVegetableKey: string): PlantedVegetable | undefined {
    if (this.plantedVegetables.some(plant => plant.key === newVegetableKey)) {
      return undefined;
    }

    const vegetablesCounts = this.getVegetableCounts();
    const typeToRemove = this.findMostFrequentType(vegetablesCounts, newVegetableKey);

    if (!typeToRemove || vegetablesCounts.get(typeToRemove)! <= 1) {
      return undefined;
    }

    return this.plantedVegetables.reverse().find(plant => plant.key === typeToRemove);
  }

  private getVegetableCounts(): Map<string, number> {
    const counts = new Map<string, number>();
    this.plantedVegetables.forEach(plant => {
      counts.set(plant.key, (counts.get(plant.key) || 0) + 1);
    });
    return counts;
  }

  private findMostFrequentType(
    counts: Map<string, number>,
    excludeKey: string
  ): string | undefined {
    let maxCount = 0;
    let typeToRemove: string | undefined;

    counts.forEach((count, key) => {
      if (key !== excludeKey && count >= maxCount) {
        maxCount = count;
        typeToRemove = key;
      }
    });

    return typeToRemove;
  }

  private removePlant(plant: PlantedVegetable): void {
    this.scene.remove(plant.mesh);
    const index = this.plantedVegetables.findIndex(p => p === plant);
    if (index >= 0) {
      this.plantedVegetables.splice(index, 1);
    }
  }

  private animateCamera(targetState: CameraState, onComplete?: () => void): void {
    const startState: CameraState = {
      position: this.camera.position.clone(),
      target: this.controls.target.clone(),
      fov: this.camera.fov,
    };

    this.smoothReset = false;
    this.controls.enableRotate = false;

    this.performCameraAnimation(startState, targetState, onComplete);
  }

  private performCameraAnimation(
    startState: CameraState,
    targetState: CameraState,
    onComplete?: () => void
  ): void {
    const startTime = performance.now();

    const animate = () => {
      const progress = Math.min((performance.now() - startTime) / this.ANIMATION_DURATION, 1);
      const easedProgress = this.easeOutCubic(progress);

      this.interpolateCameraState(startState, targetState, easedProgress);

      if (progress < 1) {
        requestAnimationFrame(animate);
      } else {
        onComplete?.();
      }
    };

    animate();
  }

  private interpolateCameraState(start: CameraState, target: CameraState, progress: number): void {
    this.camera.position.lerpVectors(start.position, target.position, progress);
    this.camera.fov = start.fov + (target.fov - start.fov) * progress;
    this.camera.updateProjectionMatrix();

    this.controls.target.lerpVectors(start.target, target.target, progress);
    this.controls.update();
  }

  private applyCameraState(state: CameraState): void {
    this.camera.position.copy(state.position);
    this.camera.fov = state.fov;
    this.camera.updateProjectionMatrix();
    this.controls.target.copy(state.target);
    this.controls.update();
  }

  private easeOutCubic(t: number): number {
    return 1 - Math.pow(1 - t, 3);
  }

  private transitionToInsideView(): void {
    if (this.cameraType === "inside") return;

    // Exécuter l'action au début de l'animation
    this.cameraType = "inside";

    // Utiliser la position par défaut de la caméra comme référence
    const spherical = new THREE.Spherical();
    spherical.setFromVector3(
      this.defaultCameraState.position.clone().sub(this.defaultCameraState.target)
    );

    // Calculer l'angle de base à partir de la position par défaut
    const baseAzimuthAngle = spherical.theta;

    // Appliquer la rotation de 90° par rapport à la position de base
    const targetAzimuthAngle = baseAzimuthAngle + Math.PI / 2;
    const targetPolarAngle = -Math.PI / 2 + 0.3;
    const targetDistance = 1.8;

    // Recalculer la position cible avec les nouveaux angles
    spherical.theta = targetAzimuthAngle;
    spherical.phi = targetPolarAngle;
    spherical.radius = targetDistance;

    const targetPosition = new THREE.Vector3();
    targetPosition.setFromSpherical(spherical).add(this.defaultCameraState.target);

    const targetState: CameraState = {
      position: targetPosition,
      target: new THREE.Vector3(0, -1.2, 0),
      fov: this.INSIDE_FOV,
    };

    this.animateCamera(targetState);
  }

  private onControlsStart = (): void => {
    this.controls.minAzimuthAngle = -Infinity;
    this.controls.maxAzimuthAngle = Infinity;
    this.controls.minPolarAngle = 0;
    this.controls.maxPolarAngle = Math.PI / 2 + 0.1;
    this.smoothReset = false;
  };

  private onControlsEnd = (): void => {
    this.smoothReset = true;
  };

  private doSmoothReset(): void {
    const alpha = this.controls.getAzimuthalAngle();
    const beta = this.controls.getPolarAngle() - Math.PI / 2;

    const snappedAlpha = Math.abs(alpha) < 0.001 ? 0 : 0.95 * alpha;
    const snappedBeta = Math.abs(beta) < 0.001 ? 0 : 0.95 * beta;

    this.controls.minAzimuthAngle = snappedAlpha;
    this.controls.maxAzimuthAngle = snappedAlpha;
    this.controls.minPolarAngle = Math.PI / 2 + snappedBeta;
    this.controls.maxPolarAngle = this.controls.minPolarAngle;

    if (snappedAlpha === 0 && snappedBeta === 0) {
      this.onControlsStart();
    }
  }

  private onResize = (): void => {
    if (!this.container) return;

    this.camera.updateProjectionMatrix();
    this.renderer.setSize(window.innerWidth, window.innerHeight);
  };

  private onPointerClick(event: MouseEvent): void {
    this.pointer.x = (event.clientX / window.innerWidth) * 2 - 1;
    this.pointer.y = -(event.clientY / window.innerHeight) * 2 + 1;
    const intersectedObject = this.getIntersectedObject();

    if (intersectedObject) {
      if (intersectedObject.name.startsWith("greenhouse_")) {
        this.transitionToInsideView();
      } else if (intersectedObject.name.startsWith("spawn_")) {
        // Future: this.showCropIdealConditions(index);
      }
    }
  }

  private getIntersectedObject(): THREE.Object3D | null {
    this.raycaster.setFromCamera(this.pointer, this.camera);
    const intersects = this.raycaster.intersectObjects(this.scene.children);
    return intersects.length > 0 ? intersects[0].object : null;
  }

  private animate = (): void => {
    requestAnimationFrame(this.animate);

    if (this.smoothReset && this.cameraType === "default") {
      this.doSmoothReset();
    }

    this.controls.update();
    this.renderer.render(this.scene, this.camera);
  };
}
