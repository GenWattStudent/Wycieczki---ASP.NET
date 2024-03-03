const map = L.map('map').setView([51.505, -0.09], 13)

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution:
    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
}).addTo(map)

function createWayPointPopup(title, description, image, id) {
  return `
        <div>
            <h4>${title}</h4>
            <p>${description}</p>
            
            <div id="slider-1" class="carousel slide" data-ride="carousel" data-interval="4000">
              <div class="carousel-inner">
                  <div class="carousel-item active">
                    ${image ? `<img class="img-fluid" src="${image}" /> ` : ''}
                  </div>
                  <div class="carousel-item">
                    ${image ? `<img class="img-fluid" src="${image}" /> ` : ''}
                  </div>
                  <div class="carousel-item">
                    ${image ? `<img class="img-fluid" src="${image}" /> ` : ''}
                  </div>
              </div>
              <a class="carousel-control-prev" href="#slider-1" role="button" data-slide="prev">
                  <span class="carousel-control-prev-icon" aria-hidden="true"></span>
              </a>
              <a class="carousel-control-next" href="#slider-1" role="button" data-slide="next">
                  <span class="carousel-control-next-icon" aria-hidden="true"></span>
              </a>
            </div>

            <button type="button" class="btn btn-primary mt-2" onclick="removeWayPoint(${id})">Remove</button>
        </div>
    `
}

function removeWayPoint(id) {
  const waypoint = waypoints.find((w) => w.id === id)

  if (waypoint) {
    map.removeLayer(waypoint.marker)
    waypoints = waypoints.filter((w) => w.id !== id)
  }

  connectWaypointsWithLine(waypoints)
}

function createWaypointData(lat, lng, marker) {
  return {
    lat: lat,
    lng: lng,
    marker: marker,
    name: `Waypoint ${waypoints.length + 1}`,
    description: 'Simple description :)',
    image: null,
    id: Date.now(),
  }
}

function createWaypointForm(waypointData) {
  return `
        <div>
            <label for="waypoint-title">Title</label>
            <div class="input-group mt-1">
                <input class="form-control w-100" type="text" id="waypoint-title" value="${waypointData.name}" />
            </div>
            <label for="waypoint-description">Description</label>
            <div class="input-group mt-1">
                <textarea class="form-control w-100" id="waypoint-description">${waypointData.description}</textarea>
            </div>
            <label for="waypoint-image">Image</label>
            <input class="form-control w-100" type="file" id="waypoint-image" />
            <button type="button" class="btn btn-primary mt-2" onclick="updateWaypoint(${waypointData.id})">Update</button>
        </div>
    `
}
