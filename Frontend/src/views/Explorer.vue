<script setup>

import DirectoryCard from "../components/DirectoryCard.vue";
import {onMounted} from "vue";
import axios from "axios";
/*
onMounted(() => {
  let directoriesReq = axios({
    method: "GET",
    url: 
  });  
});
*/
</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <DirectoryCard dir-name="Test Folder" item-count="0"/>

    </div>
  </div>
  

</template>

<style scoped>

</style>