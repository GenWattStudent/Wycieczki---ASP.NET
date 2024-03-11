export class Waypoint {
  constructor(
    lat,
    lng,
    id = new Date().toISOString(),
    name = 'Title',
    description = 'Description',
    type = 'start',
    images = [],
    fromDb = false,
    isRoad = false
  ) {
    this.id = id
    this.lat = lat
    this.lng = lng
    this.name = name
    this.description = description
    this.images = images
    this.fromDb = fromDb
    this.isRoad = isRoad
    this.type = type
    this.isTourIndicator = false
    this.marker = null
  }

  getSubmitData() {
    return {
      id: typeof this.id === 'string' ? 0 : this.id,
      lat: this.lat,
      lng: this.lng,
      name: this.name,
      description: this.description,
      images: this.images,
      isRoad: this.isRoad,
      fromDb: this.fromDb,
      type: this.type,
    }
  }

  createMarker(draggable = false) {
    this.marker = L.marker(
      { lat: this.lat, lng: this.lng },
      { draggable }
    ).addTo(mapAdmin.map)
  }

  update(title, description, images) {
    this.name = title
    this.description = description
    this.images = images
  }

  bindPopup(popupContent) {
    this.marker.bindPopup(popupContent).openPopup()
  }

  on(eventName, eventHandler) {
    this.marker.on(eventName, eventHandler)
  }

  addRemoveListener(eventHandler) {
    document
      .getElementById(`remove-${this.id}`)
      .addEventListener('click', eventHandler)
  }
}
