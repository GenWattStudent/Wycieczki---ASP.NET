const imageFiles = []

function ImgPre(input) {
  if (input.files && input.files[0]) {
    const fileArray = Array.from(input.files)

    fileArray.forEach((file, index) => {
      const reader = new FileReader()
      reader.onload = function (e) {
        const imgBox = document.createElement('div')
        imgBox.dataset.index = index
        imgBox.className = 'position-relative'
        const img = document.createElement('img')
        const removeBtn = document.createElement('button')

        removeBtn.textContent = 'X'
        removeBtn.type = 'button'
        removeBtn.className = 'btn-sm btn-danger position-absolute'
        removeBtn.style.right = '10px'
        removeBtn.onclick = function () {
          removeImage(imgBox)
        }

        img.src = e.target.result
        img.classList.add('small-image-preview')

        imgBox.appendChild(img)
        imgBox.appendChild(removeBtn)
        document.getElementById('preview').appendChild(imgBox)
        imageFiles.push({ file, element: imgBox, index })
      }
      reader.readAsDataURL(file)
    })

    input.value = ''
  }
}

function removeImage(element) {
  const index = element.dataset.index

  imageFiles.splice(index, 1)
  element.remove()
  imageFiles.forEach((image, index) => {
    image.element.dataset.index = index
  })
}
