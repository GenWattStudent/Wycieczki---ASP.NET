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

    waypointData.isTourIndicator = waypoint.isTourIndicator

    this.waypoints.push(waypointData)
    console.log(waypointData, 'waypointData')
    this.createMarker(
      waypointData,
      false,
      false,
      true,
      this.getIcon(this.getTypeFromEnum(waypointData.type))
    )

    this.connectWaypointsWithLine(this.waypoints)
  }

  updateTourIndicator(lat, long) {
    console.log(this.waypoints)
    const tourIndicator = this.waypoints.find(
      (waypoint) => waypoint.isTourIndicator
    )

    console.log(tourIndicator, 'tourIndicator')
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
