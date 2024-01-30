<script setup>

import DirectoryCard from "../components/DirectoryCard.vue";
</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <DirectoryCard dir-name="Test1" item-count="10"/>
      <DirectoryCard dir-name="Test2" item-count="10"/>
      <DirectoryCard dir-name="Test3" item-count="10"/>
      <DirectoryCard dir-name="Test4" item-count="10"/>
      <DirectoryCard dir-name="Test5" item-count="10"/>
      <DirectoryCard dir-name="Test6" item-count="10"/>
      <DirectoryCard dir-name="Test7" item-count="10"/>


    </div>
  </div>
  

</template>

<style scoped>

</style>