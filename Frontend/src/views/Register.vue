<script setup>
import {useRoute} from "vue-router";
import {BackendUrl} from "../config.json";
import {ref} from "vue";
import axios from "axios";

var username = ref('');
var password = ref('');
var route = useRoute()

function sendLoginPost() {
  if (route.query.returnUrl === undefined) {
    console.log("Redirect Failed")
    return;
  }

  var req = axios.post(BackendUrl + "v1/user/register", {
    username: username,
    password: password
  }, {withCredentials: true}).then((res) => {
    if (res.data.success) window.location.href = decodeURI(route.query.returnUrl);
  }).catch(() => {
    
  });
}
</script>

<template>
  <div class="flex items-center justify-center py-auto h-screen">
    <div class="cBorder w-96 p-10 bg-slate-900">
      <h1 class="text-center">Signup</h1>
      <hr>
      <div class="formInput text-left">
        <p>Username</p>
        <div class="bg-slate-800 w-full px-3 py-1">
          <input v-bind:value="username" type="text" class=" bg-slate-800 focus:outline-none w-full">
        </div>
      </div>
      <div class="formInput text-left">
        <p>Password</p>
        <div class="bg-slate-800 w-full px-3 py-1">
          <input v-bind:value="password" type="text" class=" bg-slate-800 focus:outline-none w-full">
        </div>
      </div>
      <hr>
      <button @click="sendLoginPost()" class="btn-grad w-full focus:outline-none">Continue</button>
    </div>
  </div>
</template>

<style scoped>
.cBorder * {
  margin-top: 1rem;
  margin-bottom: 1rem;
}
.formInput * {
  margin: 0.1rem;
}
div {
  border-radius: 2rem;

}



.btn-grad {
  background-image: linear-gradient(to right, #4776E6 0%, #8E54E9  51%, #4776E6  100%);
  text-align: center;
  transition: 0.5s;
  background-size: 200% auto;

  border-radius: 5rem;
  display: block;
}

.btn-grad:hover {
  background-position: right center; /* change the direction of the change here */

  text-decoration: none;
  outline: none;
}

</style>