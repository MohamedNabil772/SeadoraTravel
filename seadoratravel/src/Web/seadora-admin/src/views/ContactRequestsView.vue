<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '../services/api'

interface ContactInquiry {
  id: string
  firstName: string
  lastName: string
  email: string
  interest: string | null
  message: string
  createdAt: string
  status: string
  replyMessage: string | null
  repliedAt: string | null
}

const inquiries = ref<ContactInquiry[]>([])
const loading = ref(true)

const replyTarget = ref<ContactInquiry | null>(null)
const replySubject = ref('')
const replyBody = ref('')
const sending = ref(false)

async function loadData() {
  loading.value = true
  try {
    const res = await api.get('/api/booking/api/contact')
    inquiries.value = res.data
  } catch (e) {
    console.error('Failed to load contact requests', e)
  } finally {
    loading.value = false
  }
}

function openReply(inq: ContactInquiry) {
  replyTarget.value = inq
  replySubject.value = `Re: your enquiry to Seadora Travel`
  replyBody.value = `Hi ${inq.firstName},\n\n`
}

function closeReply() {
  replyTarget.value = null
  replySubject.value = ''
  replyBody.value = ''
}

async function sendReply() {
  if (!replyTarget.value) return
  if (!replySubject.value.trim() || !replyBody.value.trim()) {
    alert('Subject and message are required.')
    return
  }
  sending.value = true
  try {
    await api.post(`/api/booking/api/contact/${replyTarget.value.id}/reply`, {
      id: replyTarget.value.id,
      subject: replySubject.value,
      message: replyBody.value
    })
    replyTarget.value.status = 'Replied'
    replyTarget.value.replyMessage = replyBody.value
    closeReply()
  } catch (e: any) {
    console.error('Failed to send reply', e)
    alert(e?.response?.data?.error || 'Failed to send reply. Check SMTP settings.')
  } finally {
    sending.value = false
  }
}

function formatDate(dateStr: string) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleString('en-US', {
    year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
  })
}

onMounted(loadData)
</script>

