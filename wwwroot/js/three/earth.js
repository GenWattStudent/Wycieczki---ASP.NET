import * as THREE from 'https://cdn.jsdelivr.net/npm/three@0.163.0/build/three.module.js'
import EarthPlanet from './EarthPlanet.js'
import { OrbitControls } from 'three/addons/controls/OrbitControls.js'
import { latLongToVector3 } from './helpers.js'

export default class EarthScene {
  constructor(assets) {
    this.scene = new THREE.Scene()
    this.camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000)
    this.height = null
    this.width = null
    this.renderer = new THREE.WebGLRenderer()
    this.resize()
    $('#earth').append(this.renderer.domElement)

    this.camera.position.z = 8

    this.controls = new OrbitControls(this.camera, this.renderer.domElement)
    this.controls.mouseButtons = {
      LEFT: THREE.MOUSE.ROTATE,
      MIDDLE: '',
      RIGHT: '',
    }
    this.controls.enableZoom = false

    this.directionalLight = new THREE.DirectionalLight(0xffffff, 1)
    this.directionalLight.position.set(5, 3, 5)

    this.earth = new EarthPlanet(this.scene, this.camera, true, assets)
    this.scene.add(this.directionalLight)
    window.addEventListener('resize', this.resize.bind(this))
  }

  setHeight(height) {
    this.height = height
    this.resize()
  }

  setWidth(width) {
    this.width = width
    this.resize()
  }

  enableZoom(isZoom) {
    this.controls.enableZoom = isZoom
  }

  animate() {
    const animation = () => {
      !this.earth.isLoading ? this.earth.animate() : null
      this.controls.update()
      this.renderer.render(this.scene, this.camera)
      this.renderer.setClearColor(0xffffff, 0)
      requestAnimationFrame(animation)
    }
    animation()
  }

  updateTourLatLong(lat, long, waypoint) {
    // this.earth.updateCamera(
    //   latLongToVector3(lat, long, this.earth.earthRadius, 0.01)
    // )
    this.earth.updateCar(
      latLongToVector3(lat, long, this.earth.earthRadius, 0.01),
      latLongToVector3(waypoint.latitude, waypoint.longitude, this.earth.earthRadius, 0.01)
    )
  }

  resize() {
    let height = this.height
    let width = this.width

    if (!this.height) {
      const offsetTop = $('#earth').offset().top
      const footerHeight = $('#footer').height()
      const footerBorder = 1
      height = window.innerHeight - offsetTop - footerHeight - footerBorder
    }

    if (!this.width) {
      width = window.innerWidth
    }

    this.camera.aspect = width / height
    this.camera.updateProjectionMatrix()
    this.renderer.setSize(width, height)
  }
}
