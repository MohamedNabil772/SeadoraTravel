import { defineStore } from 'pinia';
import { ref } from 'vue';
import { authApi } from '../api/authApi';
import { loadLanguageAsync } from '@/i18n';

export const useAuthStore = defineStore('auth', () => {
  const user = ref<any>(null);
  const token = ref<string | null>(localStorage.getItem('token') || null);
  const isAuthenticated = ref<boolean>(!!token.value);
  const isLoggedIn = isAuthenticated; // alias for compatibility
  const isAuthModalOpen = ref(false);
  const favorites = ref<(number | string)[]>([]);
  const isFavorite = (id: number | string) => favorites.value.includes(id);
  const toggleFavorite = (id: number | string) => { if (isFavorite(id)) { favorites.value = favorites.value.filter(fid => fid !== id); } else { favorites.value.push(id); } };

  // Initialize session from localStorage if token exists
  if (token.value) {
    try {
      const storedUser = localStorage.getItem('user');
      if (storedUser) {
        user.value = JSON.parse(storedUser);
        if (user.value.preferredLanguage) loadLanguageAsync(user.value.preferredLanguage);
      } else {
        fetchProfile();
      }
    } catch (e) {
      console.error('Failed to parse user from localStorage', e);
    }
  }

  const openAuthModal = () => {
    isAuthModalOpen.value = true;
  };

  const closeAuthModal = () => {
    isAuthModalOpen.value = false;
  };

  const setSession = (sessionToken: string, sessionUser: any) => {
    token.value = sessionToken;
    user.value = sessionUser;
    isAuthenticated.value = true;
    localStorage.setItem('token', sessionToken);
    localStorage.setItem('user', JSON.stringify(sessionUser));
    if (sessionUser?.preferredLanguage) loadLanguageAsync(sessionUser.preferredLanguage);
  };

  const login = async (data: any) => {
    try {
      const response = await authApi.login(data);
      setSession(response.data.token, response.data.user);
      return response.data;
    } catch (error) {
      console.error('Login error', error);
      throw error;
    }
  };

  const registerCustomer = async (data: any) => {
    try {
      const response = await authApi.registerCustomer(data);
      setSession(response.data.token, response.data.user);
      return response.data;
    } catch (error) {
      console.error('Registration error', error);
      throw error;
    }
  };

  async function fetchProfile() {
    try {
      const response = await authApi.fetchProfile();
      user.value = response.data;
      localStorage.setItem('user', JSON.stringify(user.value));
      if (user.value?.preferredLanguage) loadLanguageAsync(user.value.preferredLanguage);
      return response.data;
    } catch (error) {
      console.error('Fetch profile error', error);
      logout();
      throw error;
    }
  };

  const logout = () => {
    user.value = null;
    token.value = null;
    isAuthenticated.value = false;
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  };

  const updateUser = (updates: any) => {
    if (user.value) {
      user.value = { ...user.value, ...updates };
      localStorage.setItem('user', JSON.stringify(user.value));
    }
  };

  return {
    user,
    token,
    isAuthenticated,
    isAuthModalOpen,
    openAuthModal,
    closeAuthModal,
    login,
    registerCustomer,
    fetchProfile,
    isLoggedIn,
    favorites,
    isFavorite,
    toggleFavorite,
    logout,
    updateUser
  };
});
