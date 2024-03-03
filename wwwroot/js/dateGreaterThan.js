$(document).ready(function () {
  $.validator.addMethod(
    'dategreaterthan',
    function (value, element, parameters) {
      var startDate = $(element).data('val-dategreaterthan-propertyname')
      var endDate = $(element).data('val-dategreaterthan-target')
      return (
        Date.parse($(`#${endDate}`).val()) >
        Date.parse($(`#${startDate}`).val())
      )
    }
  )

  $.validator.unobtrusive.adapters.addBool('dategreaterthan')
})
