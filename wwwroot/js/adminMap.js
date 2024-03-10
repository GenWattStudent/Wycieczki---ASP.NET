import { Map } from './map.js'
import { Waypoint } from './Waypoint.js'

class MapAdmin extends Map {
  constructor(toolSelector) {
    super()
    this.toolSelector = toolSelector
    this.toolSelector.selectTool('start')
    this.map.on('click', this.addWayPointClick.bind(this))
  }

  createSubmitData(waypointData) {
    console.log(waypointData)
    const { marker, ...data } = waypointData
    return {
      isRoad: marker ? false : true,
      ...data,
      id: (typeof waypointData.id === 'string'
        ? 0
        : waypointData.id
      ).toString(),
    }
  }

  updateWaypoint(id, edit = false) {
    console.log('update', id, this.waypoints)
    const waypoint = this.waypoints.find((w) => w.id == id)

    if (waypoint) {
      const title = document.getElementById('waypoint-title').value
      const description = document.getElementById('waypoint-description').value
      let images = []

      if (!edit) {
        const imagesEL = document.getElementById('waypoint-image').files
        waypoint.update(title, description, Array.from(imagesEL))
      } else {
        waypoint.update(title, description, waypoint.images)
      }

      const popup = edit
        ? this.createEditPopup(waypoint)
        : this.createWayPointPopup(title, description, waypoint.images, id)

      const imageUrls = []
      if (!edit) {
        for (let i = 0; i < images.length; i++) {
          imageUrls.push(URL.createObjectURL(images[i]))
        }
      }

      waypoint.marker.bindPopup(popup).openPopup()
      if (!edit)
        waypoint.addRemoveListener(() => this.removeWayPoint(waypoint.id))
      console.log(this.waypoints)
    }
  }

  selectWaypoint(waypointData, edit = false) {
    console.log(waypointData)
    const form = this.createWaypointForm(waypointData, edit)
    document.getElementById('waypoint-form').innerHTML = form

    if (!edit) {
      document
        .getElementById('create-marker-btn')
        .addEventListener('click', () => this.updateWaypoint(waypointData.id))
    } else {
      document
        .getElementById('edit-marker-btn')
        .addEventListener('click', () => updateWaypointApi(waypointData.id))

      document
        .getElementById('waypoint-image')
        .addEventListener('change', (e) => {
          addImages(waypointData.id, e)
        })
    }
  }

  addWayPointClick(e) {
    const waypoint = new Waypoint(e.latlng.lat, e.latlng.lng)
    waypoint.type = this.toolSelector.currentTool

    if (this.waypoints.some((w) => w.type === 'end')) {
      return toastr.error('You have endpoint in your tour!')
    }

    if (
      this.toolSelector.currentTool === 'start' &&
      this.waypoints.some((w) => w.type === 'start')
    ) {
      return toastr.error('Start point already exists!')
    }

    if (
      this.toolSelector.currentTool === 'end' &&
      !this.waypoints.some((w) => w.type === 'start')
    ) {
      return toastr.error('Start point must be added first!')
    }

    this.addWaypoint(waypoint, this.toolSelector.currentTool)
  }

  addWaypoint(waypoint, currentTool, edit = false) {
    const marker = this.createMarker(
      waypoint,
      edit,
      true,
      false,
      this.getIcon(currentTool)
    )
    waypoint.marker = marker

    if (currentTool === 'marker') {
      waypoint.isRoad = false
    } else {
      waypoint.isRoad = true
    }

    this.waypoints.push(waypoint)
    this.connectWaypointsWithLine(this.waypoints)
  }

  createWaypointForm(waypointData, edit = false) {
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
                  ? `<input class="form-control w-100" type="file" id="waypoint-image" multiple />`
                  : ''
              }
              ${
                !edit
                  ? `<button id="create-marker-btn" type="button" class="btn btn-primary mt-2">Update</button>`
                  : ''
              }
              ${
                edit === true
                  ? `<button id="edit-marker-btn" type="button" class="btn btn-primary mt-2">Update</button>`
                  : ''
              }
          </div>
      `
  }
}

class ToolSelector {
  constructor() {
    this.currentTool = 'marker'
    this.buttons = document.querySelectorAll('#toolbar button')
    this.setUpButtons()
    console.log(this.buttons)
  }

  setUpButtons() {
    this.buttons.forEach((b) => {
      b.addEventListener('click', () => {
        this.selectTool(b.dataset.tool)
      })
    })
  }

  selectTool(tool) {
    this.currentTool = tool
    this.buttons.forEach((b) => {
      if (b.dataset.tool === tool) {
        b.classList.remove('bg-secondary')
        b.classList.add('bg-primary')
      } else {
        b.classList.remove('bg-primary')
        b.classList.add('bg-secondary')
      }
    })
  }
}

const toolSelector = new ToolSelector()
export const adminMap = new MapAdmin(toolSelector)

function updateWaypointApi(id) {
  console.log(id, adminMap.waypoints)
  adminMap.updateWaypoint(id, true)
  const waypoint = adminMap.waypoints.find((waypoint) => waypoint.id == id)
  if (!waypoint) return

  const formData = new FormData()

  formData.append('id', waypoint.id)
  formData.append('lat', waypoint.lat)
  formData.append('lng', waypoint.lng)
  formData.append('name', waypoint.name)
  formData.append('description', waypoint.description)
  formData.append('isRoad', waypoint.isRoad)

  console.log(waypoint)
  fetch('/Waypoint/Edit', {
    method: 'POST',
    body: formData,
  })
    .then((res) => {
      if (res.ok) {
        toastr.success(`${waypoint.name} updated!`)
      } else {
        throw new Error('Something went wrong!')
      }
    })
    .catch((err) => {
      toastr.error(`${waypoint.name} not updated! ${err.message}`)
    })
}

function addImages(id, event) {
  const formData = new FormData()
  console.log(event.target.files, id)
  formData.append('id', id)
  const imagesArray = Array.from(event.target.files)
  imagesArray.forEach((file) => {
    formData.append('images', file)
  })

  fetch('/Waypoint/AddImages', {
    method: 'POST',
    body: formData,
  })
    .then((res) => {
      window.location.reload()
    })
    .catch((err) => {
      console.log(err)
    })
}
