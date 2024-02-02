<script setup>
import {onMounted, ref, watch, watchEffect} from "vue";
import {useRoute} from "vue-router";
import DirectoryCard from "../components/DirectoryCard.vue";
import Config from "../config.json";
import axios from "axios";
import FileCard from "../components/FileCard.vue";
import {store} from "../state/state.js";

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
  //console.log("change")
  await updateScreen()
  
});
watch(store,()=>{
  if (store.dropFired) {
    store.dropFired = false;
    DropCard();
  }
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

let held = ref({
  id: -1,
  name: "",
  itemCount: 0,
  path: [""],
  show: false
});

function MoveDir(data, id) {
    held.value.id = id;
    held.value.name = data.name;
    held.value.itemCount = data.itemCount;
    held.value.path = dPath.value;
    held.value.show = true;
    store.cardHeld = true;
}

function DropCard() {
  held.value = {
    id: -1,
    name: "",
    itemCount: 0,
    path: [""],
    show: false
  };
}
</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div id="mainBox"  class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <template v-if="gettingData === 'success'" :key="updateCount">

        <template v-for="(directory, index) in directories" :key="index">
          <DirectoryCard v-if="index !== held.id || dPath !== held.path"  :id="'dCard'+index" @move-card="MoveDir(directory, index)" :dir-name="directory.name" :item-count="directory.itemCount" />
          <DirectoryCard v-else :id="'dCard'+index" @move-card="MoveDir(directory, index)" :dir-name="directory.name" :item-count="directory.itemCount" v-show="false"/>
        </template>
        <template v-for="file in files">
          <FileCard :file-name="file.name" />
        </template>
      </template>

    </div>
    <div id="pickupBox" class="absolute left-3 top-16">
      <DirectoryCard v-if="held.show" :id="'dCard' + held.id" :dir-name="held.name" :item-count="held.itemCount" />
    </div>
  </div>
  

</template>

<style scoped>

</style>