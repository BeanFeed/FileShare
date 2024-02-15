<script setup>
import {onMounted, ref} from "vue";
import {onClickOutside} from "@vueuse/core";

const props = defineProps(["fileName","isHeld"])
const name = ref(props.fileName);
defineEmits(['moveCard', 'renameCard','deleteCard','downloadCard'])
const optionsButton = ref(); 
if (name.value.length > 16) {
  name.value = name.value.slice(0, 13) + "...";
}
const open = ref(false);
onMounted(() => {
  onClickOutside(optionsButton, event => {
    if (open.value === true) open.value = false;
  })
})
</script>

<template>
  <div class="md:flex items-center flex-col border-4 border-slate-950 h-[17rem] w-[12rem] rounded-2xl py-3 ">
    
    <div class="flex h-36 w-full items-center justify-center">
      <i class="bi bi-file-earmark text-9xl text-teal-400 hover:bg-white hover:bg-opacity-5 cursor-pointer px-3 py-1 rounded-lg"></i>
    </div>
    <div class="flex flex-col items-center h-full">
      <a :title="fileName" class="cursor-pointer hover:bg-white hover:bg-opacity-5 text-white hover:text-white px-3 py-1 my-1 rounded-lg">{{name}}</a>
      
      <i ref="optionsButton" @click="isHeld !== false ? open = !open : open = open;" class="cursor-pointer bi bi-three-dots text-2xl hover:bg-white hover:bg-opacity-5 px-3 py-1 mt-auto rounded-lg"></i>
    </div>
    <div class="relative text-left items-center inline-flex flex-col w-32">
      <div v-if="open" class="absolute w-full mt-6 bg-slate-900 border-teal-500 border-2 rounded-lg">
        <a @click="$emit('downloadCard')" class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-b border-teal-500 text-center">Download</a>
        <a @click="$emit('moveCard')"     class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-y border-teal-500 text-center">Move</a>
        <a @click="$emit('renameCard')"   class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-y border-teal-500 text-center">Rename</a>
        <a @click="$emit('deleteCard')"   class="cursor-pointer text-teal-400 hover:text-teal-400 hover:bg-white hover:bg-opacity-10 block border-t border-teal-500 text-center">Delete</a>
      </div>
    </div>
    
  </div>
</template>

<style scoped>

</style>