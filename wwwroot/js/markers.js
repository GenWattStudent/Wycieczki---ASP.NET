export const startMarker = L.divIcon({
  className:
    'start-marker bg-primary border-light custom-marker d-flex align-items-center justify-content-center',
  iconAnchor: [15, 15],
  html: '<span class="border-light"></span><span class="border-light"></span><span class="border-light"></span> <p class="m-0 text-light text-uppercase font-weight-bold">S</p>',
})

export const endMarker = L.divIcon({
  className:
    'start-marker bg-primary border-light custom-marker d-flex align-items-center justify-content-center',
  html: '<span class="border-light"></span><span class="border-light"></span><span class="border-light"></span> <p class="m-0 text-light text-uppercase font-weight-bold">E</p>',
  iconSize: [30, 30],
  iconAnchor: [15, 30],
})

export const waypointMarker = L.divIcon({
  className: 'waypoint-marker',
  html: '<ion-icon name="star"></ion-icon>',
  iconSize: [30, 30],
  iconAnchor: [15, 30],
})

export const defaultMarker = L.divIcon({
  className: 'default-marker bg-primary',
  iconAnchor: [8, 8],
})

export const tourIndicator = L.divIcon({
  className: 'tour-indicator text-success icon-marker',
  html: '<span class="border-success"></span><span class="border-success"></span><span class="border-success"></span><ion-icon name="people-circle"></ion-icon>',
  iconAnchor: [15, 15],
})

export const noMarker = L.divIcon({
  className: 'no-marker',
  iconAnchor: [8, 8],
})
