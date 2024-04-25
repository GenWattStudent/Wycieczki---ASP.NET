import { TourData } from '/js/tourData.js'

function reloadWhenTourStarts(startDate, isTourStarted) {
  if (!isTourStarted && new Date(startDate) < new Date()) {
    location.reload()
  }
}

function tourEnded(endDate, isTourEnded) {
  if (isTourEnded || new Date(endDate) < new Date()) {
    $('#TourEnded').modal('show')
  }
}

export function updateTourData(userMap, earthScene, waypoints, startDate, endDate, isTourStarted, isTourEnded) {
  reloadWhenTourStarts(startDate, isTourStarted)
  tourEnded(endDate, isTourEnded)

  if (!isTourStarted) return

  const tourData = new TourData(
    waypoints.filter((w) => w.type !== 3),
    startDate,
    endDate
  )
  const completed = tourData.calculatePercentComplete()
  const data = tourData.calculateDistanceToNextWaypoint(completed)

  $('#passed').text(`Passed ${tourData.calculatePassedDistance(completed).toFixed(3)} km`)
  $('#completed').text(`${(completed * 100).toFixed(1)}% completed`)
  $('#distanceToNextWaypoint').html(
    `${data.distance.toFixed(3)} km to <strong class="text-primary">${data.nextWaypoint.name}</strong>`
  )

  $('[data-tour-waypoint]').removeClass('active')
  $('[data-tour-waypoint-arrow]').removeClass('active')

  $(`#waypoint-arrow-${data.nextWaypoint.id}`)?.addClass('active')
  $(`#waypoint-${data.nextWaypoint.id}`)?.addClass('text-primary')

  if (userMap) userMap.updateTourIndicator(data.tourLat, data.tourLon)
  if (earthScene) earthScene.updateTourLatLong(data.tourLat, data.tourLon, data.nextWaypoint)
}
