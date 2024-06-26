<script setup>
import {ref, watchEffect} from "vue";
import axios from "axios";
import Config from "../config.json";
import {useRoute} from "vue-router";
import {store} from "../state/state.js";
const route = useRoute();
let props = defineProps(["defName","isDir"])
defineEmits(['save'])
let text = ref('');
let visibilityDropdown = ref('');
const dPath = ref();
let anyoneCanPlace = ref();
dPath.value = [""]
let data = {};
watchEffect(async () => {
  
  dPath.value = route.params.Directory === undefined ? [] : route.params.Directory;
  console.log(text.value.length);
  if(props.isDir !== null){
    
    let newPath = [...dPath.value];
    newPath[newPath.length] = props.defName;
    let req = axios.get(Config.BackendUrl + "v1/filesystem/getitemproperties" + GetUrlArray("pathArr" ,newPath) + "&isDirectory=" + props.isDir, {withCredentials: true})
    .then(res => {
      data = res.data.message;
      visibilityDropdown.value = data.visibility;
      anyoneCanPlace.value = data.anyoneCanUpload;
    })
    .catch(res => {});
    
  }
  
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

async function DeleteItem() {
  let path = [...dPath.value];
  if(path[0] === "") path = [props.defName];
  else path[path.length] = props.defName;
  let req = await axios({
    url:Config.BackendUrl + "v1/filesystem/deleteitem",
    data: path,
    method:"DELETE",
    withCredentials: true
  }).then(async function (res) {
    if (res.status === 200 && res.data.success === true) {
      
      console.log(res.data.message)
      store.cardPropertyEdit = null;
    }
  }).catch();
}

function CloseMenu() {
  store.cardPropertyEdit = null;
}

async function SaveProperties() {
  if (text.value.length !== 0) {

    let oldPath = [...dPath.value];
    if (oldPath[0] === ""){
      oldPath = [text.value];
    } else {
      oldPath[oldPath.length] = text.value;
    }
    //console.log(oldPath)
    let nameData = {
      itemPath: oldPath,
      newName: text.value
    }
    
    let req = axios.post(Config.BackendUrl + "v1/filesystem/renameitem", nameData, {
      headers: {'Content-Type': 'application/json'},
      withCredentials: true
    }).then(res => {
      
    }).catch(() => {});
  }
  
  if (data.visibility !== visibilityDropdown.value) {

    let oldPath = [...dPath.value];
    if (oldPath[0] === ""){
      oldPath = [text.value];
    } else {
      oldPath[oldPath.length] = text.value.length === 0 ? props.defName : text.value;
    }
    
    let propertyData = {
      pathArr: oldPath,
      visibility: visibilityDropdown.value
    }
    
    let req = axios.post(Config.BackendUrl + "v1/filesystem/changeitemvisibility", propertyData, {
      headers: {'Content-Type': 'application/json'},
      withCredentials: true
    }).then(res => {

    }).catch(() => {});
  }
  
  if (anyoneCanPlace.value !== data.anyoneCanUpload) {
    let oldPath = [...dPath.value];
    if (oldPath[0] === ""){
      oldPath = [text.value];
    } else {
      oldPath[oldPath.length] = text.value.length === 0 ? props.defName : text.value;
    }
    
    let uploadData = {
      pathArr: oldPath,
      anyoneCanUpload: anyoneCanPlace.value
    };
    
    let req = axios.post(Config.BackendUrl + "v1/filesystem/changepublicuploading", uploadData, {
      headers: {'Content-Type': 'application/json'},
      withCredentials: true
    }).then(() => {}).catch(() => {});
  }
  
  store.cardPropertyEdit = null;
}
</script>

<template>
<div class="overflow-hidden fixed left-0 top-0 w-screen h-screen bg-black bg-opacity-50 flex items-center justify-center z-50">
  <div class="bg-slate-900 rounded-lg px-3 py-3 text-center">
    <div class="my-3 px-2 py-1 bg-slate-850 rounded-2xl">
      <input v-model="text" :placeholder="defName" type="text" class="bg-slate-850 focus:outline-none">
    </div>
    <div class="grid grid-cols-2 gap-3 items-center align-middle">
      <select v-model="visibilityDropdown" class="bg-slate-850 rounded-lg px-2">
        <option value="public">Public</option>
        <option value="private">Private</option>
        <option value="unlisted">Unlisted</option>
      </select>
      <template v-if="isDir">
        <div class="container" style="display: flex; vertical-align: middle">
          <input v-model="anyoneCanPlace" style="margin-bottom: auto; margin-top: auto;" class="size-5 mr-1" type="checkbox">
          <p>Public Placing</p>
        </div>
      </template>
    </div>
    <div class="grid grid-cols-3 gap-3">
      <a class="text-white hover:text-white bg-red-500"  @click="DeleteItem">Delete</a>
      <a class="text-white hover:text-white bg-teal-500" @click="SaveProperties">Save</a>
      <a class="text-black hover:text-black bg-white"    @click="CloseMenu">Cancel</a>
    </div>
    
  </div>
</div>

</template>

<style scoped>
a {
  cursor: pointer;
  padding: 0.125rem 0.5rem;
  border-radius: 0.5rem;
  margin-top: 0.5rem;
}


</style>