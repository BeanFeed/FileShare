<script setup>
import {onMounted, ref} from "vue";
  
  let props = defineProps(["promptMessage","defName","outputType", "outputValue"])
  let text = ref(null);
  defineEmits(['cancel','input'])
  let msg = ref(props.promptMessage)
  
</script>

<template>
  <div class="fixed left-0 top-0 w-screen h-screen bg-black bg-opacity-50 flex items-center justify-center">
    <div class="bg-slate-900 rounded-lg px-3 py-3 text-center">
      <p>{{promptMessage}}</p>
      <div class="my-3 px-2 py-1 bg-slate-850 rounded-2xl">
        <input v-model="text" :placeholder="defName" type="text" class="bg-slate-850 focus:outline-none">
      </div>
      <div class="flex-row" v-show="outputType === undefined || outputType === null">
        <a @click="$emit('cancel')" class="cursor-pointer mx-4 text-white bg-slate-850 px-1.5 py-0.5 rounded-lg hover:text-white hover:bg-slate-950">Cancel</a>
        <a @click="$emit('input', text); text = null;" class="cursor-pointer mx-4 text-slate-900 bg-teal-500 px-1.5 py-0.5 rounded-lg hover:text-slate-900 hover:bg-teal-600">Submit</a>
      </div>
      <div class="flex-row" v-show="outputType === 'progress'">
        <progress class="rounded-lg h-1" :value="outputValue" max="1" />
      </div>
    </div>
  </div>
</template>

<style scoped>

</style>