import * as THREE from 'three'
import { FontLoader } from 'three/addons/loaders/FontLoader.js'
import { TextGeometry } from 'three/addons/geometries/TextGeometry.js'
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js'

class Car {
  constructor(scene) {
    this.scene = scene
    this.car = this.create()

    // this.scene.add(this.car)
  }

  create = () => {
    const loader = new GLTFLoader()
    loader.load('/images/three/m.glb', (gltf) => {
      this.scene.add(gltf.scene)
      this.car = gltf.scene
      this.car.scale.set(0.2, 0.2, 0.2)
    })
  }
}

export default class EarthPlanet {
  constructor(scene, camera) {
    this.scene = scene
    this.waypoints = [
      { lat: 51.5074, lon: 0.1278, title: 'London' }, // London
      { lat: 40.7128, lon: -74.006, title: 'New York' }, // New York
      { lat: 35.6895, lon: 139.6917, title: 'Tokyo' }, // Tokyo
      { lat: 37.7749, lon: -122.4194, title: 'San Franciso' }, // San Francisco
    ]
    this.currentPoint = 0
    this.lerpProgress = 0
    this.lerpSpeed = 0.01
    this.points = this.waypoints.map(function (waypoint) {
      return latLongToVector3(waypoint.lat, waypoint.lon, 4, 0.01)
    })
    this.camera = camera
    this.waypointsObjects = []
    this.textObjects = []
    this.earthRadius = 4

    this.earth = this.create()
    this.car = new Car(this.scene)
  }

  create = () => {
    const loader = new THREE.TextureLoader()
    const earthDayTexture = loader.load('/images/three/earth_night.jpg')

    const earthGeometry = new THREE.SphereGeometry(this.earthRadius, 32, 32)
    const earthMaterial = new THREE.MeshBasicMaterial({ color: 0xffffff })
    const earth = new THREE.Mesh(earthGeometry, earthMaterial)

    earth.material.map = earthDayTexture
    this.scene.add(earth)
    this.drawWaypoint(this.waypoints[this.currentPoint])
    return earth
  }

  animate() {
    if (this.currentPoint < this.points.length - 1) {
      const nextPosition = new THREE.Vector3().lerpVectors(
        this.points[this.currentPoint],
        this.points[this.currentPoint + 1],
        this.lerpProgress
      )

      const positions = serArc3D(
        this.points[this.currentPoint],
        nextPosition,
        100,
        false
      )

      const arc = this.createArc(positions)
      this.scene.add(arc)

      this.updateCamera(nextPosition)
      console.log(this.car.car)
      if (this.car.car) {
        const sphereCenter = new THREE.Vector3(0, 0, 0) // Replace with the center of your sphere
        const carPosition = nextPosition
          .clone()
          .sub(sphereCenter)
          .normalize()
          .multiplyScalar(this.earthRadius)
        this.car.car.position.copy(carPosition)

        // Rotate the car to match the surface of the sphere
        this.car.car.up.copy(carPosition.clone().normalize())
        this.car.car.lookAt(this.points[this.currentPoint + 1])
      }
      // Update the lerp progress
      this.lerpProgress += this.lerpSpeed
      if (this.lerpProgress >= 1) {
        // Move to the next point
        this.currentPoint++
        this.lerpProgress = 0
        this.drawWaypoint(this.waypoints[this.currentPoint])
      }
    }

    this.textObjects.forEach((textObject) => {
      textObject.lookAt(this.camera.position)
    })
  }

  createArc(positions) {
    const geometry = new THREE.BufferGeometry()
    geometry.setAttribute(
      'position',
      new THREE.Float32BufferAttribute(positions, 3)
    )
    return new THREE.Line(
      geometry,
      new THREE.LineBasicMaterial({ color: 0x0000ff, linewidth: 2 })
    )
  }

  updateCamera(nextPosition) {
    const spherePosition = this.earth.position // Replace with the actual position of the sphere
    const cameraOffset = nextPosition
      .clone()
      .sub(spherePosition)
      .normalize()
      .multiplyScalar(10)
    this.camera.position.copy(spherePosition).add(cameraOffset)

    this.camera.lookAt(spherePosition)
  }

  drawWaypoint(waypoint) {
    const { lat, lon } = waypoint
    const position = latLongToVector3(lat, lon, this.earthRadius, 0.01)
    const geometry = new THREE.SphereGeometry(0.1, 32, 32)
    const material = new THREE.MeshBasicMaterial({ color: 0xff0000 })
    const sphere = new THREE.Mesh(geometry, material)

    this.drawText(waypoint.title, sphere)
    sphere.position.copy(position)
    this.waypointsObjects.push(sphere)
    this.scene.add(sphere)
  }

  drawText(text, sphere) {
    const loader = new FontLoader()

    loader.load(
      'https://threejs.org/examples/fonts/helvetiker_regular.typeface.json',
      function (font) {
        const geometry = new TextGeometry(text, {
          font: font,
          size: 0.15, // Adjust as needed
          height: 0.01, // Adjust as needed
        })

        const material = new THREE.MeshBasicMaterial({ color: 0x000000 })
        const mesh = new THREE.Mesh(geometry, material)

        // Postion of the text should be away from the sphere and earth
        const earthPosition = this.earth.position
        const spherePosition = sphere.position
        const direction = new THREE.Vector3()
          .subVectors(spherePosition, earthPosition)
          .normalize()

        // Scale the direction by the desired distance
        const distance = 0.6 // Adjust as needed
        const offset = direction.multiplyScalar(distance)

        // Calculate the text position
        const textPosition = new THREE.Vector3().addVectors(
          spherePosition,
          offset
        )

        mesh.position.copy(textPosition)

        this.textObjects.push(mesh)
        this.scene.add(mesh)
      }.bind(this)
    )
  }
}

export function latLongToVector3(lat, lon, radius, heigth) {
  var phi = (lat * Math.PI) / 180
  var theta = ((lon - 180) * Math.PI) / 180

  var x = -(radius + heigth) * Math.cos(phi) * Math.cos(theta)
  var y = (radius + heigth) * Math.sin(phi)
  var z = (radius + heigth) * Math.cos(phi) * Math.sin(theta)

  return new THREE.Vector3(x, y, z)
}

export function serArc3D(pointStart, pointEnd, smoothness, clockWise) {
  const cb = new THREE.Vector3()
  const ab = new THREE.Vector3()
  const normal = new THREE.Vector3()

  cb.subVectors(new THREE.Vector3(), pointEnd)
  ab.subVectors(pointStart, pointEnd)
  cb.cross(ab)

  normal.copy(cb).normalize()

  const angle = pointStart.angleTo(pointEnd)

  if (clockWise) angle = Math.PI * 2 - angleValue

  const angleDelta = angle / (smoothness - 1)

  let positions = []
  for (let i = 0; i < smoothness; i++) {
    let vertex = pointStart.clone().applyAxisAngle(normal, angleDelta * i)
    positions.push(vertex.x, vertex.y, vertex.z)
  }

  return positions
}
