import { Map } from './map.js'
import { Waypoint } from './Waypoint.js'

export class UserMap extends Map {
  constructor(waypoints) {
    super()

    waypoints.forEach((waypoint) => {
      const waypointData = new Waypoint(
        waypoint.latitude,
        waypoint.longitude,
        waypoint.id,
        waypoint.name,
        waypoint.description,
        waypoint.images,
        true,
        waypoint.isRoad
      )

      this.waypoints.push(waypointData)

      if (!waypoint.isRoad) {
        this.createMarker(waypointData, false, false, true)
      }

      this.connectWaypointsWithLine(this.waypoints)
    })
  }
}
