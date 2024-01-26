import Home from "../views/Home.vue";
import {createRouter, createWebHistory} from "vue-router";


const routes = [
    {
        path: "/",
        redirect: "/Home"
    },
    {
        path:"/Home",
        component: Home
    },
    {
        path:"/Explorer/:Directory?",
        component: Home
    }

];

export const router = createRouter({
    history: createWebHistory(),
    routes: routes
});