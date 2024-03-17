import { Map } from './map.js'
import { Waypoint } from './Waypoint.js'

export class UserMap extends Map {
  constructor(waypoints, lat, long) {
    super()
    waypoints.forEach(this.setWaypoint.bind(this))

    if (!lat || !long) {
      return
    }

    this.setWaypoint({
      latitude: lat,
      longitude: long,
      id: new Date().toISOString(),
      type: 4,
      isTourIndicator: true,
    })
  }

  setWaypoint(waypoint) {
    const waypointData = new Waypoint(
      waypoint.latitude,
      waypoint.longitude,
      waypoint.id,
      waypoint.name,
      waypoint.description,
      waypoint.type,
      waypoint.images,
      true,
      waypoint.isRoad
    )
    console.log(waypointData)
    waypointData.isTourIndicator = waypoint.isTourIndicator

    this.waypoints.push(waypointData)
    const type = this.getTypeFromEnum(waypointData.type)
    this.createMarker(
      waypointData,
      false,
      false,
      true,
      this.getIcon(type === 'road' ? '' : type)
    )

    this.connectWaypointsWithLine(this.waypoints)
  }

  updateTourIndicator(lat, long) {
    const tourIndicator = this.waypoints.find(
      (waypoint) => waypoint.isTourIndicator
    )

    if (tourIndicator) {
      tourIndicator.latitude = lat
      tourIndicator.longitude = long
      this.updateMarker(tourIndicator.marker, lat, long)
    }
  }

  getTypeFromEnum(type) {
    switch (type) {
      case 0:
        return 'start'
      case 1:
        return 'marker'
      case 2:
        return 'end'
      case 4:
        return 'indicator'
      default:
        return 'road'
    }
  }
}
