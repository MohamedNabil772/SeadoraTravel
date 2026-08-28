<script setup lang="ts">
import { ref, watch } from 'vue'
import { X, Send } from 'lucide-vue-next'

const props = defineProps<{
  isOpen: boolean
}>()

const emit = defineEmits<{
  (e: 'close'): void
  (e: 'submit', data: any): void
}>()

const subject = ref('')
const customer = ref('')
const priority = ref('Med')
const description = ref('')

watch(() => props.isOpen, (val) => {
  if (val) {
    subject.value = ''
    customer.value = ''
    priority.value = 'Med'
    description.value = ''
  }
})

function submit() {
  emit('submit', {
    subject: subject.value,
    customer: customer.value,
    priority: priority.value,
    description: description.value
  })
  emit('close')
}
</script>

<template>
  <Teleport to="body">
    <Transition name="fade">
      <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/40 backdrop-blur-sm transition-opacity" @click="emit('close')"></div>
        
        <!-- Modal Panel -->
        <div class="relative w-full max-w-lg bg-white rounded-2xl shadow-2xl border border-border/50 overflow-hidden flex flex-col transform transition-all scale-100 animate-slide-up">
          
          <div class="px-6 py-4 border-b border-border/60 flex items-center justify-between bg-surface-sunken">
            <h3 class="text-lg font-medium text-text-main font-serif">Open New Ticket</h3>
            <button @click="emit('close')" class="p-1.5 text-text-muted hover:text-text-main hover:bg-black/5 rounded-md transition-colors">
              <X class="w-5 h-5" />
            </button>
          </div>
          
          <div class="p-6 overflow-y-auto max-h-[70vh] flex flex-col gap-5">
            <div class="flex flex-col gap-1.5">
              <label class="text-sm font-medium text-text-main">Customer</label>
              <input v-model="customer" type="text" placeholder="Search customer..." class="w-full bg-surface-sunken border border-border/80 focus:border-primary/50 focus:ring-2 focus:ring-primary/20 rounded-md px-3 py-2 text-sm outline-none transition-all">
            </div>
            
            <div class="flex flex-col gap-1.5">
              <label class="text-sm font-medium text-text-main">Subject</label>
              <input v-model="subject" type="text" placeholder="Ticket subject..." class="w-full bg-surface-sunken border border-border/80 focus:border-primary/50 focus:ring-2 focus:ring-primary/20 rounded-md px-3 py-2 text-sm outline-none transition-all">
            </div>

            <div class="flex flex-col gap-1.5">
              <label class="text-sm font-medium text-text-main">Priority</label>
              <select v-model="priority" class="w-full bg-surface-sunken border border-border/80 focus:border-primary/50 focus:ring-2 focus:ring-primary/20 rounded-md px-3 py-2 text-sm outline-none transition-all appearance-none cursor-pointer">
                <option value="Low">Low</option>
                <option value="Med">Medium</option>
                <option value="High">High</option>
                <option value="Urgent">Urgent</option>
              </select>
            </div>

            <div class="flex flex-col gap-1.5">
              <label class="text-sm font-medium text-text-main">Description</label>
              <textarea v-model="description" rows="4" placeholder="Describe the issue..." class="w-full bg-surface-sunken border border-border/80 focus:border-primary/50 focus:ring-2 focus:ring-primary/20 rounded-md px-3 py-2 text-sm outline-none transition-all resize-none"></textarea>
            </div>
          </div>
          
          <div class="px-6 py-4 border-t border-border/60 bg-surface-sunken flex justify-end gap-3">
            <button @click="emit('close')" class="px-4 py-2 text-sm font-medium text-text-muted hover:text-text-main transition-colors">Cancel</button>
            <button @click="submit" class="inline-flex items-center gap-2 bg-primary hover:bg-primary-light text-text-inverse px-4 py-2 rounded-md transition-all shadow-sm active:scale-95">
              <Send class="w-4 h-4" />
              <span>Create Ticket</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.animate-slide-up {
  animation: slideUp 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideUp {
  0% {
    opacity: 0;
    transform: translateY(10px) scale(0.98);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
</style>
