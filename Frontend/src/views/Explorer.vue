<script setup>
import {onMounted, ref, watch, watchEffect} from "vue";
import {useRoute} from "vue-router";
import DirectoryCard from "../components/DirectoryCard.vue";
import Config from "../config.json";
import axios from "axios";
import FileCard from "../components/FileCard.vue";
import {store} from "../state/state.js";
import TextPrompt from "../components/TextPrompt.vue";
import CardProperties from "../components/CardProperties.vue";
const dPath = ref();
dPath.value = [""]
const route = useRoute();
var gettingData = ref('pending');
var directories = ref();
var files = ref();
var updateCount = ref(0);
var renaming = ref(false);
var dCardList = ref([]);
var fCardList = ref([]);
var fileInputName = ref('');
let cardPropertyEdit = ref(null);
watchEffect(async () => {
  
  dPath.value = route.params.Directory === undefined ? [] : route.params.Directory;
  //console.log("change")
  await updateScreen();
  
});
watch(store,async ()=>{
  if (store.dropFired) {
    store.dropFired = false;
    await DropCard();
  } else if (store.uploadedFile !== null) fileInputName.value = store.uploadedFile.name;
});

onMounted(async () => {
  console.log('mounted')
  
  await updateScreen()
})

async function updateScreen() {
  gettingData.value = "pending"
  console.log(Config.BackendUrl + "v1/filesystem/getfromdirectory" + GetUrlArray("pathArr", dPath.value))
  let req = await axios({
    method: "GET",
    url: Config.BackendUrl + "v1/filesystem/getfromdirectory" + GetUrlArray("pathArr", dPath.value),
    withCredentials: true

  })
      .then(async function (res) {
        if (res.status === 200 && res.data.success === true) {
          gettingData.value = "success";
          directories.value = res.data.message.directories;
          files.value = res.data.message.files;
          store.canEdit = res.data.message.canEdit
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
  show: false,
  type: ""
});

function MoveDir(data, id) {
  held.value.id = id;
  held.value.name = data.name;
  held.value.itemCount = data.itemCount;
  held.value.path = [...dPath.value];
  held.value.show = true;
  held.value.type = "directory";
  store.cardHeld = true;
}

let toBeRenamed = ref(-1);
let toBeRenamedType = ref('dir');
function MoveFile(data, id) {
  console.log(dPath.value)
  
  held.value.id = id;
  held.value.name = data.name;
  held.value.path = [...dPath.value];
  held.value.show = true;
  held.value.type = "file";
  store.cardHeld = true;
  console.log(held.value.path)
}

async function DropCard() {
  
  if (held.value.path !== dPath.value) {
    let data = {
      newPath : [...dPath.value]
    };
    if (held.value.type === 'directory') {
      if (held.value.path[0] === "") {
        held.value.path = [held.value.name];
      } else {
        held.value.path[held.value.path.length] = held.value.name;
      }
      data.oldPath = held.value.path;
      data.newPath[data.newPath.length] = held.value.name;
    } else {
      data.oldPath = held.value.path;
      data.itemName = held.value.name;
      data.overwrite = false;
    }
    
    
    let req = await axios.post(Config.BackendUrl + "v1/filesystem/moveitem",data, {
      headers: {'Content-Type': 'application/json'},
      withCredentials: true
    }).then(async function (res) {
      if (res.status === 200 && res.data.success === true) {

        console.log(res.data.message)
        await updateScreen();
      }
    })
  }
  

  held.value = {
    id: -1,
    name: "",
    itemCount: 0,
    path: [""],
    show: false
  };
}

async function RenameItem(newName) {
  let oldPath = [...dPath.value];
  if (oldPath[0] === ""){
    oldPath = [toBeRenamed.value];
  } else {
    oldPath[oldPath.length] = toBeRenamed.value;
  }
  //console.log(oldPath)
  let data = {
    itemPath: oldPath,
    newName: newName
  }
  let req = await axios.post(Config.BackendUrl + "v1/filesystem/renameitem",data, {
    headers: {'Content-Type': 'application/json'},
    withCredentials: true
  }).then(async function (res) {
    if (res.status === 200 && res.data.success === true) {

      console.log(res.data.message)
      await updateScreen();
    }
  })
  toBeRenamed.value = -1;
  renaming.value = false;
}

async function DeleteItem(name) {
  let path = [...dPath.value];
  if(path[0] === "") path = [name];
  else path[path.length] = name;
  let req = await axios({
    url:Config.BackendUrl + "v1/filesystem/deleteitem",
    data: path,
    method:"DELETE",
    withCredentials: true
  }).then(async function (res) {
    if (res.status === 200 && res.data.success === true) {

      console.log(res.data.message)
      await updateScreen();
    }
  })
}

let uploadProgress = ref(0);
let uploading = ref(false);
function ProgressUpdate(data) {
  console.log(data);
  uploadProgress.value = data.progress;
}
async function UploadItem(name) {
  let path = [...dPath.value];
  let formData = new FormData();
  let file = name !== null ? renameFile(store.uploadedFile, name) : store.uploadedFile;
  formData.append("File", file);
  formData.append("Path", path);
  console.log(name)
  uploading.value = true;
  let req = await axios.post(Config.BackendUrl + "v1/filesystem/uploadfile", formData, {
    headers: {
      'Content-Type' : 'multipart/form-data'
    },
    onUploadProgress: progressEvent => ProgressUpdate(progressEvent),
    withCredentials: true
  }).then(async function (res) {
    console.log(res.data.message);
    store.uploadedFile = null;
    uploadProgress.value = 0;
    uploading.value = false;
    await updateScreen();
  });

}

async function DownloadItem(name) {
  let path = [...dPath.value];
  path[path.length] = name;
  let req = await axios.get(Config.BackendUrl + "v1/filesystem/downloadfile" + GetUrlArray("pathArr" ,path) + "&raw=false", {withCredentials: true})
      .then(function (res) {
        console.log(res);
        ForceFileDownload(res, name);
      })
}

function renameFile(originalFile, newName) {
  return new File([originalFile], newName, {
    type: originalFile.type,
    lastModified: originalFile.lastModified,
  });
}

function ForceFileDownload(response, title) {
  const url = window.URL.createObjectURL(new Blob([response.data]))
  const link = document.createElement('a')
  link.href = url
  link.setAttribute('download', title)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link);
}
</script>

<template>
  <div class="flex justify-center px-5 pt-6">
    <div id="mainBox"  class="mx-auto grid place-items-center grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 2xl:grid-cols-6 2.5xl:grid-cols-7 gap-4">
      <template v-if="gettingData === 'success'" :key="updateCount">

        <template v-for="(directory, index) in directories" :key="index">
          <DirectoryCard v-if="index !== held.id || dPath !== held.path"  :id="'dCard'+index" @delete-card="DeleteItem(directory.name)" @rename-card="toBeRenamed = directory.name; renaming = true;" @move-card="MoveDir(directory, index)" :dir-name="directory.name" :item-count="directory.itemCount" />
          <DirectoryCard v-else :id="'dCard'+index" :dir-name="directory.name" :item-count="directory.itemCount" v-show="false"/>
        </template>
        <template v-for="(file, index) in files">
          <FileCard v-if="index !== held.id || dPath !== held.path" :id="'fCard'+index" @download-card="DownloadItem(file.name)" @delete-card="DeleteItem(file.name)" @rename-card="toBeRenamed = file.name; renaming = true;" @move-card="MoveFile(file, index)"  :file-name="file.name" />
          <FileCard v-else :id="'fCard'+index" :file-name="file.name" />
        </template>
      </template>

    </div>
    <div id="pickupBox" class="absolute left-3 top-16">
      <DirectoryCard v-if="held.type === 'directory' && held.show" :id="'dCard' + held.id" :dir-name="held.name" :item-count="held.itemCount" :is-held="true"/>
      <FileCard v-if="held.type === 'file' && held.show" :id="'fCard' + held.id" :file-name="held.name" :is-held="true"/>
    </div>
    <TextPrompt v-show="renaming" prompt-message="Rename this file" @cancel="toBeRenamed = -1; renaming = false;" @input="async (e) => RenameItem(e)"/>
    <TextPrompt v-show="store.uploadedFile !== null" prompt-message="Name for this file?" :def-name="fileInputName" @cancel="store.uploadedFile = null;" @input="async (e) => UploadItem(e)" :output-type="uploading ? 'progress' : null" :output-value="uploadProgress"/>
    <CardProperties v-show="cardPropertyEdit !== null" :def-name="cardPropertyEdit.name" />
  </div>
  

</template>

<style scoped>

</style>