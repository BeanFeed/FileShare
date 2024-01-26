<script setup>
import {router} from "../router/index.js";

//console.log(this.route.query.params)
</script>

<template>
<header class="w-screen h-14 bg-slate-950 text-teal-400 shadow md:flex justify-between items-center">
  <div class="text-sm flex items-center">
    <h1 class="mx-4">File Host</h1>
    
    <ul class="md:flex md:items-center text-lg">
      <li class="rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1">
        Root
      </li>

      <i class="text-white text-xl bi bi-chevron-right"></i>

      <li class="rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1">
        Thing
      </li>

      <i class="text-white text-xl bi bi-chevron-right"></i>

      <li class="rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1">
        Thing
      </li>

      <i class="text-white text-xl bi bi-chevron-right"></i>

      <li class="rounded-lg md:mx-4 hover:bg-white hover:bg-opacity-10 px-3 py-1">
        Thing
      </li>
    </ul>
  </div>
</header>
</template>

<style scoped>

</style>