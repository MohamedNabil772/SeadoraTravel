import axios from 'axios'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:8000'

const api = axios.create({
  baseURL: API_URL
})

// Attach JWT token if present
api.interceptors.request.use((config: any) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, (error: any) => {
  return Promise.reject(error)
})

// Handle errors like 401 Unauthorized
api.interceptors.response.use((response: any) => {
  return response
}, (error: any) => {
  if (error.response && error.response.status === 401) {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    // Redirect to login page
    window.location.href = '/login'
  }
  return Promise.reject(error)
})

export default api
