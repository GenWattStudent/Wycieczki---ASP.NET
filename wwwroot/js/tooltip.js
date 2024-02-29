function tooltip() {
  const tooltips = document.querySelectorAll('[data-toggle="tooltip"]')

  tooltips.forEach((tooltip) => {
    new bootstrap.Tooltip(tooltip)
  })
}

tooltip()

console.log('tooltip.js loaded')
