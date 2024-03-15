import * as THREE from 'three'
import EarthPlanet from './EarthPlanet.js'
import { OrbitControls } from 'three/addons/controls/OrbitControls.js'

const scene = new THREE.Scene()
const camera = new THREE.PerspectiveCamera(
  75,
  window.innerWidth / window.innerHeight,
  0.1,
  1000
)

const renderer = new THREE.WebGLRenderer()
resize()
$('#earth').append(renderer.domElement)

camera.position.z = 10

const controls = new OrbitControls(camera, renderer.domElement)

const directionalLight = new THREE.DirectionalLight(0xffffff, 1)
directionalLight.position.set(5, 3, 5)

const earth = new EarthPlanet(scene, camera)

scene.add(directionalLight)

function animate() {
  earth.animate()
  controls.update()
  renderer.render(scene, camera)
  renderer.setClearColor(0xffffff, 0)
  requestAnimationFrame(animate)
}

animate()

function resize() {
  const offsetTop = $('#earth').offset().top
  const footerHeight = $('#footer').height()
  const height = window.innerHeight - offsetTop - footerHeight - 5

  camera.aspect = window.innerWidth / height
  camera.updateProjectionMatrix()
  renderer.setSize(window.innerWidth, height)
}

// Add a resize event listener
window.addEventListener('resize', resize)
