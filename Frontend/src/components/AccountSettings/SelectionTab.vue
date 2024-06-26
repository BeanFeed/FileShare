<script setup>
import {onMounted} from "vue";

let props = defineProps(["text", "icon"])

</script>

<template>
<div class="hover:cursor-pointer transition-colors duration-5 ease-out hover:bg-slate-950 rounded-2xl flex justify-left items-center px-5 z-0 mb-3">
  <i :class="icon"></i>
  <p>{{text}}</p>
</div>
</template>

<style scoped>
p {
  font-size: 1.2rem;
}

i{
  padding-right: 0.25rem;
  font-size: 2rem;
}
</style>