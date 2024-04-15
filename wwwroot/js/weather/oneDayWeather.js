$(document).ready(function () {
  function updateData(weather) {
    $('#weather-card #temp').text(weather.temp + '°C')
    $('#weather-card #icon').attr('src', '/icons/weather/' + weather.icon + '.png')
    $('#weather-card #description').text(weather.description)
    $('#weather-card #wind').text(weather.windspeed)
    $('#weather-card #humidity').text(weather.humidity)
    $('#weather-card #pressure').text(weather.pressure)
    $('#weather-card #visibility').text(weather.visibility)
    $('#weather-card #sunset').text(weather.sunset.substring(0, weather.sunset.length - 3))
    $('#weather-card #sunrise').text(weather.sunrise.substring(0, weather.sunrise.length - 3))
    $('#weather-card').closest('.glassmorphism').removeClass('d-none')
  }

  function success(position) {
    if (window.innerWidth < 980) return

    const latitude = position.coords.latitude
    const longitude = position.coords.longitude

    $.ajax({
      url: '/Weather/GetOneDayWeather',
      type: 'GET',
      data: {
        lat: latitude,
        lon: longitude,
      },
      success: function (response) {
        updateData(response)
      },
      error: function () {
        toastr.error('Weather data not fetched.')
      },
    })
  }

  function error() {
    toastr.error('Some services are not available without location permission.')
  }

  navigator.geolocation.getCurrentPosition(success, error, { enableHighAccuracy: true })
})
