import * as THREE from 'three'
import { latLongToVector3, serArc3D } from './helpers.js'
import Waypoint3D from './Waypoint3D.js'
import Car from './Car.js'
import { Map } from '../map.js'

export default class EarthPlanet {
  constructor(scene, camera, isAnimate = true, assets) {
    this.scene = scene
    this.isAnimate = isAnimate
    this.waypoints = []
    this.currentPoint = 0
    this.lerpProgress = 0
    this.lerpSpeed = 0.01
    this.camera = camera
    this.waypointsObjects = []
    this.textObjects = []
    this.earthRadius = 5
    this.points = []
    this.isDay = null
    this.car = new Car(this.scene, assets)
    this.zoom = 10
    this.assets = assets
  }

  drawEarth() {
    this.earth = this.create()
  }

  setWaypoints(waypoints) {
    this.waypoints = waypoints
    this.setPoints(waypoints)
  }

  setPoints(waypoints) {
    this.points = waypoints.map((waypoint) => {
      return latLongToVector3(
        waypoint.lat,
        waypoint.lng,
        this.earthRadius,
        0.01
      )
    })
  }

  setZoom(zoom) {
    this.zoom = zoom
    this.waypointsObjects.forEach((waypoint) => {
      waypoint.setRadius()
    })
  }

  create = () => {
    const earthGeometry = new THREE.SphereGeometry(this.earthRadius, 32, 32)
    const earthMaterial = new THREE.MeshBasicMaterial({ color: 0xffffff })
    const earth = new THREE.Mesh(earthGeometry, earthMaterial)

    this.scene.add(earth)
    return earth
  }

  changeTexture = (texture, obj) => {
    // obj.material.needsUpdate = true
    obj.material.generateMipmaps = true
    obj.material.map = texture
  }

  setTextureBasedOnTime = () => {
    const date = new Date()
    const hours = date.getHours()

    if (hours >= 6 && hours < 18 && (!this.isDay || this.isDay === null)) {
      this.changeTexture(this.assets.dayTexture, this.earth)
      this.isDay = true
      console.log('day')
    } else if (hours >= 18 && this.isDay) {
      this.changeTexture(this.assets.nightTexture, this.earth)
      this.isDay = false
      console.log('night')
    }
  }

  animate() {
    this.setTextureBasedOnTime()
    if (this.isAnimate && this.currentPoint < this.points.length - 1) {
      if (this.currentPoint === 0 && !this.isDrawn) {
        this.isDrawn = true
        this.createWaypoint(this.waypoints[0])
      }

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
      this.updateCar(nextPosition, this.points[this.currentPoint + 1])

      // Update the lerp progress
      this.lerpProgress += this.lerpSpeed
      if (this.lerpProgress >= 1) {
        // Move to the next point
        this.currentPoint++
        this.lerpProgress = 0
        this.createWaypoint(this.waypoints[this.currentPoint])
      }
    }

    this.waypointsObjects.forEach((waypoint) => {
      waypoint.animate()
    })
  }

  drawRoad() {
    this.waypoints.forEach((waypoint, index) => {
      const waypointType = Map.getTypeFromEnum(waypoint.type)
      if (
        index < this.waypoints.length - 1 &&
        waypointType !== 'indicator' &&
        this.waypoints[index + 1].type !== 'indicator'
      ) {
        const positions = serArc3D(
          this.points[index],
          this.points[index + 1],
          100,
          false
        )
        const arc = this.createArc(positions)
        this.scene.add(arc)
      }

      if (waypointType !== 'road' && waypointType !== 'indicator') {
        this.createWaypoint(waypoint)
      }

      if (waypointType === 'indicator') {
        let lookAt
        if (this.points[index + 1]) {
          // If there is a next waypoint, make the car look at it
          lookAt = this.points[index + 1]
        } else {
          // If there is no next waypoint, make the car continue looking in the same direction
          const prevPoint = this.points[index - 1]
          const currentPoint = this.points[index]
          lookAt = new THREE.Vector3()
            .subVectors(currentPoint, prevPoint)
            .add(currentPoint)
        }
        this.updateCar(this.points[index], lookAt)
      }
    })
  }

  updateCar(nextPosition, lookat) {
    const sphereCenter = new THREE.Vector3(0, 0, 0) // Replace with the center of your sphere
    const carPosition = nextPosition
      .clone()
      .sub(sphereCenter)
      .normalize()
      .multiplyScalar(this.earthRadius)
    this.car.car.position.copy(carPosition)

    // Rotate the car to match the surface of the sphere
    this.car.car.up.copy(carPosition.clone().normalize())
    this.car.car.lookAt(lookat)
  }

  createWaypoint(waypointData) {
    const waypoint = new Waypoint3D(
      waypointData.lat,
      waypointData.lng,
      waypointData.name,
      this.camera,
      this.earthRadius,
      this.zoom,
      this.assets
    )

    waypoint.draw(this.scene, this.isDay ? 0x000000 : 0xffffff)
    this.waypointsObjects.push(waypoint)
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
      .multiplyScalar(this.zoom)
    this.camera.position.copy(spherePosition).add(cameraOffset)
    this.camera.lookAt(spherePosition)
  }
}
