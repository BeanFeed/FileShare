<script setup>
import SelectionTab from "../../components/AccountSettings/SelectionTab.vue";
import {onMounted, watch} from "vue";
import {store, userInfoStore} from "../../store/state.js";
import {router} from "../../router/index.js";

let userStore = userInfoStore();

onMounted(() => {
  if (userStore.user == null) router.push("/Login");
});

watch(userStore.user, () => {
  if (userStore.user == null) router.push("/Login");
})

</script>

<template>
<div class="fixed top-0 left-0 flex w-screen h-screen justify-center items-center">
  <div class="flex h-full py-36">
    <div class="box w-[15rem] p-5 bg-slate-850 overflow-y-auto">
      <SelectionTab text="Account" icon="bi bi-gear-fill"/>
      <SelectionTab text="Users" icon="bi bi-person-fill"/>
      <SelectionTab text="System Settings" icon="bi bi-globe2" class=""/>
    </div>
    <div class="box w-[50vw] bg-slate-850 p-10 overflow-auto">
     <router-view />
    </div>
  </div>
  
</div>
</template>

<style scoped>
.box {
  height: 100%;
  margin: 0 10px; /* Adjust as needed */
  border-radius: 1.5rem;
}

i {
  font-size: 2rem;
}

</style>