<template>
  <div class="contact-page">
    <div class="page-header">
      <div>
        <h2>Contact Requests</h2>
        <p>Enquiries submitted from the website contact form. Reply to send an email straight to the customer.</p>
      </div>
    </div>

    <div v-if="loading" class="loading">
      <div class="spinner"></div>
      <p>Loading requests...</p>
    </div>

    <div v-else class="table-container">
      <table class="data-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Customer</th>
            <th>Interest</th>
            <th>Message</th>
            <th>Status</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="c in inquiries" :key="c.id">
            <td>{{ formatDate(c.createdAt) }}</td>
            <td>
              <div class="customer-info">
                <div class="customer-name">{{ c.firstName }} {{ c.lastName }}</div>
                <div class="customer-email">{{ c.email }}</div>
              </div>
            </td>
            <td>{{ c.interest || '—' }}</td>
            <td class="message-cell">{{ c.message }}</td>
            <td>
              <span class="badge" :class="c.status === 'Replied' ? 'replied' : 'new'">
                {{ c.status === 'Replied' ? 'Replied ✓' : 'New' }}
              </span>
            </td>
            <td>
              <button class="btn-reply" @click="openReply(c)">Reply ✉️</button>
            </td>
          </tr>
        </tbody>
      </table>

      <div v-if="inquiries.length === 0" class="empty-state">
        <p>No contact requests yet</p>
      </div>
    </div>

    <!-- Reply modal -->
    <div v-if="replyTarget" class="modal-overlay" @click.self="closeReply">
      <div class="modal">
        <h3>Reply to {{ replyTarget.firstName }} {{ replyTarget.lastName }}</h3>
        <p class="modal-to">To: {{ replyTarget.email }}</p>

        <div class="original">
          <strong>Original message</strong>
          <p>{{ replyTarget.message }}</p>
        </div>

        <label>Subject</label>
        <input v-model="replySubject" type="text" />

        <label>Message</label>
        <textarea v-model="replyBody" rows="7"></textarea>

        <div class="modal-actions">
          <button class="btn-cancel" @click="closeReply" :disabled="sending">Cancel</button>
          <button class="btn-send" @click="sendReply" :disabled="sending">
            {{ sending ? 'Sending...' : 'Send reply' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.contact-page { color: #e0e0e0; }
.page-header { margin-bottom: 28px; }
.page-header h2 { font-size: 24px; font-weight: 700; color: #fff; margin-bottom: 4px; }
.page-header p { color: #8eafc2; font-size: 14px; }

.table-container { background: rgba(10,25,41,0.6); border: 1px solid rgba(255,255,255,0.06); border-radius: 12px; overflow: hidden; }
.data-table { width: 100%; border-collapse: collapse; }
.data-table th { padding: 14px 20px; text-align: left; font-size: 12px; letter-spacing: 0.1em; text-transform: uppercase; color: #8eafc2; background: rgba(0,0,0,0.2); border-bottom: 1px solid rgba(255,255,255,0.06); }
.data-table td { padding: 16px 20px; border-bottom: 1px solid rgba(255,255,255,0.04); font-size: 14px; vertical-align: top; }
.data-table tr:hover { background: rgba(255,255,255,0.02); }
.customer-name { font-weight: 600; color: #fff; }
.customer-email { font-size: 12px; color: #8eafc2; margin-top: 2px; }
.message-cell { max-width: 360px; word-break: break-word; color: #d0d0d0; }

.badge { padding: 4px 10px; border-radius: 999px; font-size: 12px; font-weight: 600; white-space: nowrap; }
.badge.new { background: rgba(245,164,53,0.15); color: #f5a435; border: 1px solid rgba(245,164,53,0.3); }
.badge.replied { background: rgba(46,125,79,0.15); color: #4caf78; border: 1px solid rgba(46,125,79,0.3); }

.btn-reply { padding: 6px 14px; border: none; border-radius: 4px; font-size: 12px; font-weight: 600; cursor: pointer; background: rgba(26,139,196,0.2); color: #4bb3e6; border: 1px solid rgba(26,139,196,0.35); }
.btn-reply:hover { opacity: 0.85; }

.loading { text-align: center; padding: 60px; color: #8eafc2; }
.spinner { width: 40px; height: 40px; border: 3px solid rgba(26,139,196,0.2); border-top-color: #1a8bc4; border-radius: 50%; animation: spin 0.8s linear infinite; margin: 0 auto 16px; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state { text-align: center; padding: 48px; color: #8eafc2; }

.modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 50; padding: 20px; }
.modal { background: #0a1929; border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 28px; width: 100%; max-width: 560px; max-height: 90vh; overflow-y: auto; }
.modal h3 { color: #fff; font-size: 18px; font-weight: 700; margin-bottom: 4px; }
.modal-to { color: #8eafc2; font-size: 13px; margin-bottom: 16px; }
.original { background: rgba(255,255,255,0.04); border-radius: 8px; padding: 12px 14px; margin-bottom: 18px; }
.original strong { color: #8eafc2; font-size: 12px; text-transform: uppercase; letter-spacing: 0.08em; }
.original p { color: #d0d0d0; font-size: 14px; margin-top: 6px; white-space: pre-wrap; }
.modal label { display: block; font-size: 12px; text-transform: uppercase; letter-spacing: 0.08em; color: #8eafc2; margin: 12px 0 6px; }
.modal input, .modal textarea { width: 100%; padding: 10px 12px; border-radius: 6px; border: 1px solid rgba(255,255,255,0.12); background: rgba(255,255,255,0.05); color: #fff; font-size: 14px; font-family: inherit; }
.modal textarea { resize: vertical; }
.modal-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 22px; }
.btn-cancel { padding: 9px 18px; border-radius: 6px; border: 1px solid rgba(255,255,255,0.15); background: transparent; color: #d0d0d0; cursor: pointer; }
.btn-send { padding: 9px 18px; border-radius: 6px; border: none; background: #1a8bc4; color: #fff; font-weight: 600; cursor: pointer; }
.btn-send:disabled, .btn-cancel:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
