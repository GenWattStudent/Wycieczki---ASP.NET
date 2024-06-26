import { defaultMarker, endMarker, waypointMarker, startMarker, tourIndicator, noMarker } from './markers.js'

export class Map {
  constructor() {
    this.waypoints = []
    this.map = L.map('map').setView([51.505, -0.09], 13)
    $.get('/Tour/GetMapKey', (res) => {
      L.maptilerLayer({
        apiKey: res.key,
        style: '04f9d487-86a6-463f-a5c7-4d4103d7818a',
      }).addTo(this.map)
    })
  }

  createWayPointPopup(title, description, images, id, isUser = false) {
    if (images.length && images[0] instanceof File) {
      images = images.map((image) => URL.createObjectURL(image))
    } else if (images.$values) {
      images = images.$values.map((image) => image.imageUrl)
    } else {
      images = []
    }
    const imageDivs = images
      .map(
        (image, index) => `
      <div class="carousel-item ${index === 0 ? 'active' : ''}">
          <img src="${image}" class="d-block w-100" alt="Image ${index + 1}" />
      </div>`
      )
      .join('')

    return `
      <div>
        <h4>${title}</h4>
        <p>${description}</p>
        
        <div id="slider-1" class="carousel slide" data-ride="carousel" data-interval="4000">
          <div class="carousel-inner">
            ${imageDivs}
          </div>
          <a class="carousel-control-prev" href="#slider-1" role="button" data-slide="prev">
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
          </a>
          <a class="carousel-control-next" href="#slider-1" role="button" data-slide="next">
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
          </a>
        </div>
  
        ${!isUser ? `<button id="remove-${id}" type="button" class="btn btn-primary mt-2 w-100">Remove</button>` : ''}
      </div>
    `
  }

  createEditPopup(waypointData) {
    const { name, description, images } = waypointData

    const imageDivs = images
      .map(
        (image, index) => `
      <div class="carousel-item ${index === 0 ? 'active' : ''}">
        <div class="position-relative d-flex justify-content-center" style="z-index=1;">
          <a href="/Image/DeleteImage/?id=${image.id}&tourId=${
          waypointData.id
        }" style="right: 50%; transform: translateX(50%); top: 5px; z-index:100l" class="position-absolute text-light h3 bg-danger m-0">
              <ion-icon name="close-outline"></ion-icon>
          </a>
          <img src="${image.imageUrl}" style="height: 100px; object-fit: cover;" />
        </div>
      </div>
    `
      )
      .join('')
    console.log(waypointData.id)
    return `
      <div>
        <h4>${name}</h4>
        <p>${description}</p>
        
        <div id="slider-1" class="carousel slide" data-ride="carousel" data-interval="4000">
          <div class="carousel-inner">
            ${imageDivs}
          </div>
          <a class="carousel-control-prev" href="#slider-1" role="button" data-slide="prev">
            <span class="carousel-control-prev-icon" aria-hidden="true"></span>
          </a>
          <a class="carousel-control-next" href="#slider-1" role="button" data-slide="next">
            <span class="carousel-control-next-icon" aria-hidden="true"></span>
          </a>
        </div>
  
        <a href="/Waypoint/Delete/${waypointData.id}?agencyId=${waypointData.agencyId}">
          <button type="button" class="btn btn-primary mt-2 w-100">Delete waypoint</button>
        </a>
      </div>`
  }

  updateMarker(marker, lat, lng) {
    marker.setLatLng({ lat, lng })
  }

  removeWayPoint(id) {
    const waypoint = this.waypoints.find((w) => w.id == id)

    if (waypoint) {
      this.map.removeLayer(waypoint.marker)
      this.waypoints = this.waypoints.filter((w) => w.id != id)
    }

    this.connectWaypointsWithLine(this.waypoints)
    document.dispatchEvent(new CustomEvent('waypointRemoved', { detail: { id } }))
  }

  connectWaypointsWithLine(waypoints) {
    this.clearLines()
    const latlngs = waypoints.filter((w) => !w.isTourIndicator).map((w) => [w.lat, w.lng])
    L.polyline(latlngs, { color: 'blue' }).addTo(this.map)
  }

  getIcon(currentTool) {
    switch (currentTool) {
      case 'road':
        return defaultMarker
      case 'start':
        return startMarker
      case 'end':
        return endMarker
      case 'marker':
        return waypointMarker
      case 'indicator':
        return tourIndicator
      default:
        return noMarker
    }
  }

  static getTypeFromEnum(type) {
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

  createMarker(waypointData, edit = false, draggable = false, isUser = false, icon = defaultMarker) {
    const marker = L.marker({ lat: waypointData.lat, lng: waypointData.lng }, { draggable, icon }).addTo(this.map)

    const popup = edit
      ? this.createEditPopup(waypointData)
      : this.createWayPointPopup(
          waypointData.name,
          waypointData.description,
          waypointData.images,
          waypointData.id,
          isUser
        )

    waypointData.marker = marker

    if (waypointData.isTourIndicator) {
      return
    }

    marker
      .bindPopup(popup)
      .openPopup()
      .on('click', () => {
        if (this.selectWaypoint) this.selectWaypoint(waypointData, edit)
      })
      .on('dragend', (e) => {
        waypointData.lat = e.target._latlng.lat
        waypointData.lng = e.target._latlng.lng
        this.connectWaypointsWithLine(this.waypoints)
        document.dispatchEvent(new CustomEvent('waypointMoved', { detail: { waypoint: waypointData } }))
      })
      .on('popupopen', () => {
        if (!edit && !isUser)
          document
            .getElementById(`remove-${waypointData.id}`)
            .addEventListener('click', () => this.removeWayPoint(waypointData.id))
      })

    if (!edit && !isUser)
      document
        .getElementById(`remove-${waypointData.id}`)
        .addEventListener('click', () => this.removeWayPoint(waypointData.id))
    return marker
  }

  clearLines() {
    this.map.eachLayer((layer) => {
      if (layer instanceof L.Polyline) {
        this.map.removeLayer(layer)
      }
    })
  }
}
