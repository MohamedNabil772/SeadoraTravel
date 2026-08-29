<template>
  <div class="space-y-8">
    <!-- Header -->
    <div class="bg-white rounded-3xl p-8 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
      <div>
        <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-rose-500/10 border border-rose-500/20 text-rose-600 text-[11px] font-bold uppercase tracking-widest mb-2">
          <span>💖</span> {{ $t('portal.nav.savedExperiences') || 'Wishlist & Favorites' }}
        </div>
        <h1 class="text-2xl md:text-3xl font-bold text-slate-900 tracking-tight">
          {{ $t('portal.nav.favoritesTitle') || 'Your Saved Experiences' }}
        </h1>
        <p class="text-slate-500 text-sm mt-1">
          {{ $t('portal.nav.favoritesSubtitle') || 'Handpicked luxury journeys saved for your upcoming travel adventures.' }}
        </p>
      </div>

      <router-link 
        to="/tours" 
        class="px-5 py-2.5 bg-[#062d4d] hover:bg-[#0a3d66] active:scale-[0.97] text-white text-xs font-bold rounded-xl transition-all shadow-md flex items-center gap-2 shrink-0"
      >
        <span>⛵</span>
        <span>{{ $t('portal.nav.exploreAll') || 'Explore More Tours' }}</span>
      </router-link>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="flex flex-col items-center justify-center py-20 bg-white rounded-3xl border border-slate-200/80 shadow-xs">
      <div class="w-10 h-10 border-4 border-[#c9a84c]/20 border-t-[#c9a84c] rounded-full animate-spin"></div>
      <p class="text-xs text-slate-400 font-bold uppercase tracking-widest mt-4">Loading your favorites...</p>
    </div>

    <!-- Empty State -->
    <div v-else-if="favoritedTours.length === 0" class="bg-white rounded-3xl p-12 text-center border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
      <div class="w-16 h-16 bg-rose-50 rounded-2xl flex items-center justify-center text-2xl mx-auto mb-4 text-rose-500">
        💖
      </div>
      <h3 class="text-lg font-bold text-slate-900 mb-2">No Saved Experiences Yet</h3>
      <p class="text-slate-500 text-sm max-w-md mx-auto mb-6">
        When browsing our collection of private yachts, desert expeditions, and historical tours, click the heart icon to save your favorites here.
      </p>
      <router-link 
        to="/tours" 
        class="inline-flex items-center gap-2 px-6 py-3 bg-[#062d4d] hover:bg-[#c9a84c] text-white font-bold text-sm rounded-xl transition-all shadow-md active:scale-95"
      >
        <span>Discover Experiences</span>
        <span>→</span>
      </router-link>
    </div>

    <!-- Favorites Grid -->
    <div v-else class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
      <div 
        v-for="tour in favoritedTours" 
        :key="tour.id"
        class="bg-white rounded-3xl border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] hover:shadow-[0_16px_36px_rgba(6,45,77,0.1)] transition-all duration-300 flex flex-col overflow-hidden group cursor-pointer"
        @click="goToTour(tour)"
      >
        <!-- Image Header -->
        <div class="relative h-52 w-full overflow-hidden bg-slate-100 shrink-0">
          <img 
            :src="getImageUrl(tour)" 
            :alt="getTourTitle(tour)"
            class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
          />
          
          <!-- Badges -->
          <div class="absolute top-4 left-4 flex flex-col gap-1.5">
            <span class="px-2.5 py-1 bg-[#062d4d]/90 backdrop-blur-md text-white text-[10px] font-bold rounded-lg uppercase tracking-wider">
              {{ tour.destinationName || 'Red Sea' }}
            </span>
          </div>

          <!-- Remove Button -->
          <button 
            @click.stop="removeFavorite(tour.id)"
            class="absolute top-4 right-4 w-9 h-9 rounded-full bg-white/95 backdrop-blur-md text-rose-500 shadow-md flex items-center justify-center hover:scale-110 active:scale-95 transition-transform"
            title="Remove from favorites"
          >
            <svg class="w-4 h-4 fill-rose-500" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z"></path>
            </svg>
          </button>

          <!-- Duration & Rating Overlay -->
          <div class="absolute bottom-3 left-3 right-3 flex justify-between items-center px-3 py-1.5 bg-black/60 backdrop-blur-md rounded-xl text-white text-xs">
            <span class="flex items-center gap-1 font-medium">
              <span>⏱</span> {{ tour.duration || 'Full Day' }}
            </span>
            <span class="text-[#c9a84c] font-bold">
              ★ {{ tour.rating || '4.9' }}
            </span>
          </div>
        </div>

        <!-- Details Body -->
        <div class="p-6 flex flex-col flex-1 justify-between gap-4">
          <div>
            <div class="text-[11px] font-bold uppercase tracking-wider text-slate-400 mb-1">
              {{ tour.categoryName || 'Luxury Excursion' }}
            </div>
            <h3 class="font-bold text-base text-slate-900 group-hover:text-[#c9a84c] transition-colors leading-snug line-clamp-2">
              {{ getTourTitle(tour) }}
            </h3>
            <p class="text-xs text-slate-500 mt-2 line-clamp-2 leading-relaxed">
              {{ tour.description || 'Experience the unparalleled beauty of Egypt with private VIP access and first-class services.' }}
            </p>
          </div>

          <div class="pt-4 border-t border-slate-100 flex items-center justify-between">
            <div>
              <span class="text-[10px] uppercase font-bold text-slate-400 block">Starting From</span>
              <span class="text-lg font-black text-[#062d4d]">
                {{ currencyStore.formatPrice(tour.price) }}
              </span>
            </div>

            <button 
              @click.stop="goToTour(tour)"
              class="px-4 py-2 bg-gradient-to-r from-[#062d4d] to-[#0f172a] hover:from-[#c9a84c] hover:to-[#dfc379] text-white hover:text-slate-900 text-xs font-bold rounded-xl transition-all shadow-sm active:scale-95"
            >
              Book Now →
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '@/features/auth/store/auth';
import { useCurrencyStore } from '@/store/currency';

const router = useRouter();
const { locale } = useI18n();
const authStore = useAuthStore();
const currencyStore = useCurrencyStore();

const allTours = ref<any[]>([]);
const loading = ref(true);

const favoritedTours = computed(() => {
  return allTours.value.filter(t => authStore.isFavorite(t.id));
});

function getTourTitle(tour: any) {
  if (tour.names && tour.names[locale.value]) {
    return tour.names[locale.value];
  }
  if (tour.names && tour.names['en']) {
    return tour.names['en'];
  }
  return tour.title || 'Luxury Experience';
}

function getImageUrl(tour: any) {
  const img = tour.imageUrl || tour.mainImage || (tour.mediaUrls && tour.mediaUrls[0]);
  if (!img) return 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=800&q=80';
  if (img.startsWith('http') || img.startsWith('/')) return img;
  return `/${img}`;
}

function getSlug(text: string) {
  return (text || 'tour').toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/(^-|-$)+/g, '');
}

function goToTour(tour: any) {
  const slug = tour.slug || getSlug(tour.title || tour.names?.['en'] || 'tour');
  router.push(`/tour/${slug}`);
}

function removeFavorite(tourId: string | number) {
  authStore.toggleFavorite(tourId);
}

onMounted(async () => {
  loading.value = true;
  try {
    const res = await fetch('/api/content/api/tours');
    if (res.ok) {
      allTours.value = await res.json();
    }
  } catch (err) {
    console.error('Failed to load tours for favorites', err);
  } finally {
    loading.value = false;
  }
});
</script>
