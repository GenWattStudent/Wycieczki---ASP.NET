import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js'

export default class Car {
  constructor(scene) {
    this.scene = scene
    this.car = null
  }

  loadAssets = () => {
    return new Promise((resolve, reject) => {
      const loader = new GLTFLoader()
      loader.load('/images/three/m.glb', (gltf) => {
        this.scene.add(gltf.scene)
        this.car = gltf.scene
        this.car.scale.set(0.25, 0.25, 0.25)
        resolve(this.car)
      })
    })
  }
}
