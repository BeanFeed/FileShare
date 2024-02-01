<script setup>
  import {ref} from "vue";

  const props = defineProps(["fileName"])
  const name = ref(props.fileName);
  if (name.value.length > 16) {

    name.value = name.value.slice(0, 13) + "...";
  }
</script>

<template>
  <div class="md:flex items-center flex-col border-4 border-slate-950 h-[17rem] w-[12rem] rounded-2xl py-3 overflow-hidden">
    
    <div class="flex h-36 w-full items-center justify-center">
      <i class="bi bi-file-earmark text-9xl text-teal-400 hover:bg-white hover:bg-opacity-5 cursor-pointer px-3 py-1 rounded-lg"></i>
    </div>
    <div class="flex flex-col items-center h-full">
      <a :title="fileName" class="cursor-pointer hover:bg-white hover:bg-opacity-5 text-white hover:text-white px-3 py-1 my-1 rounded-lg">{{name}}</a>
      
      <i class="cursor-pointer bi bi-three-dots text-2xl hover:bg-white hover:bg-opacity-5 px-3 py-1 mt-auto rounded-lg"></i>
    </div>
  </div>
</template>

<style scoped>

</style>