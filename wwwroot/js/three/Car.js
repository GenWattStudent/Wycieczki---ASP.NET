export default class Car {
  constructor(scene, assets) {
    this.scene = scene
    this.car = null
    this.assets = assets
    this.scene.add(this.assets.carMesh.scene)
    this.car = this.assets.carMesh.scene
    this.car.scale.set(0.25, 0.25, 0.25)
  }
}
