import Vue from 'vue'
import axios from 'axios';
import VueAxios from 'vue-axios';

import App from './App.vue'
import router from './router';
import api from './api'
import connection from './api/signalR'

import { ClientTable } from 'vue-tables-2';

import { library } from '@fortawesome/fontawesome-svg-core'
import { faArrowAltCircleRight, faTrashAlt, faExclamation, faBug, faCoffee, faGuitar, faHome, faHeart, faRunning, faFireAlt } from '@fortawesome/free-solid-svg-icons'
//import { faArrowAltCircleRight } from '@fortawesome/free-regular-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import 'bootstrap'; 
import 'bootstrap/dist/css/bootstrap.min.css';
import 'xterm/css/xterm.css';

import './filters';                                     

library.add(faArrowAltCircleRight, faTrashAlt, faExclamation, faBug, faCoffee, faGuitar, faHome, faHeart, faRunning, faFireAlt)

Vue.component('font-awesome-icon', FontAwesomeIcon)

Vue.use(VueAxios, axios);

Vue.use(ClientTable);

Vue.config.productionTip = false

Vue.$api = api

Object.defineProperty(Vue.prototype, '$api', {
  get () {
    return api
  }
})

Vue.$connection = connection
Object.defineProperty(Vue.prototype, '$connection', {
  get () {
    return connection.start()
  }
})

new Vue({
  router,
  render: h => h(App),
}).$mount('#app')
