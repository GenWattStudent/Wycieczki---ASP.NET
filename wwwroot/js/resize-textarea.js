$(document).ready(function () {
  $('textarea').each(function () {
    this.style.height = this.scrollHeight + 'px'
  })
  $(document).on('input', 'textarea', function () {
    this.style.height = 'auto'
    this.style.height = this.scrollHeight + 'px'
  })
})
