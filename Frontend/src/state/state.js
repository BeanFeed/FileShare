import {ref, reactive} from "vue";

export const store = reactive({
    dropFired: false,
    cardHeld: false,
    uploadedFile: null,
    canEdit: false,
    user: null
})