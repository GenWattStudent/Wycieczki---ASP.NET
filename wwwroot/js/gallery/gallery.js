$(document).ready(function () {
  const dataImageBtns = $('[data-image-btn]')
  console.log(dataImageBtns)
  dataImageBtns.on('click', function () {
    const imageUrl = $(this).closest('.card').find('img').attr('src')
    console.log($('#GalleryDetailsModal'))
    $('#GalleryDetailsModal img').attr('src', imageUrl)
    $('#GalleryDetailsModal').modal('show')
  })

  const fileInput = $('#files')
  fileInput.on('change', function () {
    $('#addImageForm').submit()
  })
})
