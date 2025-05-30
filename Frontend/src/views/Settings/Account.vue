<script setup>
import {store, userInfoStore} from "../../store/state.js";
import {BackendUrl} from "../../config.json";
import {onMounted, ref} from "vue";
import axios from "axios";

let userStore = userInfoStore();

let user = ref({
  username: "",
  role: "",
  id: -1
})

let oldInput = ref();
let newInput = ref();
let retype = ref();

onMounted(() => {
  user.value = userStore.user;
})

function ChangePassword() {
  let data = {
    username: user.value.username,
    oldPassword: oldInput.value,
    newPassword: newInput.value,
    retype: newInput.value
  }
  
  let req = axios.post(BackendUrl + "v1/User/ChangePassword", data, {withCredentials: true})
      .then((res) => {
        
      }).catch(() => {})
}

</script>

<template>
  <div>
    <section class="mb-12">
      <h1>{{user.username}}</h1>
      <p>Role: {{user.role}}</p>
      <p>ID: {{user.id}}</p>
    </section>
    <section>
      <h2>Change Password</h2>
      
      <p>Old Password</p>
      <div class="my-3 px-2 py-1 bg-slate-900 rounded-2xl w-min">
        <input v-model="oldInput" type="password" class="bg-slate-900 focus:outline-none w-64">
      </div>
      
      <p>New Password</p>
      <div class="my-3 px-2 py-1 bg-slate-900 rounded-2xl w-min">
        <input v-model="newInput" type="password" class="bg-slate-900 focus:outline-none w-64">
      </div>
      
      <p>Retype Password</p>
      <div class="my-3 px-2 py-1 bg-slate-900 rounded-2xl w-min">
        <input v-model="retype" type="password" class="bg-slate-900 focus:outline-none w-64">
      </div>
      <div @click="ChangePassword" class="my-3 bg-teal-500 hover:bg-teal-600 w-min p-2 rounded-2xl cursor-pointer">
        <p>Submit</p>
      </div>
    </section>
  </div>
</template>

<style scoped>

</style>