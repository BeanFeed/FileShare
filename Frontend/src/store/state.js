import {ref, reactive} from "vue";
import {defineStore} from "pinia";


export const userInfoStore = defineStore('user', {
    state: () => ({
        user: null
    }),
    persist: true
})

export const store = reactive({
    dropFired: false,
    cardHeld: false,
    uploadedFile: null,
    canEdit: false,
    cardPropertyEdit: null,
    overflow: 'visible'
})