import { FontLoader } from 'three/addons/loaders/FontLoader.js'
import { TextGeometry } from 'three/addons/geometries/TextGeometry.js'
import { latLongToVector3 } from './helpers.js'
import * as THREE from 'three'

export default class Waypoint3D {
  constructor(lat, lon, title, camera, radius) {
    this.lat = lat
    this.lon = lon
    this.title = title
    this.position = latLongToVector3(lat, lon, radius, 0.01)
    this.objectText = null
    this.object = null
    this.camera = camera
  }

  draw(scene, textColor) {
    const geometry = new THREE.SphereGeometry(0.01, 32, 32)
    const material = new THREE.MeshBasicMaterial({ color: 0xff0000 })
    const sphere = new THREE.Mesh(geometry, material)
    sphere.position.copy(this.position)
    scene.add(sphere)

    this.object = sphere

    this.drawText(scene, this.title, this.object, textColor)
  }

  drawText(scene, text, sphere, textColor) {
    const loader = new FontLoader()

    loader.load(
      'https://threejs.org/examples/fonts/helvetiker_regular.typeface.json',
      function (font) {
        const geometry = new TextGeometry(text, {
          font: font,
          size: 0.01, // Adjust as needed
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
        const distance = 0.01 // Adjust as needed
        const offset = direction.multiplyScalar(distance)

        // Calculate the text position
        const textPosition = new THREE.Vector3().addVectors(
          spherePosition,
          offset
        )

        mesh.position.copy(textPosition)

        scene.add(mesh)
        this.objectText = mesh
      }.bind(this)
    )

    return this.object
  }

  animate() {
    if (this.objectText) this.objectText.lookAt(this.camera.position)
  }
}
