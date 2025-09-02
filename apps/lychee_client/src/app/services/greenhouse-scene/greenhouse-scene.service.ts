import { ElementRef, Injectable } from "@angular/core";
import * as THREE from "three";
import { OrbitControls } from "three/addons/controls/OrbitControls.js";
import { GLTFLoader } from "three/addons/loaders/GLTFLoader.js";
import { Font, FontLoader } from "three/addons/loaders/FontLoader.js";
import { TextGeometry } from "three/addons/geometries/TextGeometry.js";
import { CROPS } from "@utils/vegetables-config/vegetables.config";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { Crop } from "@interfaces/crop.interface";

interface PlantedCrop {
  crop: Crop;
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
  private readonly FONT_PATH = "./fonts/helvetiker_regular.typeface.json";
  private readonly MAX_PLANTS = 6;
  private readonly DEFAULT_FOV = 60;
  private readonly INSIDE_FOV = 100;
  private readonly ANIMATION_DURATION = 1500;

  private scene!: THREE.Scene;
  private camera!: THREE.PerspectiveCamera;
  private renderer!: THREE.WebGLRenderer;
  private controls!: OrbitControls;
  private container!: HTMLElement;

  private readonly gltfLoader = new GLTFLoader();
  private readonly fontLoader = new FontLoader();
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
  private plantedCrops: PlantedCrop[] = [];
  private textContainers: THREE.Group[] = [];
  private font!: Font;

  isInsideView(): boolean {
    return this.cameraType === "inside";
  }

  async createScene(canvaContainer: ElementRef): Promise<void> {
    this.initializeScene(canvaContainer);
    this.setupRenderer();
    this.setupLighting();
    this.setupCamera();
    this.setupControls();
    this.setupEventListeners();
    await this.loadFont();
    await this.loadModel();
    this.animate();
  }

  async plantCropsFromGreenhouse(greenhouse: Greenhouse): Promise<void> {
    this.plantedCrops = [];
    greenhouse.crops.forEach(crop => {
      this.plantCrop(crop, crop.quantity ?? 1);
    });
  }

  plantCrop(crop: Crop, quantity: number): void {
    const config = CROPS[crop.commonName.toLowerCase()];
    if (!config) {
      console.error(`Crop "${crop.commonName}" not found in CROPS.`);
      return;
    }

    this.gltfLoader.load(config.modelPath, gltf => {
      const baseModel = gltf.scene;
      for (let i = 0; i < quantity; i++) {
        this.processPlantPlacement(crop, baseModel);
      }
    });
  }

