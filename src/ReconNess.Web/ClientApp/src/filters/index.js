import Vue from 'vue';
import moment from 'moment'

Vue.filter('formatDate', function(value, format) {
  if (value) {
    return value === '0001-01-01T00:00:00' ? 'never' : moment(value).format(format)
  }
})

Vue.filter('joinComma', function (value, property) {
  return value.map((item) => item[property]).join(', ')
});