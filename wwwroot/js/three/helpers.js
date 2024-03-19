import * as THREE from 'three'
import { geoInterpolate } from 'https://cdn.jsdelivr.net/npm/d3-geo@3/+esm'

const DEGREE_TO_RADIAN = Math.PI / 180
const CURVE_MIN_ALTITUDE = 0.05
const CURVE_MAX_ALTITUDE = 0.2

export function latLongToVector3(lat, lng, radius, heigth) {
  const phi = (90 - lat) * DEGREE_TO_RADIAN
  const theta = (lng + 180) * DEGREE_TO_RADIAN

  return new THREE.Vector3(
    -radius * Math.sin(phi) * Math.cos(theta),
    radius * Math.cos(phi),
    radius * Math.sin(phi) * Math.sin(theta)
  )
}

export function clamp(num, min, max) {
  return num <= min ? min : num >= max ? max : num
}

export function serArc3DLatLong(pointStart, pointEnd, earthRadius) {
  // start and end points
  const start = latLongToVector3(pointStart.lat, pointStart.lng, earthRadius)
  const end = latLongToVector3(pointEnd.lat, pointEnd.lng, earthRadius)

  // altitude
  const altitude = clamp(start.distanceTo(end) * 0.75, CURVE_MIN_ALTITUDE, CURVE_MAX_ALTITUDE)

  // 2 control points
  const interpolate = geoInterpolate([pointStart.lng, pointStart.lat], [pointEnd.lng, pointEnd.lat])
  const midCoord1 = interpolate(0.25)
  const midCoord2 = interpolate(0.75)
  const mid1 = latLongToVector3(midCoord1[1], midCoord1[0], earthRadius + altitude)
  const mid2 = latLongToVector3(midCoord2[1], midCoord2[0], earthRadius + altitude)

  return {
    start,
    end,
    spline: new THREE.CubicBezierCurve3(start, mid1, mid2, end),
  }
}

export function serArc3D(pointStart, pointEnd, smoothness, clockWise, earthRadius = 4.1) {
  const cb = new THREE.Vector3()
  const ab = new THREE.Vector3()
  const normal = new THREE.Vector3()

  cb.subVectors(new THREE.Vector3(), pointEnd)
  ab.subVectors(pointStart, pointEnd)
  cb.cross(ab)

  normal.copy(cb).normalize()

  let angle = pointStart.angleTo(pointEnd)

  if (clockWise) angle = Math.PI * 2 - angle

  const angleDelta = angle / (smoothness - 1)

  let positions = []
  for (let i = 0; i < smoothness; i++) {
    // Calculate the vertex position on the sphere's surface
    let vertex = pointStart.clone().applyAxisAngle(normal, angleDelta * i)

    // Project the vertex onto the sphere's surface
    const direction = vertex.clone().normalize()
    vertex = direction.multiplyScalar(earthRadius)

    positions.push(vertex.x, vertex.y, vertex.z)
  }

  return positions
}
