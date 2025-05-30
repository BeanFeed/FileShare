<script setup>

import {router} from "../router/index.js";
import {useRoute} from "vue-router";
import {onMounted, ref, watch, watchEffect} from "vue";
import Explorer from "../views/Explorer.vue";
import {onClickOutside} from "@vueuse/core";
import {store, userInfoStore} from "../store/state.js";
import axios from "axios";
import {BackendUrl, CanRegister} from "../config.json";

const route = useRoute();
const dPath = ref();
const pathName = ref();
const dropdown = ref(null);
const upload = ref(null);
const fileInput = ref(null);
let userStore = userInfoStore();

watchEffect(() => {
  dPath.value = route.params.Directory;
  pathName.value = route.name;
})
const loginMenu = ref();
loginMenu.value = true;
watch(store, () => {
  loginMenu.value = userStore.user == null;

});
let open = ref();
open.value = false;
let loginOpen = ref(null);
let profileButton = ref();
loginOpen.value = false;
let isLoading = ref(true);
onMounted(() => {
  
  console.log(route);
  
  isLoading.value = false;
  onClickOutside(dropdown, event => {
    if (open.value === true) open.value = false;
  });

  onClickOutside(profileButton, event => {
    loginOpen.value = false;
  });
  
  console.log("Before: " + JSON.stringify(userStore.user))
  var userReq = axios.get(BackendUrl + "v1/user/me", {withCredentials: true}).then((res) => {
    userStore.user = res.data.message;
    console.log("After: " + JSON.stringify(userStore.user))
  }).catch(() =>{
    userStore.user = null;
  });
})

function GetUser() {
  
}

function GoBack(amount) {
  let newP = [...dPath.value];
  for (var i = newP.length; i !== amount+1; i--){
    newP.pop();
  }
  router.push('/Explorer/' + newP.join('/'));
}

function SubmitFile(event) {
  store.uploadedFile = event.target.files[0];
}

function OpenFilePrompt() {
  fileInput.value.click()
}

function OpenLogin() {
  router.push("/Login?returnUrl=" + encodeURI(route.fullPath));
}

function OpenSignup() {
  router.push("/Register?returnUrl=" + encodeURI(route.fullPath));
}

function Signout() {
  var req = axios.get(BackendUrl + "v1/user/signout", {withCredentials: true}).then(() => {
    userStore.user = null;
    location.reload();
  });
  
}
</script>



<template>
<header>
  <div style="box-shadow: 0 4px 7px 3px black;" class="w-screen h-14 bg-slate-950 text-teal-400 shadow md:flex justify-between items-center">
    <div class="text-sm flex items-center">
      <h1 class="mx-4 no-highlight">File Host</h1>
      <ul class="md:flex md:items-center text-lg">
        <li>
          <router-link to="/Home" class="text-teal-400 rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1 hover:text-teal-400">Home</router-link>
        </li>
        <template v-if="pathName === 'Explorer'">
          <i class="text-white text-xl bi bi-chevron-right"></i>
          <li>
            <a @click="router.push('/Explorer')" class="cursor-pointer text-teal-400 rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1 hover:text-teal-400">Explorer</a>
          </li>
          <template v-for="(path, index) in dPath" :key="path">
            <i  class="text-white text-xl bi bi-chevron-right"></i>
            <li>
              <a class="cursor-pointer text-teal-400 rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1 hover:text-teal-400" @click="GoBack(index)">{{path}}</a>
            </li>
          </template>
          <li v-if="store.canEdit">
            <div class="flex items-center">
              <div class="relative text-left inline-flex flex-col w-32">
                <a ref="dropdown" @click="open = !open" class="cursor-pointer bg-teal-500 text-slate-900 text-3xl rounded-lg items-center justify-center w-7 h-7 flex hover:text-slate-900 hover:bg-teal-600">
                  <i class="bi bi-plus"></i>
                </a>

                <div v-if="open" class="absolute left-0 right-0 w-full mt-9 bg-slate-950 border-teal-500 border-2 rounded-lg">
                  <a @click="OpenFilePrompt()" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-b border-teal-500 text-center">Upload File</a>
                  <a class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-t border-teal-500 text-center">New Folder</a>
                </div>
              </div>
            </div>
          </li>

        </template>
        <template v-else-if="pathName === 'Home'">
          <li>
            <router-link to="/Explorer" class="cursor-pointer text-teal-400 rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1 hover:text-teal-400">Go To Explorer</router-link>
          </li>
        </template>
      </ul>
      <input ref="fileInput" @change="e => SubmitFile(e)" type="file" style="display: none">
    </div>

    <div class="float-right mx-4 md:flex items-center">
      <a v-if="store.cardHeld" @click="store.cardHeld = false; store.dropFired = true;" class="mx-4 bg-teal-500 hover:bg-teal-600 px-2 text-slate-900 py-0.5 rounded-lg hover:text-slate-950 cursor-pointer">Drop Card</a>
      <div v-show="false" class="bg-slate-900 px-2 py-0.5 rounded-3xl">
        <ul class="md:flex items-center">
          <li class="mx-1">
            <i class="bi bi-search"></i>
          </li>
          <li class="mx-1 my-1">
            <input type="text" class="bg-slate-900 focus:outline-none">
          </li>
        </ul>
      </div>
      <p v-if="userStore.user !== null">Hello, {{userStore.user.username}}</p>
      <div class="ml-4 relative text-left inline-flex flex-col">
        <i ref="profileButton" class="text-3xl bi bi-person-circle cursor-pointer" @click="loginOpen = !loginOpen"/>
        <div v-if="loginOpen" class="absolute right-0 w-32 mt-9 bg-slate-950 border-teal-500 border-2 rounded-lg">
          <template v-if="userStore.user == null">
            <a @click="OpenLogin()" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-b border-teal-500 text-center">Login</a>
            <a @click="OpenSignup()" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-t border-teal-500 text-center" v-show="CanRegister">Signup</a>
          </template>
          <template v-else>
            <a @click="Signout()" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-teal-500 text-center">Signout</a>
          </template>
        </div>
      </div>
    </div>
    
    
  </div>
</header>

</template>

<style scoped>

.no-highlight{
  user-select: none;
  -moz-user-select: none;
  -webkit-text-select: none;
  -webkit-user-select: none;
}

</style>