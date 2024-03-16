import * as THREE from 'three'

export function latLongToVector3(lat, lon, radius, heigth) {
  var phi = (lat * Math.PI) / 180
  var theta = ((lon - 180) * Math.PI) / 180

  var x = -(radius + heigth) * Math.cos(phi) * Math.cos(theta)
  var y = (radius + heigth) * Math.sin(phi)
  var z = (radius + heigth) * Math.cos(phi) * Math.sin(theta)

  return new THREE.Vector3(x, y, z)
}

export function serArc3D(pointStart, pointEnd, smoothness, clockWise) {
  const cb = new THREE.Vector3()
  const ab = new THREE.Vector3()
  const normal = new THREE.Vector3()

  cb.subVectors(new THREE.Vector3(), pointEnd)
  ab.subVectors(pointStart, pointEnd)
  cb.cross(ab)

  normal.copy(cb).normalize()

  const angle = pointStart.angleTo(pointEnd)

  if (clockWise) angle = Math.PI * 2 - angleValue

  const angleDelta = angle / (smoothness - 1)

  let positions = []
  for (let i = 0; i < smoothness; i++) {
    let vertex = pointStart.clone().applyAxisAngle(normal, angleDelta * i)
    positions.push(vertex.x, vertex.y, vertex.z)
  }

  return positions
}
