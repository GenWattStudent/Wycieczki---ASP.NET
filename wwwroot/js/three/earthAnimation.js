import EarthScene from './earth.js'
import { Waypoint } from '../Waypoint.js'

const waypoint1 = new Waypoint(
  51.5074,
  0.1278,
  new Date().toISOString(),
  'London'
)
waypoint1.type = 'start'

const waypoint2 = new Waypoint(
  40.7128,
  -74.006,
  new Date().toISOString(),
  'New York'
)
waypoint2.type = 'road'

const waypoint3 = new Waypoint(
  35.6895,
  139.6917,
  new Date().toISOString(),
  'Tokyo'
)
waypoint3.type = 'marker'

const waypoint4 = new Waypoint(
  37.7749,
  -122.4194,
  new Date().toISOString(),
  'San Francisco'
)
waypoint4.type = 'end'

const waypoints = [waypoint1, waypoint2, waypoint3, waypoint4]
const earthScence = new EarthScene()

earthScence.earth.setWaypoints(waypoints)
earthScence.earth.loadAssets().then(() => {
  earthScence.earth.drawEarth()
  earthScence.animate()
})
