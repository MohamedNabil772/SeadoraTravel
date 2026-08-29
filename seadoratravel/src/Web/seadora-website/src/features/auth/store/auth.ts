import { defineStore } from 'pinia';
import { ref } from 'vue';
import { authApi } from '../api/authApi';
import { loadLanguageAsync } from '@/i18n';

function getCookie(name: string): string | null {
  if (typeof document === 'undefined') return null;
  const match = document.cookie.match(new RegExp('(^|;\\s*)(' + name + ')=([^;]*)'));
  return match ? decodeURIComponent(match[3]) : null;
}

function setCookie(name: string, value: string, days = 365) {
  if (typeof document === 'undefined') return;
  const expires = new Date(Date.now() + days * 864e5).toUTCString();
  document.cookie = `${name}=${encodeURIComponent(value)}; expires=${expires}; path=/; SameSite=Lax`;
}

function loadInitialFavorites(): string[] {
  try {
    const cookieFavs = getCookie('seadora_favorites');
    if (cookieFavs) {
      return JSON.parse(cookieFavs);
    }
    const localFavs = localStorage.getItem('seadora_favorites') || localStorage.getItem('seadora_guest_favorites');
    if (localFavs) {
      return JSON.parse(localFavs);
    }
  } catch (e) {
    console.warn('Failed to parse favorites', e);
  }
  return [];
}

function persistFavorites(favList: string[]) {
  const serialized = JSON.stringify(favList);
  try {
    localStorage.setItem('seadora_favorites', serialized);
    localStorage.setItem('seadora_guest_favorites', serialized);
    setCookie('seadora_favorites', serialized, 365);
  } catch (e) {
    console.warn('Failed to persist favorites', e);
  }
}

export const useAuthStore = defineStore('auth', () => {
  const user = ref<any>(null);
  const token = ref<string | null>(localStorage.getItem('token') || null);
  const isAuthenticated = ref<boolean>(!!token.value);
  const isLoggedIn = isAuthenticated; // alias for compatibility
  const isAuthModalOpen = ref(false);
  const favorites = ref<string[]>(loadInitialFavorites());

  const isFavorite = (id: number | string) => {
    return favorites.value.map(String).includes(String(id));
  };

  const toggleFavorite = (id: number | string) => {
    const strId = String(id);
    const exists = isFavorite(strId);
    if (exists) {
      favorites.value = favorites.value.filter(fid => String(fid) !== strId);
    } else {
      favorites.value.push(strId);
    }
    persistFavorites(favorites.value);

    // Asynchronously notify backend for global favorites statistics
    try {
      fetch(`/api/content/api/tours/${strId}/favorite?isFavorite=${!exists}`, {
        method: 'POST'
      }).catch(() => {});
    } catch (_) {}
  };

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
