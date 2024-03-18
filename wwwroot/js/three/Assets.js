import { FontLoader } from 'three/addons/loaders/FontLoader.js'
import { GLTFLoader } from 'three/addons/loaders/GLTFLoader.js'
import { TextureLoader } from 'three'

export class Assets {
  constructor() {
    this.dayTexture = null
    this.nightTexture = null
    this.carMesh = null
    this.font = null
  }

  loadAssets = () => {
    const loader = new TextureLoader()
    const fontLoader = new FontLoader()
    const meshLoader = new GLTFLoader()

    return new Promise((resolve, reject) => {
      loader.load('/images/three/earth_day.jpg', (dayTexture) => {
        this.dayTexture = dayTexture
        loader.load('/images/three/earth_night.jpg', (nightTexture) => {
          this.nightTexture = nightTexture
          fontLoader.load(
            'https://threejs.org/examples/fonts/helvetiker_regular.typeface.json',
            (font) => {
              this.font = font
              meshLoader.load('/images/three/m.glb', (carMesh) => {
                this.carMesh = carMesh
                resolve()
              })
            }
          )
        })
      })
    })
  }
}
