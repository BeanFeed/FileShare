<script setup>

import DirectoryCard from "../components/DirectoryCard.vue";
import { BackendUrl } from "../config.json";
import {onBeforeMount, onMounted, ref, watchEffect} from "vue";
import axios from "axios";
import {useRoute} from "vue-router";

const dPath = ref();
const route = useRoute();
var gettingData = ref('pending');
var directories = ref();
var files = ref();
var dirTag = ref();
watchEffect(async () => {
  
  dPath.value = route.params.Directory;
  let req = await axios.get(BackendUrl + "api/v1/filesystem/getfromdirectory" + GetUrlArray("path", dPath.value))
      .then(function (res) {
        if (res.status === 200 && res.data.success === true) {
          gettingData.value = "success";
          directories = res.data.message.directories;
          files = res.data.message.files;
          console.log(directories)
        }
      })
  
});

onMounted( async function updateScreen() {
  let req = await axios.get(BackendUrl + "api/v1/filesystem/getfromdirectory" + GetUrlArray("path", dPath.value))
      .then(function (res) {
        if (res.status === 200 && res.data.success === true) {
          gettingData.value = "success";
          directories = res.data.message.directories;
          files = res.data.message.files;
          console.log(directories)
        }
      })
})


function GetUrlArray(varname, arr) {
  var url = "?" + varname
  if (arr === undefined) arr = [""]
  if(arr[0] == null) arr[0] = ""
  
  url += "=" + arr[0];
  
  for (var i = 1; i < arr.length; i++) {
    url += "&" + varname + "=" + arr[i];
  
  }
  return url;
}
</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <template v-if="gettingData == 'success'">

        <template ref="dirTag" v-for="directory in directories">
          <DirectoryCard :dir-name="directory.name" :item-count="directory.itemCount" />
        </template>
      </template>

    </div>
    
    
  </div>
  

</template>

<style scoped>

</style>