<script setup>
import {router} from "../router/index.js";
import {useRoute} from "vue-router";
import {ref, watchEffect} from "vue";
const route = useRoute();
const props = defineProps(["dirName","itemCount"])
const dPath = ref();
watchEffect(() => {
  dPath.value = route.params.Directory;
})
function nextDirectory(name) {
  
  if (route.params.Directory === undefined) route.params["Directory"] = [];
  
  let newP = [...router.currentRoute.value.params.Directory];
  newP.push(name)
  router.push("/Explorer/" + newP.join('/'))
}
</script>

<template>
<div class="md:flex items-center flex-col border-4 border-slate-950 h-auto w-[12rem] rounded-2xl py-3">
  <div class="flex h-36 w-full items-center justify-center">
    <i @click="nextDirectory(dirName)" class="bi bi-folder text-9xl text-teal-400 hover:bg-white hover:bg-opacity-5 cursor-pointer px-3 py-1 rounded-lg"></i>
  </div>
  <div class="flex flex-col items-center">
    <a @click="nextDirectory(dirName)" class="cursor-pointer hover:bg-white hover:bg-opacity-5 text-white hover:text-white px-3 py-1 my-1 rounded-lg">{{dirName}}</a>
    <p class="text-xs font-light">{{itemCount}} items</p>
    <i class="cursor-pointer bi bi-three-dots text-2xl hover:bg-white hover:bg-opacity-5 px-3 py-1 mt-1 rounded-lg"></i>
  </div>
</div>
</template>

<style scoped>

</style>