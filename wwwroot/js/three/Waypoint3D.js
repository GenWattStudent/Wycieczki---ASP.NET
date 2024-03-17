import { TextGeometry } from 'three/addons/geometries/TextGeometry.js'
import { latLongToVector3 } from './helpers.js'
import * as THREE from 'three'

export default class Waypoint3D {
  constructor(lat, lon, title, camera, radius, zoom, assets) {
    this.lat = lat
    this.lon = lon
    this.title = title
    this.position = latLongToVector3(lat, lon, radius, 0.01)
    this.objectText = null
    this.object = null
    this.camera = camera
    this.zoom = zoom
    this.assets = assets
  }

  draw(scene, textColor) {
    const geometry = new THREE.SphereGeometry(this.zoom / 48, 32, 32)
    const material = new THREE.MeshBasicMaterial({ color: 0xff0000 })
    const sphere = new THREE.Mesh(geometry, material)
    sphere.position.copy(this.position)
    scene.add(sphere)

    this.object = sphere
    this.drawText(scene, this.title, this.object, textColor)
  }

  setRadius() {
    if (!this.object) return
    this.object.scale.set(this.zoom / 48, this.zoom / 48, this.zoom / 48)
    // this.objectText.scale.set(this.zoom / 48, this.zoom / 48, this.zoom / 48)
    // this.objectText.position
    //   .copy(this.object.position)
    //   .multiplyScalar(this.zoom / 9)
  }

  drawText(scene, text, sphere, textColor) {
    const geometry = new TextGeometry(text, {
      font: this.assets.font,
      size: this.zoom / 72, // Adjust as needed
      height: 0.01, // Adjust as needed
    })

    const material = new THREE.MeshBasicMaterial({ color: textColor })
    const mesh = new THREE.Mesh(geometry, material)

    // Postion of the text should be away from the sphere and earth
    const spherePosition = sphere.position
    const direction = new THREE.Vector3()
      .subVectors(spherePosition, new THREE.Vector3(0, 0, 0))
      .normalize()

    // Scale the direction by the desired distance
    const distance = this.zoom / 32 // Adjust as needed
    const offset = direction.multiplyScalar(distance)

    // Calculate the text position
    const textPosition = new THREE.Vector3().addVectors(spherePosition, offset)

    mesh.position.copy(textPosition)

    scene.add(mesh)
    this.objectText = mesh

    return this.object
  }

  animate() {
    if (this.objectText) this.objectText.lookAt(this.camera.position)
  }
}
