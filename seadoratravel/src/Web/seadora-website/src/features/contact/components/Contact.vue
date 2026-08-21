<script setup lang="ts">
import { ref, reactive } from 'vue'
import { contactApi } from '../api/contactApi'

const form = reactive({
  fullName: '',
  email: '',
  phone: '',
  destinationInterest: '',
  dateOrGuests: '',
  message: ''
})

const isSubmitting = ref(false)
const isSuccess = ref(false)
const toastMessage = ref('')
const toastType = ref<'success' | 'error'>('success')
const showToast = ref(false)

const triggerToast = (message: string, type: 'success' | 'error' = 'success') => {
  toastMessage.value = message
  toastType.value = type
  showToast.value = true
  setTimeout(() => {
    showToast.value = false
  }, 4000)
}

const handleSubmit = async () => {
  if (isSubmitting.value) return;
  isSubmitting.value = true
  
  try {
    await contactApi.submitInquiry({ ...form })
    
    isSubmitting.value = false
    isSuccess.value = true
    triggerToast('✓ Request Received — Our VIP Concierge will contact you shortly', 'success')
    
    // Reset form
    form.fullName = ''
    form.email = ''
    form.phone = ''
    form.destinationInterest = ''
    form.dateOrGuests = ''
    form.message = ''
    
    setTimeout(() => {
      isSuccess.value = false
    }, 4000)
  } catch (error) {
    isSubmitting.value = false
    triggerToast('Network failure. Please try again later.', 'error')
  }
}
</script>

<template>
  <section class="section" id="contact">
    <!-- Custom Luxury Toast -->
    <Transition name="toast">
      <div v-if="showToast" :class="['luxury-toast', toastType]">
        <div class="toast-icon">
          <svg v-if="toastType === 'success'" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"></polyline></svg>
          <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>
        </div>
        <span>{{ toastMessage }}</span>
      </div>
    </Transition>

    <div class="contact-wrapper">
      <div class="contact-left" v-reveal="'reveal-fade-up'">
        <div class="vip-badge">
          ✦ PERSONAL CONCIERGE & VIP INQUIRIES ✦
        </div>
        <h2 class="section-title">
          Bespoke Travel <span class="gold-accent">Experiences</span>
        </h2>
        <p class="contact-desc">
          Allow our personal concierges to craft a journey tailored to your most exacting standards. Available 24/7.
        </p>
        <div class="contact-items">
          <!-- Phone/WhatsApp -->
          <div class="contact-item glass">
            <div class="icon-wrapper gold-glow">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
              </svg>
            </div>
            <div class="item-content">
              <div class="item-header">
                <span class="detail">WhatsApp / Phone</span>
                <span class="status-badge"><span class="pulse-dot"></span> Instant response</span>
              </div>
              <div class="value"><a href="https://wa.me/201068940967" target="_blank" rel="noopener noreferrer">+20 106 894 0967</a></div>
            </div>
          </div>
          
          <!-- Email -->
          <div class="contact-item glass">
            <div class="icon-wrapper">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"></path>
                <polyline points="22,6 12,13 2,6"></polyline>
              </svg>
            </div>
            <div class="item-content">
              <div class="item-header">
                <span class="detail">Email</span>
              </div>
              <div class="value"><a href="mailto:info@sedoratravel.com">info@sedoratravel.com</a></div>
            </div>
          </div>
          
          <!-- Base -->
          <div class="contact-item glass">
            <div class="icon-wrapper">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path>
                <circle cx="12" cy="10" r="3"></circle>
              </svg>
            </div>
            <div class="item-content">
              <div class="item-header">
                <span class="detail">Base & Operations</span>
                <span class="support-badge">24/7 Support</span>
              </div>
              <div class="value">Hurghada Marina & Cairo, Egypt 🇪🇬</div>
            </div>
          </div>
        </div>
        
        <!-- Trust Guarantees Row -->
        <div class="trust-guarantees">
          <div class="trust-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path></svg>
            <span>Licensed Egyptian Operator</span>
          </div>
          <div class="trust-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="7" width="20" height="15" rx="2" ry="2"></rect><polyline points="17 2 12 7 7 2"></polyline></svg>
            <span>Private Luxury Fleet</span>
          </div>
          <div class="trust-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="var(--gold)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"></path></svg>
            <span>Instant WhatsApp Voucher</span>
          </div>
        </div>
      </div>
      
      <!-- Right Column: Frosted VIP Inquiry Card -->
      <div class="contact-right glass-card" v-reveal="'reveal-fade-up'">
        <h3 class="form-title">
          Curate Your Journey
        </h3>
        <form @submit.prevent="handleSubmit" class="vip-form">
          <div class="form-row">
            <div class="form-group">
              <label>Full Name</label>
              <input type="text" v-model="form.fullName" placeholder="e.g. John Doe" required>
            </div>
            <div class="form-group">
              <label>Email Address</label>
              <input type="email" v-model="form.email" placeholder="john@example.com" required>
            </div>
          </div>
          <div class="form-row">
            <div class="form-group">
              <label>Destination / Interest</label>
              <div class="select-wrapper">
                <select v-model="form.destinationInterest" required>
                  <option value="" disabled selected>Select an experience...</option>
                  <option>Luxury Red Sea Cruise</option>
                  <option>Cairo Historical Tour</option>
                  <option>Luxor VIP Experience</option>
                  <option>Desert Safari Adventure</option>
                  <option>Nile River Elegance</option>
                  <option>Bespoke Custom Journey</option>
                </select>
                <div class="select-icon">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 12 15 18 9"></polyline></svg>
                </div>
              </div>
            </div>
            <div class="form-group">
              <label>Date / Guest Count</label>
              <input type="text" v-model="form.dateOrGuests" placeholder="e.g. Oct 12, 2 Guests" required>
            </div>
          </div>
          <div class="form-group">
            <label>Message / Special Wishes</label>
            <textarea v-model="form.message" placeholder="Tell us about your expectations, dietary requirements, or any special celebrations..." required></textarea>
          </div>
          
          <button type="submit" class="btn-submit luxury-btn" :class="{ 'is-loading': isSubmitting, 'is-success': isSuccess }">
            <span class="btn-text" v-if="!isSubmitting && !isSuccess">Send VIP Request</span>
            <span class="btn-text" v-else-if="isSubmitting">
              <svg class="spinner" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="2" x2="12" y2="6"></line><line x1="12" y1="18" x2="12" y2="22"></line><line x1="4.93" y1="4.93" x2="7.76" y2="7.76"></line><line x1="16.24" y1="16.24" x2="19.07" y2="19.07"></line><line x1="2" y1="12" x2="6" y2="12"></line><line x1="18" y1="12" x2="22" y2="12"></line><line x1="4.93" y1="19.07" x2="7.76" y2="16.24"></line><line x1="16.24" y1="7.76" x2="19.07" y2="4.93"></line></svg>
              Processing...
            </span>
            <span class="btn-text" v-else>✓ Request Received</span>
            <div class="shine-sweep"></div>
          </button>
        </form>
      </div>
    </div>
  </section>
