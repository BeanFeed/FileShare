<script setup>
import NavBar from "./components/NavBar.vue";

</script>

<template>
  
  <div class="min-h-screen h-auto w-screen bg-slate-900 pt-14">
    <NavBar class="fixed top-0 left-0"/>
    <router-view/>
  </div>
  
</template>

<style scoped>

</style>
