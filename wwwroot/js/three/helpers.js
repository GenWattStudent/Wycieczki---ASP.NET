import * as THREE from 'three'

const DEGREE_TO_RADIAN = Math.PI / 180
const GLOBE_RADIUS = 4

export function latLongToVector3(lat, lng, radius, heigth) {
  const phi = (90 - lat) * DEGREE_TO_RADIAN
  const theta = (lng + 180) * DEGREE_TO_RADIAN

  return new THREE.Vector3(
    -radius * Math.sin(phi) * Math.cos(theta),
    radius * Math.cos(phi),
    radius * Math.sin(phi) * Math.sin(theta)
  )
}

export function serArc3D(
  pointStart,
  pointEnd,
  smoothness,
  clockWise,
  earthRadius = 4.1
) {
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
