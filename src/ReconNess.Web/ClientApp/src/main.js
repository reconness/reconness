import Vue from 'vue'

import VueAxios from 'vue-axios';
import axios from 'axios';

import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'xterm/css/xterm.css';

import './filters';
import App from './App.vue'
import store from './store'
import router from './router';
import connection from './api/signalR'

import { ClientTable } from 'vue-tables-2';

import { library } from '@fortawesome/fontawesome-svg-core'
import { faArrowAltCircleRight, faTrashAlt, faExclamation,
    faBug, faCoffee, faGuitar, faHome, faHeart, faBookOpen,
    faFireAlt, faDollarSign, faMicroscope, faMinusCircle, faPlusCircle
} from '@fortawesome/free-solid-svg-icons'

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'                                     

library.add(faArrowAltCircleRight, faTrashAlt, faExclamation, faBug,
    faCoffee, faGuitar, faHome, faHeart, faBookOpen, faFireAlt,
    faDollarSign, faMicroscope, faMinusCircle, faPlusCircle)

Vue.component('font-awesome-icon', FontAwesomeIcon)

Vue.use(VueAxios, axios);

Vue.use(ClientTable);

Vue.config.productionTip = false

Vue.$connection = connection
Object.defineProperty(Vue.prototype, '$connection', {
  get () {
    return connection.start()
  }
})

new Vue({
    router,
    store,
    render: h => h(App),
}).$mount('#app')
