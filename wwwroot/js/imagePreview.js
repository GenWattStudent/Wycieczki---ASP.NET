function ImgPre(input) {
  if (input.files && input.files[0]) {
    const fileArray = Array.from(input.files)
    document.getElementById('preview').innerHTML = ''
    fileArray.forEach((file, index) => {
      const reader = new FileReader()
      reader.onload = function (e) {
        const imgBox = document.createElement('div')
        const img = document.createElement('img')

        img.src = e.target.result
        img.style.width = '100px'
        img.style.height = '100px'
        img.style.margin = '5px'
        img.style.border = '1px solid #000'
        img.style.borderRadius = '5px'
        img.style.padding = '5px'
        img.style.boxShadow = '0 0 5px 0 #000'

        imgBox.appendChild(img)
        document.getElementById('preview').appendChild(imgBox)
      }
      reader.readAsDataURL(file)
    })
  }
}
