<template>
  <div class="whatsapp-otp-container">
    <div class="otp-inputs" @paste="handlePaste">
      <input
        v-for="(_, index) in 6"
        :key="index"
        ref="inputRefs"
        type="text"
        inputmode="numeric"
        maxlength="1"
        class="otp-input"
        :class="{ 'has-value': otpValues[index], 'is-active': activeIndex === index }"
        v-model="otpValues[index]"
        @input="handleInput($event, index)"
        @keydown="handleKeydown($event, index)"
        @focus="activeIndex = index"
        @blur="activeIndex = -1"
        :disabled="isVerifying || isSuccess"
      />
    </div>
    
    <div class="action-area">
      <div v-if="isVerifying" class="verification-status">
        <div class="spinner"></div>
        <span>Verifying...</span>
      </div>
      <div v-else-if="isSuccess" class="verification-status success">
        <svg class="checkmark" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 52 52">
          <circle class="checkmark__circle" cx="26" cy="26" r="25" fill="none" />
          <path class="checkmark__check" fill="none" d="M14.1 27.2l7.1 7.2 16.7-16.8" />
        </svg>
        <span>Verified</span>
      </div>
      <div v-else class="resend-container">
        <button 
          class="resend-btn" 
          :disabled="cooldown > 0"
          @click="resendCode"
        >
          Resend WhatsApp Code
        </button>
        <span v-if="cooldown > 0" class="cooldown-timer">({{ cooldown }}s)</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';

const emit = defineEmits(['complete', 'resend']);

const otpValues = ref<string[]>(Array(6).fill(''));
const inputRefs = ref<HTMLInputElement[]>([]);
const activeIndex = ref(-1);
const isVerifying = ref(false);
const isSuccess = ref(false);

const cooldown = ref(60);
let timer: number | null = null;

const startTimer = () => {
  cooldown.value = 60;
  if (timer) clearInterval(timer);
  timer = window.setInterval(() => {
    if (cooldown.value > 0) {
      cooldown.value--;
    } else {
      if (timer) clearInterval(timer);
    }
  }, 1000);
};

onMounted(() => {
  startTimer();
  // Focus first input
  setTimeout(() => {
    if (inputRefs.value[0]) {
      inputRefs.value[0].focus();
    }
  }, 100);
});

onUnmounted(() => {
  if (timer) clearInterval(timer);
});

const handleInput = (event: Event, index: number) => {
  const input = event.target as HTMLInputElement;
  const val = input.value;
  
  // Ensure only numbers
  if (!/^\d*$/.test(val)) {
    otpValues.value[index] = '';
    return;
  }

  if (val && index < 5) {
    // Auto-advance
    inputRefs.value[index + 1]?.focus();
  }

  checkComplete();
};

const handleKeydown = (event: KeyboardEvent, index: number) => {
  if (event.key === 'Backspace' && !otpValues.value[index] && index > 0) {
    inputRefs.value[index - 1]?.focus();
  }
};

const handlePaste = (event: ClipboardEvent) => {
  event.preventDefault();
  const pastedData = event.clipboardData?.getData('text');
  if (!pastedData) return;

  const numbers = pastedData.replace(/\D/g, '').slice(0, 6).split('');
  
  numbers.forEach((num, idx) => {
    otpValues.value[idx] = num;
  });

  const focusIndex = Math.min(numbers.length, 5);
  inputRefs.value[focusIndex]?.focus();

  checkComplete();
};

const checkComplete = async () => {
  if (otpValues.value.every(val => val !== '')) {
    isVerifying.value = true;
    const code = otpValues.value.join('');
    
    // Simulate API call
    setTimeout(() => {
      isVerifying.value = false;
      isSuccess.value = true;
      emit('complete', code);
    }, 1500);
  }
};

const resendCode = () => {
  if (cooldown.value === 0) {
    emit('resend');
    startTimer();
    otpValues.value = Array(6).fill('');
    inputRefs.value[0]?.focus();
    isSuccess.value = false;
  }
};
</script>

<style scoped>
.whatsapp-otp-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 24px;
  width: 100%;
  max-width: 400px;
  margin: 0 auto;
}

.otp-inputs {
  display: flex;
  gap: 12px;
  justify-content: center;
}

.otp-input {
  width: 48px;
  height: 56px;
  border-radius: 12px;
  border: 1px solid #e0e0e0;
  background: #f9f9f9;
  text-align: center;
  font-size: 24px;
  font-weight: 600;
  color: #333;
  transition: all 0.2s cubic-bezier(0.25, 1, 0.5, 1); /* Emil Kowalski style easing */
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02) inset;
  outline: none;
  font-family: inherit;
}

/* Emil Kowalski motion details: Spring scaling, golden border glow */
.otp-input:focus, .otp-input.is-active {
  border-color: #d4af37; /* Golden border glow */
  background: #ffffff;
  transform: scale(1.05) translateY(-2px);
  box-shadow: 0 8px 16px rgba(212, 175, 55, 0.15), 0 0 0 3px rgba(212, 175, 55, 0.1);
}

.otp-input.has-value {
  border-color: #bbbbbb;
  background: #ffffff;
}

.action-area {
  min-height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.resend-container {
  display: flex;
  align-items: center;
  gap: 8px;
}

.resend-btn {
  background: none;
  border: none;
  color: #25D366; /* WhatsApp Green */
  font-weight: 500;
  font-size: 14px;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 8px;
  transition: background 0.2s ease, opacity 0.2s ease;
}

.resend-btn:hover:not(:disabled) {
  background: rgba(37, 211, 102, 0.1);
}

.resend-btn:disabled {
  color: #999;
  cursor: not-allowed;
  opacity: 0.7;
}

.cooldown-timer {
  font-size: 14px;
  color: #666;
  font-variant-numeric: tabular-nums;
}

/* Spinner */
.verification-status {
  display: flex;
  align-items: center;
  gap: 12px;
  color: #666;
  font-size: 15px;
  font-weight: 500;
  animation: fadeIn 0.3s ease-out;
}

.spinner {
  width: 20px;
  height: 20px;
  border: 2px solid rgba(0,0,0,0.1);
  border-left-color: #d4af37;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

/* Animated Checkmark */
.success {
  color: #25D366;
}
.checkmark {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  display: block;
  stroke-width: 3;
  stroke: #25D366;
  stroke-miterlimit: 10;
  box-shadow: inset 0px 0px 0px #25D366;
  animation: fill .4s ease-in-out .4s forwards, scale .3s ease-in-out .9s both;
}
.checkmark__circle {
  stroke-dasharray: 166;
  stroke-dashoffset: 166;
  stroke-width: 3;
  stroke-miterlimit: 10;
  stroke: #25D366;
  fill: none;
  animation: stroke 0.6s cubic-bezier(0.65, 0, 0.45, 1) forwards;
}
.checkmark__check {
  transform-origin: 50% 50%;
  stroke-dasharray: 48;
  stroke-dashoffset: 48;
  animation: stroke 0.3s cubic-bezier(0.65, 0, 0.45, 1) 0.6s forwards;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
@keyframes stroke {
  100% { stroke-dashoffset: 0; }
}
@keyframes scale {
  0%, 100% { transform: none; }
  50% { transform: scale3d(1.1, 1.1, 1); }
}
@keyframes fill {
  100% { box-shadow: inset 0px 0px 0px 30px #fff; }
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(5px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
