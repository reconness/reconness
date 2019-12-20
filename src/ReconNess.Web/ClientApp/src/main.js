import Vue from 'vue'
import axios from 'axios';
import VueAxios from 'vue-axios';

import App from './App.vue'
import router from './router';
import api from './api'
import connection from './api/signalR'

import { ClientTable } from 'vue-tables-2';

import 'bootstrap'; 
import 'bootstrap/dist/css/bootstrap.min.css';
import 'xterm/css/xterm.css';

import './filters';

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
