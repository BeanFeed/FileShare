<script setup>

import {router} from "../router/index.js";
import {useRoute} from "vue-router";
import {onMounted, ref, watchEffect} from "vue";
import Explorer from "../views/Explorer.vue";
import {onClickOutside} from "@vueuse/core";
import {store} from "../state/state.js";

const route = useRoute();
const dPath = ref();
const pathName = ref();
const dropdown = ref(null);
const fileInput = ref(null);
watchEffect(() => {
  dPath.value = route.params.Directory;
  pathName.value = route.name;
})
//console.log(this.route.query.params)
let open = ref();
open.value = false;
let isLoading = ref(true);
onMounted(() => {
  isLoading.value = false;
  onClickOutside(dropdown, event => {
    if (open.value === true) open.value = false;
  })
})
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
  console.log(fileInput.value.click())
}


</script>



<template>
<header class="w-screen h-14 bg-slate-950 text-teal-400 shadow md:flex justify-between items-center">
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
        <li>
          <div class="container flex items-center">
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
    <div class="bg-slate-900 px-2 py-0.5 rounded-3xl">
      <ul class="md:flex items-center">
        <li class="mx-1">
          <i class="bi bi-search"></i>
        </li>
        <li class="mx-1 my-1">
          <input type="text" class="bg-slate-900 focus:outline-none">
        </li>
      </ul>
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