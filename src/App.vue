<script setup>
import NavBar from "./components/NavBar.vue";
</script>

<template>
  
  <div class="h-screen w-screen bg-slate-900">
    <NavBar/>
    <router-view/>
  </div>
  
</template>

<style scoped>

</style>