</template>

<style scoped>
.section { 
  padding: 120px 24px; 
  background: var(--cream, #FDFBF7); 
  position: relative;
  overflow: hidden;
}

/* Subtle background accent */
.section::before {
  content: '';
  position: absolute;
  top: -20%; left: -10%;
  width: 50%; height: 60%;
  background: radial-gradient(circle, rgba(201, 168, 76, 0.05) 0%, rgba(201,168,76,0) 70%);
  z-index: 0;
  pointer-events: none;
}

.contact-wrapper {
  max-width: 1280px;
  margin: 0 auto;
  display: grid; 
  grid-template-columns: 1fr 1fr; 
  gap: 80px; 
  align-items: center;
  position: relative;
  z-index: 1;
}

/* LEFT COLUMN */
.contact-left {
  display: flex;
  flex-direction: column;
}

.vip-badge {
  font-family: var(--font-sans, system-ui);
  font-size: 11px; 
  letter-spacing: 0.2em; 
  text-transform: uppercase;
  color: var(--gold, #C9A84C); 
  font-weight: 700; 
  margin-bottom: 16px;
  display: inline-flex;
  align-items: center;
  background: rgba(201, 168, 76, 0.1);
  padding: 8px 16px;
  border-radius: 4px;
  width: fit-content;
  border: 1px solid rgba(201, 168, 76, 0.2);
}

.section-title {
  font-family: var(--font-serif-display, 'Playfair Display', serif);
  font-size: 48px; 
  font-weight: 700; 
  color: var(--sea-deep, #0A1B28); 
  margin-bottom: 20px; 
  line-height: 1.1;
}

.gold-accent {
  color: var(--gold, #C9A84C);
  font-style: italic;
}

.contact-desc { 
  font-family: var(--font-serif-accent, 'Cormorant Garamond', serif); 
  font-size: 22px; 
  color: var(--text, #333); 
  opacity: 0.8; 
  line-height: 1.6; 
  margin-bottom: 40px; 
}

/* Glassmorphic Contact Items */
.contact-items { 
  display: flex; 
  flex-direction: column; 
  gap: 16px; 
  margin-bottom: 40px;
}

.contact-item.glass { 
  display: flex; 
  align-items: center; 
  gap: 20px; 
  background: rgba(255, 255, 255, 0.6);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255,255,255,0.8);
  padding: 16px 20px;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(10, 27, 40, 0.04);
  transition: transform 0.3s cubic-bezier(0.16, 1, 0.3, 1), box-shadow 0.3s ease;
}
.contact-item.glass:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 30px rgba(10, 27, 40, 0.08);
  border: 1px solid rgba(201, 168, 76, 0.3);
}

.icon-wrapper {
  width: 48px; height: 48px; 
  border-radius: 50%; 
  flex-shrink: 0;
  display: flex; align-items: center; justify-content: center; 
  background: var(--sea-deep, #0A1B28);
  color: var(--gold, #C9A84C);
}
.icon-wrapper.gold-glow {
  box-shadow: 0 0 15px rgba(201, 168, 76, 0.4);
}

.item-content {
  flex: 1;
}
.item-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 4px;
}

.contact-item .detail { 
  font-family: var(--font-sans, system-ui); 
  font-size: 11px; 
  color: var(--muted, #777); 
  letter-spacing: 0.1em; 
  text-transform: uppercase; 
  font-weight: 600; 
}

.status-badge, .support-badge {
  font-family: var(--font-sans, system-ui);
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 4px 8px;
  border-radius: 20px;
  display: flex;
  align-items: center;
  gap: 6px;
}
.status-badge {
  background: rgba(46, 204, 113, 0.1);
  color: #27ae60;
}
.pulse-dot {
  width: 6px; height: 6px;
  background: #2ecc71;
  border-radius: 50%;
  position: relative;
}
.pulse-dot::after {
  content: '';
  position: absolute;
  top: -2px; left: -2px; right: -2px; bottom: -2px;
  border: 2px solid #2ecc71;
  border-radius: 50%;
  animation: pulse 1.5s infinite cubic-bezier(0.4, 0, 0.2, 1);
}
@keyframes pulse {
  0% { transform: scale(0.5); opacity: 1; }
  100% { transform: scale(2); opacity: 0; }
}

.support-badge {
  background: rgba(201, 168, 76, 0.1);
  color: var(--gold, #C9A84C);
}

.contact-item .value { 
  font-family: var(--font-sans, system-ui); 
  font-size: 16px; 
  color: var(--sea-deep, #0A1B28); 
  font-weight: 600; 
}
.contact-item .value a { color: inherit; text-decoration: none; transition: color 0.2s; }
.contact-item .value a:hover { color: var(--gold, #C9A84C); }

/* Trust Guarantees */
.trust-guarantees {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  padding-top: 20px;
  border-top: 1px solid rgba(10, 27, 40, 0.1);
}
.trust-item {
  display: flex;
  align-items: center;
  gap: 8px;
  font-family: var(--font-sans, system-ui);
  font-size: 12px;
  font-weight: 600;
  color: var(--sea-deep, #0A1B28);
  background: rgba(255,255,255,0.5);
  padding: 6px 12px;
  border-radius: 4px;
}

/* RIGHT COLUMN - FROSTED VIP CARD */
.glass-card {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-radius: 16px;
  padding: 48px;
  box-shadow: 
    0 20px 40px rgba(10, 27, 40, 0.08),
    inset 0 0 0 1px rgba(255, 255, 255, 0.5);
  border: 1px solid rgba(201, 168, 76, 0.3);
  position: relative;
}
.glass-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent, var(--gold, #C9A84C), transparent);
  border-radius: 16px 16px 0 0;
  opacity: 0.8;
}

.form-title {
  font-family: var(--font-serif-display, 'Playfair Display', serif);
  font-size: 28px; 
  color: var(--sea-deep, #0A1B28); 
  margin-bottom: 32px;
  font-weight: 700;
  text-align: center;
}

.form-row { 
  display: grid; 
  grid-template-columns: 1fr 1fr; 
  gap: 20px; 
  margin-bottom: 20px; 
}
.form-group { margin-bottom: 20px; }
.form-group label {
  display: block;
  font-family: var(--font-sans, system-ui);
  font-size: 11px;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: var(--sea-deep, #0A1B28);
  font-weight: 700;
  margin-bottom: 8px;
}
.form-group input, .form-group select, .form-group textarea {
  width: 100%; 
  padding: 14px 16px;
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(10, 27, 40, 0.1);
  border-radius: 8px;
  font-family: var(--font-sans, system-ui); 
  font-size: 14px; 
  color: var(--text, #333);
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  outline: none;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.02);
}
.form-group input::placeholder, .form-group textarea::placeholder {
  color: #999;
}

.select-wrapper {
  position: relative;
}
.form-group select {
  appearance: none;
  padding-right: 40px;
  cursor: pointer;
}
.select-icon {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  pointer-events: none;
  color: var(--gold, #C9A84C);
}

.form-group input:hover, .form-group select:hover, .form-group textarea:hover {
  border-color: rgba(201, 168, 76, 0.4);
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus {
  border-color: var(--gold, #C9A84C);
  background: #fff;
  box-shadow: 0 0 0 4px rgba(201, 168, 76, 0.1);
}
.form-group textarea { 
  height: 100px; 
  resize: none; 
}

/* Luxury Button */
.luxury-btn {
  width: 100%;
  position: relative;
  overflow: hidden;
  background: var(--sea-deep, #0A1B28);
  color: #fff; 
  border: none; 
  padding: 16px; 
  border-radius: 8px;
  font-family: var(--font-sans, system-ui); 
  font-size: 14px; 
  font-weight: 700;
  letter-spacing: 0.1em; 
  text-transform: uppercase; 
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
  box-shadow: 0 8px 20px rgba(10, 27, 40, 0.3);
  margin-top: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.luxury-btn::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: linear-gradient(135deg, rgba(201,168,76,0) 0%, rgba(201,168,76,0.2) 100%);
  opacity: 0;
  transition: opacity 0.3s;
}
.luxury-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 24px rgba(10, 27, 40, 0.4);
}
.luxury-btn:hover::before {
  opacity: 1;
}
.luxury-btn:active {
  transform: translateY(1px) scale(0.98);
}
.shine-sweep {
  position: absolute;
  top: 0; left: -100%;
  width: 50%; height: 100%;
  background: linear-gradient(to right, transparent, rgba(255,255,255,0.3), transparent);
  transform: skewX(-25deg);
  animation: shine 6s infinite;
}
@keyframes shine {
  0% { left: -100%; }
  20% { left: 200%; }
  100% { left: 200%; }
}

.luxury-btn.is-loading {
  background: #333;
  pointer-events: none;
}
.luxury-btn.is-success {
  background: #2ecc71;
  pointer-events: none;
  box-shadow: 0 8px 20px rgba(46, 204, 113, 0.3);
}

/* RESPONSIVE */
@media (max-width: 1024px) {
  .contact-wrapper {
    gap: 40px;
  }
  .glass-card {
    padding: 32px;
  }
}
@media (max-width: 768px) {
  .section { padding: 80px 20px; }
  .contact-wrapper { 
    grid-template-columns: 1fr; 
    gap: 50px; 
  }
  .form-row { 
    grid-template-columns: 1fr; 
    gap: 20px; 
    margin-bottom: 20px; 
  }
  .section-title { font-size: 36px; }
  .glass-card { padding: 24px; }
  
  .contact-item.glass {
    flex-direction: column;
    align-items: flex-start;
    padding: 20px;
    gap: 12px;
  }
  .item-header {
    width: 100%;
    margin-bottom: 8px;
  }
}

/* Spinner Animation */
.spinner {
  animation: spin 1s linear infinite;
  margin-right: 8px;
}
@keyframes spin {
  100% { transform: rotate(360deg); }
}

/* Luxury Toast Styles */
.luxury-toast {
  position: fixed;
  bottom: 24px;
  right: 24px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(201, 168, 76, 0.3);
  box-shadow: 0 10px 30px rgba(10, 27, 40, 0.1);
  padding: 16px 24px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  gap: 12px;
  z-index: 1000;
  font-family: var(--font-sans, system-ui);
  font-size: 14px;
  font-weight: 500;
  color: var(--sea-deep, #0A1B28);
}
.luxury-toast.success .toast-icon {
  color: #2ecc71;
}
.luxury-toast.error .toast-icon {
  color: #e74c3c;
}
.toast-enter-active,
.toast-leave-active {
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
}
.toast-enter-from,
.toast-leave-to {
  opacity: 0;
  transform: translateY(20px) scale(0.95);
}
</style>
