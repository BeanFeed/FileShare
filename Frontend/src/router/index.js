import Home from "../views/Home.vue";
import {createRouter, createWebHistory} from "vue-router";
import Explorer from "../views/Explorer.vue";


const routes = [
    {
        path: "/",
        redirect: "/Home"
    },
    {
        path:"/Home",
        component: Home,
        name: "Home"
    },
    {
        path:"/Explorer",
        component: Explorer,
        name: "Explorer",
        children: [
            {
                path: ":Directory+",
                component: Explorer,
                name: "Explorer"
            }
        ]
    }

];

export const router = createRouter({
    history: createWebHistory(),
    routes: routes
});