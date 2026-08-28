import axios from 'axios';

// Mock API for Auth
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:3000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authApi = {
  login(data: any) {
    // Mocked implementation
    return Promise.resolve({
      data: {
        token: 'mock-jwt-token',
        user: { id: 1, name: data.name || 'John Doe', email: data.email || data.phone }
      }
    });
  },
  
  registerCustomer(data: any) {
    return Promise.resolve({
      data: {
        token: 'mock-jwt-token',
        user: { id: 1, name: data.name, email: data.email }
      }
    });
  },

  fetchProfile() {
    return apiClient.get('/customer/portal/me');
  },
  
  updateProfile(data: any) {
    return apiClient.put('/customer/portal/me', data);
  },
  
  fetchVoucher(id: string) {
    return apiClient.get(`/customer/portal/bookings/${id}/voucher`);
  }
};
