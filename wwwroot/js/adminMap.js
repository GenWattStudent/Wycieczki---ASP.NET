let waypoints = []
let currentTool = 'marker'

function createSubmitData(waypointData) {
  console.log(waypointData)
  const { marker, ...data } = waypointData
  return {
    isRoad: marker ? false : true,
    ...data,
    id: (typeof waypointData.id === 'string' ? 0 : waypointData.id).toString(),
  }
}

function updateWaypoint(id, edit = false) {
  console.log('update', id, waypoints)
  const waypoint = waypoints.find((w) => w.id == id)

  if (waypoint) {
    const title = document.getElementById('waypoint-title').value
    const description = document.getElementById('waypoint-description').value
    let images = []
    waypoint.name = title
    waypoint.description = description

    if (!edit) {
      const imagesEL = document.getElementById('waypoint-image').files
      waypoint.images = Array.from(imagesEL)
    }

    const popup = edit
      ? createEditPopup(waypoint)
      : createWayPointPopup(title, description, waypoint.images, id)

    const imageUrls = []
    if (!edit) {
      for (let i = 0; i < images.length; i++) {
        imageUrls.push(URL.createObjectURL(images[i]))
      }
    }

    waypoint.marker.bindPopup(popup).openPopup()

    console.log(waypoints)
  }
}

function selectWaypoint(waypointData, edit = false) {
  const form = createWaypointForm(waypointData, edit)
  document.getElementById('waypoint-form').innerHTML = form
}

function clearLines() {
  map.eachLayer((layer) => {
    if (layer instanceof L.Polyline) {
      map.removeLayer(layer)
    }
  })
}

function connectWaypointsWithLine(waypoints) {
  clearLines()
  const latlngs = waypoints.map((w) => [w.lat, w.lng])
  L.polyline(latlngs, { color: 'red' }).addTo(map)
}

function addWayPointClick(e) {
  const waypointData = createWaypointData(e.latlng.lat, e.latlng.lng, null)
  addWaypoint(waypointData, currentTool)
}

function createMarker(waypointData, edit = false) {
  const marker = L.marker(
    { lat: waypointData.lat, lng: waypointData.lng },
    { draggable: true }
  ).addTo(map)

  const popup = edit
    ? createEditPopup(waypointData)
    : createWayPointPopup(
        waypointData.name,
        waypointData.description,
        waypointData.images,
        waypointData.id
      )

  marker
    .bindPopup(popup)
    .openPopup()
    .on('click', () => selectWaypoint(waypointData, edit))
    .on('dragend', (e) => {
      waypointData.lat = e.target._latlng.lat
      waypointData.lng = e.target._latlng.lng
      connectWaypointsWithLine(waypoints)
    })

  return marker
}

function addWaypoint(waypointData, currentTool, edit = false) {
  if (currentTool === 'marker') {
    const marker = createMarker(waypointData, edit)
    waypointData.marker = marker
  }

  waypoints.push(waypointData)

  connectWaypointsWithLine(waypoints)
}

function selectTool(tool) {
  currentTool = tool
  const buttons = document.querySelectorAll('#toolbar button')
  buttons.forEach((b) => {
    if (b.dataset.tool === tool) {
      b.classList.remove('bg-secondary')
      b.classList.add('bg-primary')
    } else {
      b.classList.remove('bg-primary')
      b.classList.add('bg-secondary')
    }
  })
}

selectTool(currentTool)
map.on('click', addWayPointClick)
