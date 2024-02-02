<script setup>
import {onMounted, ref, watchEffect} from "vue";
import {useRoute} from "vue-router";
import DirectoryCard from "../components/DirectoryCard.vue";
import Config from "../config.json";
import axios from "axios";
import FileCard from "../components/FileCard.vue";

const dPath = ref();
dPath.value = [""]
const route = useRoute();
var gettingData = ref('pending');
var directories = ref();
var files = ref();
var updateCount = ref(0);

var dCardList = ref([]);
var fCardList = ref([]);

watchEffect(async () => {
  
  dPath.value = route.params.Directory;
  await updateScreen()
  
});

onMounted(async () => {
  console.log('mounted')
  await updateScreen()
})

async function updateScreen(_props) {
  gettingData.value = "pending"
  console.log(Config.BackendUrl + "api/v1/filesystem/getfromdirectory" + GetUrlArray("path", dPath.value))
  let req = await axios({
    method: "GET",
    url: Config.BackendUrl + "api/v1/filesystem/getfromdirectory" + GetUrlArray("path", dPath.value)

  })
      .then(function (res) {
        if (res.status === 200 && res.data.success === true) {
          gettingData.value = "success";
          directories.value = res.data.message.directories;
          files.value = res.data.message.files;
          console.log(directories)
          updateCount += 1;
        }
      })
}
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

function MoveDir(id) {
  dCardList.value[id].pickup();
}

</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <div id="pickupBox" class="absolute left-0 top-0"></div>
      <template v-if="gettingData === 'success'" :key="updateCount">

        <template v-for="(directory, index) in directories" :key="index">
          <DirectoryCard @move-card="MoveDir(index)" ref="(el) => dCardList[index] = el" :dir-name="directory.name" :item-count="directory.itemCount" />
        </template>
        <template v-for="file in files">
          <FileCard :file-name="file.name" />
        </template>
      </template>

    </div>
  </div>
  

</template>

<style scoped>

</style>