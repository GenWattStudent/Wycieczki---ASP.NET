// Time
const timeEl = $('#time')
const timeIcon = $('#time-icon')
const countryEl = $('#country')

function updateTime() {
  const date = new Date()
  const hours = date.getHours() < 10 ? `0${date.getHours()}` : date.getHours()
  const minutes =
    date.getMinutes() < 10 ? `0${date.getMinutes()}` : date.getMinutes()
  const seconds =
    date.getSeconds() < 10 ? `0${date.getSeconds()}` : date.getSeconds()

  if (date.getHours() >= 6 && date.getHours() < 18) {
    timeIcon.attr('name', 'sunny')
  } else {
    timeIcon.attr('name', 'moon')
  }

  countryEl.text(Intl.DateTimeFormat().resolvedOptions().timeZone)

  timeEl.text(`${hours}:${minutes}:${seconds}`)
}

setInterval(updateTime, 1000)
