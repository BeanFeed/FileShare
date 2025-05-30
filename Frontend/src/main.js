import {createApp, VueElement} from 'vue'
import {createPinia} from "pinia";
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import 'bootstrap-icons/font/bootstrap-icons.css'
import './style.css'
import App from './App.vue'
import {router} from "./router/index.js";

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate)

createApp(App).use(router).use(pinia).mount('#app')
