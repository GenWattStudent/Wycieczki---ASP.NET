document.querySelector('form').addEventListener('submit', function (e) {
  e.preventDefault()

  const formData = new FormData()
  formData.append('Name', document.querySelector('input[name="Name"]').value)
  formData.append(
    'Description',
    document.querySelector('input[name="Description"]').value
  )
  formData.append('Price', document.querySelector('input[name="Price"]').value)
  formData.append(
    'MaxUsers',
    document.querySelector('input[name="MaxUsers"]').value
  )
  formData.append(
    'StartDate',
    document.querySelector('input[name="StartDate"]').value
  )
  formData.append(
    'EndDate',
    document.querySelector('input[name="EndDate"]').value
  )

  const data = waypoints.map(createSubmitData)
  console.log(data.images)

  for (let i = 0; i < data.length; i++) {
    for (let j = 0; j < data[i].images.length; j++) {
      formData.append(
        `Waypoints[${i}].Images`,
        data[i].images[j],
        'image-name.jpg'
      )
    }

    formData.append(`Waypoints[${i}][name]`, data[i].name)
    formData.append(`Waypoints[${i}][description]`, data[i].description)
    formData.append(`Waypoints[${i}][lat]`, data[i].lat)
    formData.append(`Waypoints[${i}][lng]`, data[i].lng.toString())
    formData.append(`Waypoints[${i}][isRoad]`, data[i].isRoad)
    formData.append(`Waypoints[${i}][id]`, data[i].id)
  }

  // Add the files
  for (let i = 0; i < imageFiles.length; i++) {
    formData.append(`images`, imageFiles[i].file)
  }
  // Send the form data to the server
  fetch('/Tour/AddTour', {
    method: 'POST',
    body: formData,
  })
    .then(() => (window.location.href = '/Tour/Tours'))
    .catch((error) => {
      // Handle the error
    })
})