  returnToDefaultCamera(): void {
    if (this.cameraType === "default" || !this.defaultCameraState.position.length()) {
      return;
    }

    this.cameraType = "default";
    this.controls.enableRotate = true;
    this.smoothReset = true;

    this.clearTextContainers();

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

  private async loadFont(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.fontLoader.load(
        this.FONT_PATH,
        font => {
          this.font = font;
          resolve();
        },
        undefined,
        reject
      );
    });
  }

  private async loadModel(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.gltfLoader.load(
        this.MODEL_PATH,
        gltf => {
          this.processModelLoad(gltf.scene);
          resolve();
        },
        undefined,
        reject
      );
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

  private processPlantPlacement(crop: Crop, baseModel: THREE.Object3D): void {
    if (this.plantedCrops.length < this.MAX_PLANTS) {
      this.addCropToScene(crop, baseModel);
    } else {
      const plantToRemove = this.findPlantToRemove(crop);
      if (plantToRemove) {
        this.removePlant(plantToRemove);
        this.addCropToScene(crop, baseModel);
      }
    }
  }

  private addCropToScene(crop: Crop, baseModel: THREE.Object3D): void {
    const availableSpawnIndex = this.findAvailableSpawnPoint();
    const point = this.spawnPoints[availableSpawnIndex];
    const vegClone = baseModel.clone(true);

    vegClone.position.copy(point.getWorldPosition(new THREE.Vector3()));
    vegClone.rotation.y = Math.random() * Math.PI * 2;
    vegClone.position.y += 0.1;
    vegClone.name = `greenhouse_pot_${availableSpawnIndex + 1}`;

    this.scene.add(vegClone);
    this.plantedCrops.push({
      crop: crop,
      mesh: vegClone,
      spawnPointIndex: availableSpawnIndex,
    });
  }

  private findAvailableSpawnPoint(): number {
    const usedSpawnPoints = this.plantedCrops.map(v => v.spawnPointIndex);
    for (let i = 0; i < this.spawnPoints.length; i++) {
      if (!usedSpawnPoints.includes(i)) {
        return i;
      }
    }
    return this.spawnPoints.length - 1;
  }

  private findPlantToRemove(newCrop: Crop): PlantedCrop | undefined {
    if (this.plantedCrops.some(plant => plant.crop.commonName === newCrop.commonName)) {
      return undefined;
    }

    const cropsCounts = this.getCropCounts();
    const typeToRemove = this.findMostFrequentType(cropsCounts, newCrop.commonName);

    if (!typeToRemove || cropsCounts.get(typeToRemove)! <= 1) {
      return undefined;
    }

    return this.plantedCrops.reverse().find(plant => plant.crop.commonName === typeToRemove);
  }

  private getCropCounts(): Map<string, number> {
    const counts = new Map<string, number>();
    this.plantedCrops.forEach(plant => {
      counts.set(plant.crop.commonName, (counts.get(plant.crop.commonName) || 0) + 1);
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

  private removePlant(plant: PlantedCrop): void {
    this.scene.remove(plant.mesh);
    const index = this.plantedCrops.findIndex(p => p === plant);
    if (index >= 0) {
      this.plantedCrops.splice(index, 1);
    }
  }

  private create3DTextContainer(text: string, position: THREE.Vector3): THREE.Group {
    const container = new THREE.Group();

    const fontSize = 0.05;
    const margin = 0.08;
    const containerDepth = 0.04;

    const textGeometry = new TextGeometry(text, {
      font: this.font,
      size: fontSize,
      depth: 0,
    });

    textGeometry.computeBoundingBox();
    const textWidth = textGeometry.boundingBox!.max.x - textGeometry.boundingBox!.min.x;
    const textHeight = textGeometry.boundingBox!.max.y - textGeometry.boundingBox!.min.y;

    textGeometry.translate(-textWidth / 2, textHeight / 2 - fontSize, containerDepth / 2 + 0.011);

    const textMaterial = new THREE.MeshBasicMaterial({ color: 0x374141 });
    const textMesh = new THREE.Mesh(textGeometry, textMaterial);

    const containerWidth = textWidth + margin * 2;
    const containerHeight = textHeight + margin * 2;

    const containerGeometry = this.createContainerGeometry(
      containerWidth,
      containerHeight,
      containerDepth - 0.01
    );
    const containerMaterial = new THREE.MeshStandardMaterial({
      color: 0xffffff,
      emissive: 0xfafafa,
      emissiveIntensity: 0.5,
    });
    const containerMesh = new THREE.Mesh(containerGeometry, containerMaterial);

    container.add(containerMesh);
    container.add(textMesh);

    const direction = new THREE.Vector3();
    direction.subVectors(this.camera.position, position).normalize();
    const yAxisRotation = Math.atan2(direction.x, direction.z);

    container.position.copy(position);
    container.rotation.set(0, yAxisRotation, 0);

    return container;
  }

  private createContainerGeometry(
    containerWidth: number,
    containerHeight: number,
    containerDepth: number
  ): THREE.BufferGeometry {
    const borderRadius = containerDepth;

    const shape = new THREE.Shape();
    const x = -containerWidth / 2;
    const y = -containerHeight / 2;

    shape.moveTo(x, y + borderRadius);
    shape.lineTo(x, y + containerHeight - borderRadius);
    shape.quadraticCurveTo(x, y + containerHeight, x + borderRadius, y + containerHeight);
    shape.lineTo(x + containerWidth - borderRadius, y + containerHeight);
    shape.quadraticCurveTo(
      x + containerWidth,
      y + containerHeight,
      x + containerWidth,
      y + containerHeight - borderRadius
    );
    shape.lineTo(x + containerWidth, y + borderRadius);
    shape.quadraticCurveTo(x + containerWidth, y, x + containerWidth - borderRadius, y);
    shape.lineTo(x + borderRadius, y);
    shape.quadraticCurveTo(x, y, x, y + borderRadius);

    return new THREE.ExtrudeGeometry(shape, {
      depth: containerDepth,
      bevelEnabled: false,
    });
  }

  showCropIdealConditions(index: number): void {
    const plantedCrop = this.plantedCrops.find(plant => plant.spawnPointIndex === index);
    if (!plantedCrop) {
      console.error("No planted crop found at index:", index);
      return;
    }

    const infoText = `${plantedCrop.crop.commonName}\n${plantedCrop.crop.cropGrowthConditions?.atmosphericHumidity} %RH\n${plantedCrop.crop.cropGrowthConditions?.miniumTemperature}° - ${plantedCrop.crop.cropGrowthConditions?.maximumTemperature}°C\n`;

    const textPosition = new THREE.Vector3(
      plantedCrop.mesh.position.x,
      plantedCrop.mesh.position.y + 0.5,
      plantedCrop.mesh.position.z
    );

    const textContainer = this.create3DTextContainer(infoText, textPosition);

    this.adjustTextContainerPosition(textContainer, textPosition);

    this.textContainers.push(textContainer);
    this.scene.add(textContainer);
  }

  private clearTextContainers(): void {
    this.textContainers.forEach(container => {
      this.scene.remove(container);
      container.traverse(child => {
        if (child instanceof THREE.Mesh) {
          child.geometry?.dispose();
          if (Array.isArray(child.material)) {
            child.material.forEach(material => material.dispose());
          } else {
            child.material?.dispose();
          }
        }
      });
    });
    this.textContainers = [];
  }

  private isObjectVisibleOnScreen(object: THREE.Object3D): boolean {
    const box = new THREE.Box3().setFromObject(object);
    const corners = [
      new THREE.Vector3(box.min.x, box.min.y, box.min.z),
      new THREE.Vector3(box.min.x, box.min.y, box.max.z),
      new THREE.Vector3(box.min.x, box.max.y, box.min.z),
      new THREE.Vector3(box.min.x, box.max.y, box.max.z),
      new THREE.Vector3(box.max.x, box.min.y, box.min.z),
      new THREE.Vector3(box.max.x, box.min.y, box.max.z),
      new THREE.Vector3(box.max.x, box.max.y, box.min.z),
      new THREE.Vector3(box.max.x, box.max.y, box.max.z),
    ];

    const screenCorners = corners.map(corner => {
      const screenPos = corner.clone().project(this.camera);
      return {
        x: (screenPos.x + 1) / 2,
        y: (screenPos.y + 1) / 2,
      };
    });

    const minX = Math.min(...screenCorners.map(c => c.x)) + 0.1;
    const maxX = Math.max(...screenCorners.map(c => c.x)) - 0.1;
    const minY = Math.min(...screenCorners.map(c => c.y)) + 0.1;
    const maxY = Math.max(...screenCorners.map(c => c.y)) - 0.1;

    return minX >= 0 && maxX <= 1 && minY >= 0 && maxY <= 1;
  }

  private adjustTextContainerPosition(
    container: THREE.Group,
    originalPosition: THREE.Vector3
  ): void {
    const maxIterations = 50;
    const zStep = 0.01;
    let iterations = 0;

    container.position.copy(originalPosition);

    while (!this.isObjectVisibleOnScreen(container) && iterations < maxIterations) {
      if (container.position.z > 0) {
        container.position.z = Math.max(0, container.position.z - zStep);
      } else if (container.position.z < 0) {
        container.position.z = Math.min(0, container.position.z + zStep);
      } else {
        break;
      }

      const direction = new THREE.Vector3();
      direction.subVectors(this.camera.position, container.position).normalize();
      const yAxisRotation = Math.atan2(direction.x, direction.z);
      container.rotation.set(0, yAxisRotation, 0);

      iterations++;
    }

    if (!this.isObjectVisibleOnScreen(container) && iterations >= maxIterations) {
      const originalY = container.position.y;

      container.position.y += 0.3;
      if (!this.isObjectVisibleOnScreen(container)) {
        container.position.y = originalY - 0.3;
        if (!this.isObjectVisibleOnScreen(container)) {
          container.position.y = originalY;
        }
      }
    }
  }

  private animateCamera(targetState: CameraState, onComplete?: () => void): void {
    const startState: CameraState = {
      position: this.camera.position.clone(),
      target: this.controls.target.clone(),
      fov: this.camera.fov,
    };

    this.smoothReset = false;

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

    this.cameraType = "inside";
    this.controls.enableRotate = false;

    const spherical = new THREE.Spherical();
    spherical.setFromVector3(
      this.defaultCameraState.position.clone().sub(this.defaultCameraState.target)
    );

    const baseAzimuthAngle = spherical.theta;

    const targetAzimuthAngle = baseAzimuthAngle + Math.PI / 2;
    const targetPolarAngle = -Math.PI / 2 + 0.3;
    const targetDistance = 1.8;

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

    this.camera.aspect = window.innerWidth / window.innerHeight;
    this.renderer.setSize(window.innerWidth, window.innerHeight);
  };

  private onPointerClick(event: MouseEvent): void {
    const rect = this.renderer.domElement.getBoundingClientRect();

    this.pointer.x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
    this.pointer.y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

    const intersectedObject = this.getIntersectedObject();

    if (intersectedObject) {
      this.clearTextContainers();
      if (
        intersectedObject.parent?.name.startsWith("greenhouse_pot_") ||
        intersectedObject.parent?.parent?.name.startsWith("greenhouse_pot_")
      ) {
        const str = intersectedObject.parent?.name.startsWith("greenhouse_pot_")
          ? intersectedObject.parent?.name
          : intersectedObject.parent?.parent?.name;
        const index = parseInt(str!.charAt(str!.length - 1)) - 1;
        this.showCropIdealConditions(index);
      } else if (intersectedObject.name.startsWith("greenhouse_")) {
        this.transitionToInsideView();
      }
    }
  }

  private getIntersectedObject(): THREE.Object3D | null {
    this.raycaster.setFromCamera(this.pointer, this.camera);
    const intersects = this.raycaster.intersectObjects(this.scene.children, true);
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
