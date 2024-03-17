import { Assets } from './Assets.js'
import EarthScene from './earth.js'
import { latLongToVector3 } from './helpers.js'

export class EarthOptions {
  constructor(width, height, zoom) {
    this.width = width
    this.height = height
    this.zoom = zoom
  }
}

function setupGlob(waypointsObj, options) {
  const assets = new Assets()

  assets.loadAssets().then(() => {
    const earthScene = new EarthScene(assets)

    earthScene.earth.setWaypoints(waypointsObj)
    earthScene.setHeight(options.height)
    earthScene.setWidth(options.width)
    earthScene.enableZoom(true)
    earthScene.earth.drawEarth()
    earthScene.earth.setTextureBasedOnTime()
    earthScene.earth.drawRoad()
    earthScene.earth.isAnimate = false
    earthScene.earth.setZoom(options.zoom)
    earthScene.earth.updateCamera(
      latLongToVector3(
        waypointsObj[0].lat,
        waypointsObj[0].lng,
        earthScene.earth.earthRadius,
        0.01
      )
    )
    earthScene.animate()
  })
}

export { setupGlob }
