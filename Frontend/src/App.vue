<script setup>
import NavBar from "./components/NavBar.vue";
</script>

<template>
  
  <div class="min-h-screen h-auto w-screen bg-slate-900">
    <NavBar class="fixed"/>
    <router-view class="pt-20"/>
  </div>
  
</template>

<style scoped>

</style>
