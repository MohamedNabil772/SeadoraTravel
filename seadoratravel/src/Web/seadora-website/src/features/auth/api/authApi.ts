import axios from 'axios'

const API_BASE_URL = '/api/auth'

export const authApi = {
  sendWhatsAppOtp(phone: string) {
    return axios.post(`${API_BASE_URL}/send-otp`, { phone })
  },
  verifyWhatsAppOtp(phone: string, code: string) {
    return axios.post(`${API_BASE_URL}/verify-otp`, { phone, code })
  },
  socialLogin(provider: string, token: string, profile: any) {
    return axios.post(`${API_BASE_URL}/social-login`, { provider, token, profile })
  },
  login(credentials: any) {
    return axios.post(`${API_BASE_URL}/login`, credentials)
  },
  register(userData: any) {
    return axios.post(`${API_BASE_URL}/register`, userData)
  }
}
