import Home from "../views/Home.vue";
import {createRouter, createWebHistory} from "vue-router";
import Explorer from "../views/Explorer.vue";
import Login from "../views/Login.vue";
import Register from "../views/Register.vue";
import { CanRegister } from "../config.json"
import Account from "../views/Settings/Account.vue";
import Settings from "../views/Settings/Settings.vue";

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
    },
    {
        path: "/Login",
        component: Login,
        name: "Login"
    },
    {
        path:"/Settings",
        component: Settings,
        name: "Settings",
        redirect: "/Settings/Account",
        children: [
            {
                path:"Account",
                component: Account
            }
        ]
    }
];
if(CanRegister === true) {
    routes[routes.length] = {
        path: "/Register",
        component: Register,
        name: "Register"
    };
} else {
    routes[routes.length] = {
        path: "/Register",
        redirect: to => {
            return { name: "Home" }
        }
    };
}

export const router = createRouter({
    history: createWebHistory(),
    routes: routes
});