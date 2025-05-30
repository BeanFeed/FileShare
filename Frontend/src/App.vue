<script setup>
import NavBar from "./components/NavBar.vue";
import {store} from "./store/state.js";

</script>

<template>
  
  <div class="min-h-screen h-auto w-screen bg-slate-900 pt-14" :style="{overflow: store.overflow}">
    <NavBar class="fixed top-0 left-0 z-40"/>
    <router-view/>
  </div>
  
</template>

<style scoped>

</style>
