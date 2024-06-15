<script setup>
import {ref} from "vue";

let props = defineProps(["defName"])

let text = ref('');
</script>

<template>
<div class="absolute left-0 top-0 w-screen h-screen bg-black bg-opacity-50 flex items-center justify-center">
  <div class="bg-slate-900 rounded-lg px-3 py-3 text-center">
    <div class="my-3 px-2 py-1 bg-slate-850 rounded-2xl">
      <input v-model="text" :placeholder="defName" type="text" class="bg-slate-850 focus:outline-none">
    </div>
  </div>
</div>

</template>

<style scoped>

</style>