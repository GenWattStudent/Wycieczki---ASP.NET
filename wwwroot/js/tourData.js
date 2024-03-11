export class TourData {
  constructor(waypoints, startTime, endTime) {
    this.waypoints = waypoints
    this.startTime = startTime
    this.endTime = endTime
    this.earthRadius = 6371 // km
  }

  calculateTotalDistance() {
    let totalDistance = 0
    for (let i = 0; i < this.waypoints.length - 1; i++) {
      totalDistance += this.calculateDistanceBetween(
        this.waypoints[i],
        this.waypoints[i + 1]
      )
    }
    return totalDistance
  }

  calculateDistanceBetween(waypoint, nextWaypoint) {
    let distance = 0
    if (waypoint && nextWaypoint) {
      const lat1 = waypoint.latitude * (Math.PI / 180)
      const lon1 = waypoint.longitude * (Math.PI / 180)
      const lat2 = nextWaypoint.latitude * (Math.PI / 180)
      const lon2 = nextWaypoint.longitude * (Math.PI / 180)
      const dlat = lat2 - lat1
      const dlon = lon2 - lon1
      const a =
        Math.sin(dlat / 2) * Math.sin(dlat / 2) +
        Math.cos(lat1) *
          Math.cos(lat2) *
          Math.sin(dlon / 2) *
          Math.sin(dlon / 2)
      const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a))
      distance = this.earthRadius * c
    }

    return distance
  }

  calculateDistanceToNextWaypoint(percentage) {
    let distance = 0
    if (this.waypoints.length > 1) {
      const nextWaypoint = this.calculateNextWaypoint(percentage)
      distance = this.calculateDistanceBetween(
        this.waypoints[nextWaypoint - 1],
        this.waypoints[nextWaypoint]
      )
    }

    return distance
  }

  calculateDistanceToNextWaypoint(percentOfTime) {
    let totalDistance = 0
    let accumulatedDistance = 0
    let tourLat = 0
    let tourLon = 0
    let nextWaypoint = {}

    // First, calculate the total distance of the tour
    for (let i = 0; i < this.waypoints.length - 1; i++) {
      let point1 = this.waypoints[i]
      let point2 = this.waypoints[i + 1]
      totalDistance += this.calculateDistanceBetween(point1, point2)
    }

    // Then, find the next waypoint based on percentOfTime
    for (let i = 0; i < this.waypoints.length - 1; i++) {
      let point1 = this.waypoints[i]
      let point2 = this.waypoints[i + 1]

      let distanceBetweenPoints = this.calculateDistanceBetween(point1, point2)
      accumulatedDistance += distanceBetweenPoints

      if (accumulatedDistance / totalDistance >= percentOfTime) {
        // Calculate the distance travelled from the previous waypoint
        let distanceTravelled =
          percentOfTime * totalDistance -
          (accumulatedDistance - distanceBetweenPoints)

        // Interpolate the current position based on the distance travelled
        let ratio = distanceTravelled / distanceBetweenPoints
        tourLat = point1.latitude + ratio * (point2.latitude - point1.latitude)
        tourLon =
          point1.longitude + ratio * (point2.longitude - point1.longitude)

        nextWaypoint = point2
        break
      }
    }

    return {
      distance: this.calculateDistanceBetween(
        { latitude: tourLat, longitude: tourLon },
        nextWaypoint
      ),
      tourLat: tourLat,
      tourLon: tourLon,
      nextWaypoint: nextWaypoint,
    }
  }

  calculatePercentComplete() {
    let startTimeDate = new Date(this.startTime)
    let endTimeDate = new Date(this.endTime)
    let currentTimeDate = new Date()

    let percentComplete = 0

    if (currentTimeDate > startTimeDate && currentTimeDate < endTimeDate) {
      let totalTourTime = endTimeDate - startTimeDate
      let timePassed = currentTimeDate - startTimeDate
      percentComplete = timePassed / totalTourTime
    } else if (currentTimeDate > endTimeDate) {
      percentComplete = 1
    } else if (currentTimeDate < startTimeDate) {
      percentComplete = 0
    } else {
      percentComplete = 0
    }

    return percentComplete
  }

  calculatePassedDistance(percentage) {
    let passedDistance = 0
    if (this.waypoints.length > 1) {
      passedDistance = this.calculateTotalDistance() * percentage
    }
    return passedDistance
  }

  calculateTimeToWaypoint(nextWaypoint) {
    let time = 0
    if (nextWaypoint) {
      time = this.calculateTimeBetweenWaypoints(
        this.waypoints[nextWaypoint - 1],
        this.waypoints[nextWaypoint]
      )
    }
    return time
  }

  calculateTimeBetweenWaypoints(start, end) {
    let time = 0
    if (start && end) {
      time = this.calculateDistanceBetween(start, end) / 5
    }
    return time
  }
}
