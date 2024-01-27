<script setup>
import {router} from "../router/index.js";
import {useRoute} from "vue-router";
import {ref, watchEffect} from "vue";
import Explorer from "../views/Explorer.vue";
const route = useRoute();
const dPath = ref();
const pathName = ref();
watchEffect(() => {
  dPath.value = route.params.Directory;
  pathName.value = route.name;
})
let path
//console.log(this.route.query.params)


function GoBack(amount) {
  let newP = [...dPath.value];
  for (var i = newP.length; i !== amount+1; i--){
    newP.pop();
  }
  router.push('/Explorer/' + newP.join('/'));
}

</script>

<template>
<header class="w-screen h-14 bg-slate-950 text-teal-400 shadow md:flex justify-between items-center">
  <div class="text-sm flex items-center">
    <h1 class="mx-4">File Host</h1>
    
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
        
      </template>
      <template v-else-if="pathName === 'Home'">
        <li>
          <router-link to="/Explorer" class="cursor-pointer text-teal-400 rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1 hover:text-teal-400">Go To Explorer</router-link>
        </li>
      </template>
      
      
      
      
      

      
    </ul>
  </div>
</header>
</template>

<style scoped>

</style>