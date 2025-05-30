<script setup>
import {router} from "../router/index.js";
import {useRoute} from "vue-router";
import {onMounted, ref, watchEffect} from "vue";
import {onClickOutside} from "@vueuse/core";
const route = useRoute();
const props = defineProps(["dirName","itemCount","isHeld","canEdit"])
const dPath = ref();
const name = ref(props.dirName)
defineEmits(['openProperties','moveCard'])
watchEffect(() => {
  dPath.value = route.params.Directory;
})

if (name.value.length > 16) {
  name.value = name.value.slice(0, 13) + "...";
}

const open = ref(false);
const optionsButton = ref();
onMounted(() => {
  onClickOutside(optionsButton, event => {
    if (open.value === true) open.value = false;
  })
})

function nextDirectory(name) {
  
  if (route.params.Directory === undefined) route.params["Directory"] = [];
  
  let newP = [...router.currentRoute.value.params.Directory];
  newP.push(name)
  router.push("/Explorer/" + newP.join('/'))
}

</script>

<template>
<div class="md:flex items-center flex-col border-4 border-slate-950 h-[17rem] w-[12rem] rounded-2xl py-3 bg-slate-900">
  <div class="flex h-36 w-full items-center justify-center">
    <i @click="nextDirectory(dirName)" class="bi bi-folder text-9xl text-teal-400 hover:bg-white hover:bg-opacity-5 cursor-pointer px-3 py-1 rounded-lg"></i>
  </div>
  <div class="flex flex-col items-center">
    <a :title="dirName" @click="nextDirectory(dirName)" class="cursor-pointer hover:bg-white hover:bg-opacity-5 text-white hover:text-white px-3 py-1 my-1 rounded-lg">{{name}}</a>
    <p class="text-xs font-light">{{itemCount}} items</p>
    <i v-show="props.canEdit" ref="optionsButton" @click="isHeld !== false ? open = !open : open = open;" class="cursor-pointer bi bi-three-dots text-2xl hover:bg-white hover:bg-opacity-5 px-3 py-1 mt-1 rounded-lg"></i>
  </div>
  <div class="relative text-left items-center inline-flex flex-col w-32">
    <div v-if="open" class="absolute w-full mt-6 bg-slate-900 border-teal-500 border-2 rounded-lg">
      <a @click="$emit('openProperties')" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-b border-teal-500 text-center">Properties</a>
      <a @click="$emit('moveCard')"       class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-t border-teal-500 text-center">Move</a>
    </div>
  </div>
</div>
</template>

<style scoped>
.moving {
  position: absolute;
  left: 5rem;
  top: 5rem;
}
</style>