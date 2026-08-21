<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  interest: '',
  message: ''
})
const submitting = ref(false)

const handleSubmit = async (e: Event) => {
  const formEl = e.target as HTMLFormElement
  const btn = formEl.querySelector('.btn-submit') as HTMLElement
  const orig = btn.innerHTML

  if (submitting.value) return
  submitting.value = true
  try {
    const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'
    const res = await fetch(`${API_URL}/api/booking/api/contact`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        interest: form.interest,
        message: form.message
      })
    })
    if (!res.ok) throw new Error('Request failed')

    btn.innerHTML = '✓ Message Sent!'
    btn.style.background = 'linear-gradient(135deg, var(--grass), var(--grass-light))'
    formEl.reset()
    form.firstName = form.lastName = form.email = form.interest = form.message = ''
  } catch {
    btn.innerHTML = '✕ Failed — try again'
    btn.style.background = 'linear-gradient(135deg, #c0392b, #e74c3c)'
  } finally {
    setTimeout(() => {
      btn.innerHTML = orig
      btn.style.background = ''
      submitting.value = false
    }, 3000)
  }
}
</script>

<template>
  <section class="section" id="contact">
    <div class="contact-wrapper">
      <div class="contact-left">
        <div class="section-eyebrow" style="justify-content:flex-start">
          {{ t('contact.eyebrow') }}
        </div>
        <h2>
          <span v-html="t('contact.title')"></span>
        </h2>
        <p>
          {{ t('contact.description') }}
        </p>
        <div class="contact-items">
          <div class="contact-item">
            <div class="icon icon-phone">📞</div>
            <div>
              <div class="detail">WhatsApp / Call</div>
              <div class="value"><a href="tel:+201001296641">+20 100 129 6641</a></div>
            </div>
          </div>
          <div class="contact-item">
            <div class="icon icon-email">✉️</div>
            <div>
              <div class="detail">Email</div>
              <div class="value"><a href="mailto:info@seadoratravel.com">info@seadoratravel.com</a></div>
            </div>
          </div>
          <div class="contact-item">
            <div class="icon icon-map">📍</div>
            <div>
              <div class="detail">Base</div>
              <div class="value">Hurghada, Red Sea, Egypt 🇪🇬</div>
            </div>
          </div>
        </div>
      </div>
      <div class="contact-right">
        <h3>
          {{ t('contact.form.title') }}
        </h3>
        <form @submit.prevent="handleSubmit">
          <div class="form-row">
            <div class="form-group">
              <label>{{ t('contact.form.firstName') }}</label>
              <input v-model="form.firstName" type="text" required :placeholder="t('contact.form.placeholders.firstName')">
            </div>
            <div class="form-group">
              <label>{{ t('contact.form.lastName') }}</label>
              <input v-model="form.lastName" type="text" required :placeholder="t('contact.form.placeholders.lastName')">
            </div>
          </div>
          <div class="form-group">
            <label>{{ t('contact.form.email') }}</label>
            <input v-model="form.email" type="email" required :placeholder="t('contact.form.placeholders.email')">
          </div>
          <div class="form-group">
            <label>{{ t('contact.form.interest') }}</label>
            <select v-model="form.interest">
              <option>{{ t('contact.form.interests.hurghada') }}</option>
              <option>{{ t('contact.form.interests.cairo') }}</option>
              <option>{{ t('contact.form.interests.luxor') }}</option>
              <option>{{ t('contact.form.interests.sharm') }}</option>
              <option>{{ t('contact.form.interests.desert') }}</option>
              <option>{{ t('contact.form.interests.nile') }}</option>
              <option>{{ t('contact.form.interests.custom') }}</option>
            </select>
          </div>
          <div class="form-group">
            <label>{{ t('contact.form.message') }}</label>
            <textarea v-model="form.message" required :placeholder="t('contact.form.placeholders.message')"></textarea>
          </div>
          <button type="submit" class="btn-submit">
            {{ t('contact.form.submit') }}
          </button>
        </form>
      </div>
    </div>
  </section>
</template>

<style scoped>
/* ─── CONTACT ─── */
.section { padding: 100px 48px; }
.section-eyebrow {
  font-size: 11px; letter-spacing: 0.25em; text-transform: uppercase;
  color: var(--grass); font-weight: 600; margin-bottom: 14px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
}
.section-eyebrow::before, .section-eyebrow::after {
  content: ''; width: 40px; height: 1px; background: var(--grass);
}

.contact-wrapper {
  display: grid; grid-template-columns: 1fr 1fr; gap: 80px; align-items: center;
}
.contact-left h2 {
  font-family: 'Playfair Display', serif;
  font-size: 44px; font-weight: 700; color: var(--dark); margin-bottom: 20px; line-height: 1.15;
}
.contact-left h2 :deep(span) { color: var(--sea); }
.contact-left p { font-family: 'Cormorant Garamond', serif; font-size: 18px; color: var(--muted); line-height: 1.7; margin-bottom: 36px; }
.contact-items { display: flex; flex-direction: column; gap: 20px; }
.contact-item { display: flex; align-items: center; gap: 16px; }
.contact-item .icon {
  width: 48px; height: 48px; border-radius: 10px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center; font-size: 22px;
}
.icon-phone { background: linear-gradient(135deg, var(--sea), var(--sea-light)); }
.icon-email { background: linear-gradient(135deg, var(--sun), var(--sun-light)); }
.icon-map { background: linear-gradient(135deg, var(--grass), var(--grass-light)); }
.contact-item .detail { font-size: 13px; color: var(--muted); }
.contact-item .value { font-size: 16px; color: var(--dark); font-weight: 500; }
.contact-item .value a { color: var(--sea); text-decoration: none; }
.contact-right {
  background: var(--white); border-radius: 16px;
  padding: 44px; box-shadow: 0 20px 60px rgba(0,0,0,0.1);
}
.contact-right h3 {
  font-family: 'Playfair Display', serif;
  font-size: 26px; color: var(--dark); margin-bottom: 28px;
}
.form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 16px; }
.form-group { margin-bottom: 16px; }
.form-group label { display: block; font-size: 12px; letter-spacing: 0.1em; text-transform: uppercase; color: var(--muted); margin-bottom: 6px; }
.form-group input, .form-group select, .form-group textarea {
  width: 100%; padding: 12px 16px; border: 1px solid #dce6ec; border-radius: 6px;
  font-family: 'Jost', sans-serif; font-size: 14px; color: var(--dark);
  background: #f7fbfd; transition: border-color 0.2s;
  outline: none;
}
.form-group input:focus, .form-group select:focus, .form-group textarea:focus {
  border-color: var(--sea); background: var(--white);
}
.form-group textarea { height: 100px; resize: none; }
.btn-submit {
  width: 100%; background: linear-gradient(135deg, var(--sea), var(--sea-light));
  color: var(--white); border: none; padding: 16px; border-radius: 6px;
  font-family: 'Jost', sans-serif; font-size: 14px; font-weight: 600;
  letter-spacing: 0.1em; text-transform: uppercase; cursor: pointer;
  transition: all 0.3s;
}
.btn-submit:hover { transform: translateY(-2px); box-shadow: 0 10px 30px rgba(10,92,138,0.35); }

@media (max-width: 768px) {
  .contact-wrapper { grid-template-columns: 1fr; gap: 40px; }
}
</style>
