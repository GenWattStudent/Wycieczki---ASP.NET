import EarthScene from './earth.js'
import { Waypoint } from '../Waypoint.js'
import { Assets } from './Assets.js'

const waypoint1 = new Waypoint(51.5074, 0.1278, new Date().toISOString(), 'London')
waypoint1.type = 'start'

const waypoint2 = new Waypoint(40.7128, -74.006, new Date().toISOString(), 'New York')
waypoint2.type = 'road'

const waypoint3 = new Waypoint(35.6895, 139.6917, new Date().toISOString(), 'Tokyo')
waypoint3.type = 'marker'

const waypoint4 = new Waypoint(37.7749, -122.4194, new Date().toISOString(), 'San Francisco')
waypoint4.type = 'end'

const waypoints = [waypoint1, waypoint2, waypoint3, waypoint4]

const assets = new Assets()

function clamp(value, min, max) {
  return Math.min(Math.max(value, min), max)
}

assets.loadAssets().then(() => {
  const earthScence = new EarthScene(assets)
  earthScence.earth.setWaypoints(waypoints)
  const main = $('main')
  const mainWidth = main.width()
  const mainHeight = main.height()
  earthScence.earth.earthRadius = 5.5
  earthScence.earth.drawEarth()

  earthScence.setWidth(mainWidth / 2)
  earthScence.setHeight(clamp(mainHeight - main.offset().top - $('footer').height() - 80, 100, mainWidth / 2))

  earthScence.animate()
})
