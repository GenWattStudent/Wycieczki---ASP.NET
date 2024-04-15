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
      id: (typeof waypointData.id === 'string' ? 0 : waypointData.id).toString(),
    }
  }

  updateWaypoint(id, edit = false) {
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
      if (!edit) waypoint.addRemoveListener(() => this.removeWayPoint(waypoint.id))
      console.log(this.waypoints)
    }
  }

  selectWaypoint(waypointData, edit = false) {
    console.log(waypointData)

    // document.getElementById('waypoint-form').innerHTML = form

    // if (!edit) {
    //   document.getElementById('create-marker-btn').addEventListener('click', () => this.updateWaypoint(waypointData.id))
    // } else {
    //   document.getElementById('edit-marker-btn').addEventListener('click', () => updateWaypointApi(waypointData.id))

    //   document.getElementById('waypoint-image').addEventListener('change', (e) => {
    //     addImages(waypointData.id, e)
    //   })
    // }
  }

  addWayPointClick(e) {
    const waypoint = new Waypoint(e.latlng.lat, e.latlng.lng)
    waypoint.type = this.toolSelector.currentTool

    if (this.waypoints.some((w) => w.type === 'end')) {
      return toastr.error('You have endpoint in your tour!')
    }

    if (this.toolSelector.currentTool === 'start' && this.waypoints.some((w) => w.type === 'start')) {
      return toastr.error('Start point already exists!')
    }

    if (this.toolSelector.currentTool === 'end' && !this.waypoints.some((w) => w.type === 'start')) {
      return toastr.error('Start point must be added first!')
    }

    this.addWaypoint(waypoint, this.toolSelector.currentTool)
  }

  addWaypoint(waypoint, currentTool, edit = false) {
    const marker = this.createMarker(waypoint, edit, true, false, this.getIcon(currentTool))
    waypoint.marker = marker

    if (currentTool === 'marker') {
      waypoint.isRoad = false
    } else {
      waypoint.isRoad = true
    }

    this.waypoints.push(waypoint)
    this.connectWaypointsWithLine(this.waypoints)
    document.dispatchEvent(new CustomEvent('waypointAdded', { detail: { waypoint } }))
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
