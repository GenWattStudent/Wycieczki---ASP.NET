const map = L.map('map').setView([51.505, -0.09], 13)

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution:
    '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
}).addTo(map)

function createWayPointPopup(title, description, images, id) {
  console.log(images, 'Asdsadasdsa')

  if (images.length && images[0] instanceof File) {
    images = images.map((image) => URL.createObjectURL(image))
  } else {
    images = images.map((image) => image.imageUrl)
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

      <button type="button" class="btn btn-primary mt-2 w-100" onclick="removeWayPoint('${id}')">Remove</button>
    </div>
  `
}

function createEditPopup(waypointData) {
  const { name, description, images } = waypointData
  const imageDivs = images
    .map(
      (image, index) => `
    <div class="carousel-item ${index === 0 ? 'active' : ''}">
      <div class="position-relative d-flex justify-content-center" style="z-index=1;">
        <a href="/Tour/DeleteImage/?id=${image.id}&tourId=${
        waypointData.id
      }" style="right: 50%; transform: translateX(50%); top: 5px; z-index:100l" class="position-absolute text-light h3 bg-danger m-0">
            <ion-icon name="close-outline"></ion-icon>
        </a>
        <img src="${
          image.imageUrl
        }" style="height: 100px; object-fit: cover;" />
      </div>
    </div>
  `
    )
    .join('')

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

      <a href="/Waypoint/Delete/${waypointData.id}">
        <button type="button" class="btn btn-primary mt-2 w-100">Delete waypoint</button>
      </a>
    </div>`
}

function removeWayPoint(id) {
  const waypoint = waypoints.find((w) => w.id == id)

  if (waypoint) {
    map.removeLayer(waypoint.marker)
    waypoints = waypoints.filter((w) => w.id != id)
  }

  console.log(waypoints)

  connectWaypointsWithLine(waypoints)
}

function createWaypointData(
  lat,
  lng,
  marker,
  name = `Waypoint ${waypoints.length + 1}`,
  description = 'Simple description :)',
  images = [],
  id = new Date().toISOString(),
  fromDb = false
) {
  return {
    lat,
    lng,
    marker,
    name,
    description,
    images,
    id,
    fromDb,
  }
}

function createWaypointForm(waypointData, edit = false) {
  console.log(waypointData)
  return `
        <div>
            <label for="waypoint-title">Title</label>
            <div class="input-group mt-1">
                <input class="form-control w-100" type="text" id="waypoint-title" value="${
                  waypointData.name
                }" />
            </div>
            <label for="waypoint-description">Description</label>
            <div class="input-group mt-1">
                <textarea class="form-control w-100" id="waypoint-description">${
                  waypointData.description
                }</textarea>
            </div>
            <label for="waypoint-image">Image</label>
            ${
              !edit
                ? `<input class="form-control w-100" type="file" id="waypoint-image" multiple />`
                : ''
            }
            ${
              edit
                ? `<input class="form-control w-100" type="file" id="waypoint-image" multiple onchange="addImages(${waypointData.id}, event)"/>`
                : ''
            }
            ${
              !edit
                ? `<button type="button" class="btn btn-primary mt-2" onclick="updateWaypoint('${waypointData.id}')">Update</button>`
                : ''
            }
            ${
              edit === true
                ? `<button type="button" class="btn btn-primary mt-2" onclick="updateWaypointApi('${waypointData.id}')">Update</button>`
                : ''
            }
        </div>
    `
}
