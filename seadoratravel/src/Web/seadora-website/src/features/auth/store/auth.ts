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
  const isLoggedIn = isAuthenticated;
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

    try {
      fetch(`/api/content/api/tours/${strId}/favorite?isFavorite=${!exists}`, {
        method: 'POST'
      }).catch(() => {});
    } catch (_) {}
  };

  // Helper to load and merge persistent profile
  const getPersistedProfile = (email?: string) => {
    try {
      const emailKey = email ? `seadora_customer_profile_${email.toLowerCase().trim()}` : '';
      const raw = (emailKey ? localStorage.getItem(emailKey) : null) || localStorage.getItem('seadora_customer_profile');
      if (raw) return JSON.parse(raw);
    } catch (_) {}
    return null;
  };

  // Initialize session from localStorage if token exists
  if (token.value) {
    try {
      const storedUser = localStorage.getItem('user');
      if (storedUser) {
        const parsed = JSON.parse(storedUser);
        const saved = getPersistedProfile(parsed.email);
        user.value = {
          ...parsed,
          ...(saved || {}),
          avatarUrl: saved?.avatarUrl || parsed?.avatarUrl || ''
        };
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
    const saved = getPersistedProfile(sessionUser?.email);
    const mergedUser = {
      ...sessionUser,
      ...(saved || {}),
      avatarUrl: saved?.avatarUrl || sessionUser?.avatarUrl || '',
      name: saved?.fullName || saved?.name || sessionUser?.name || 'VIP Guest',
      phone: saved?.phoneNumber || saved?.phone || sessionUser?.phone || '',
      preferredLanguage: saved?.preferredLanguage || sessionUser?.preferredLanguage || 'en'
    };

    user.value = mergedUser;
    isAuthenticated.value = true;
    localStorage.setItem('token', sessionToken);
    localStorage.setItem('user', JSON.stringify(mergedUser));
    if (mergedUser.email) {
      localStorage.setItem(`seadora_customer_profile_${mergedUser.email.toLowerCase().trim()}`, JSON.stringify(mergedUser));
    }
    localStorage.setItem('seadora_customer_profile', JSON.stringify(mergedUser));
    if (mergedUser?.preferredLanguage) loadLanguageAsync(mergedUser.preferredLanguage);
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
      const saved = getPersistedProfile(response.data?.email);
      user.value = {
        ...response.data,
        ...(saved || {}),
        avatarUrl: saved?.avatarUrl || response.data?.avatarUrl || ''
      };
      localStorage.setItem('user', JSON.stringify(user.value));
      if (user.value?.preferredLanguage) loadLanguageAsync(user.value.preferredLanguage);
      return user.value;
    } catch (error) {
      console.warn('Fetch profile offline fallback', error);
      return user.value;
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
      if (user.value.email) {
        localStorage.setItem(`seadora_customer_profile_${user.value.email.toLowerCase().trim()}`, JSON.stringify(user.value));
      }
      localStorage.setItem('seadora_customer_profile', JSON.stringify(user.value));
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
