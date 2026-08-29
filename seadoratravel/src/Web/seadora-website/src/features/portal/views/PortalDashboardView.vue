<template>
  <div class="space-y-8">
    <!-- Welcome & Journey Countdown Hero Card -->
    <div class="bg-white rounded-3xl p-8 md:p-10 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] relative overflow-hidden">
      <div class="absolute -right-16 -top-16 w-80 h-80 bg-gradient-to-br from-[#c9a84c]/10 to-transparent rounded-full blur-3xl pointer-events-none"></div>
      
      <div class="relative z-10 flex flex-col lg:flex-row justify-between items-start lg:items-center gap-8">
        <div class="max-w-2xl">
          <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-[#c9a84c]/15 border border-[#c9a84c]/30 text-[#a38030] text-[11px] font-bold uppercase tracking-widest mb-3">
            <span>✦</span> {{ $t('portal.dashboard.vipLounge') }}
          </div>
          <h1 class="text-3xl md:text-4xl font-bold text-slate-900 tracking-tight leading-tight">
            {{ $t('portal.dashboard.welcome') }} <span class="text-[#062d4d]">{{ customerFirstName }}</span>.
          </h1>
          <p class="text-slate-500 text-sm md:text-base mt-2 leading-relaxed">
            {{ $t('portal.dashboard.anticipating') }} <strong class="text-slate-800">Luxor & the Nile Valley</strong>:
          </p>

          <!-- Countdown Timer Pills -->
          <div class="flex flex-wrap gap-3.5 mt-6">
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">12</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">{{ $t('portal.dashboard.days') }}</div>
            </div>
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">08</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">{{ $t('portal.dashboard.hours') }}</div>
            </div>
            <div class="bg-[#F8FAFC] border border-slate-200/80 rounded-2xl px-4 py-3 text-center min-w-[80px] shadow-xs">
              <div class="text-2xl md:text-3xl font-bold text-[#c9a84c] tracking-tight">45</div>
              <div class="text-[10px] text-slate-400 uppercase tracking-widest font-semibold mt-0.5">{{ $t('portal.dashboard.mins') }}</div>
            </div>
          </div>
        </div>

        <!-- Destination Weather & Live Status Widget -->
        <div class="bg-[#062d4d] text-white rounded-3xl p-6 md:p-8 w-full lg:w-auto shadow-xl min-w-[260px] relative overflow-hidden flex flex-col justify-between">
          <div class="absolute -bottom-8 -right-8 w-32 h-32 bg-[#c9a84c]/20 rounded-full blur-2xl"></div>
          <div>
            <div class="text-[10px] font-bold text-[#c9a84c] uppercase tracking-widest mb-1">{{ $t('portal.dashboard.destinationClimate') }}</div>
            <div class="text-xl font-bold">Luxor, Egypt</div>
            <div class="text-3xl md:text-4xl font-bold text-white mt-3 mb-1">
              {{ weatherTemp !== null ? weatherTemp + '°C' : '--°C' }}
            </div>
            <div class="text-xs text-white/70 font-medium">{{ weatherDescription }}</div>
          </div>
          <div class="mt-6 pt-4 border-t border-white/10 flex items-center justify-between text-xs text-white/80">
            <span>Water: 27°C</span>
            <span class="text-[#c9a84c] font-semibold">Optimal Sailing</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Asymmetrical Editorial Layout: Upcoming Journey vs Concierge Actions -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      
      <!-- Main Featured Trip Card (2 Columns) -->
      <div class="lg:col-span-2 bg-white rounded-3xl p-8 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex flex-col justify-between">
        <div>
          <div class="flex items-center justify-between gap-4 mb-4">
            <div class="flex items-center gap-2">
              <span class="w-2.5 h-2.5 rounded-full bg-emerald-500 animate-pulse"></span>
              <span class="text-xs font-bold text-slate-500 uppercase tracking-wider">{{ $t('portal.dashboard.activeJourney') }}</span>
            </div>
            <span class="font-mono text-xs font-bold text-[#062d4d] bg-slate-100 px-2.5 py-1 rounded-lg">#BK-10293</span>
          </div>

          <h2 class="text-xl md:text-2xl font-bold text-slate-900 tracking-tight mb-2">
            7-Night Private Dahabiya Expedition: Luxor to Aswan
          </h2>
          <p class="text-slate-500 text-sm leading-relaxed mb-6">
            Private luxury Nile sailing with onboard Egyptologist, private chef, fast-track temple access, and sunset felucca dining.
          </p>

          <!-- Key Journey Details Pills -->
          <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 mb-6">
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Dates</div>
              <div class="text-xs font-bold text-slate-800 mt-0.5">Oct 30 – Nov 6</div>
            </div>
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Guests</div>
              <div class="text-xs font-bold text-slate-800 mt-0.5">2 VIP Guests</div>
            </div>
            <div class="bg-slate-50 border border-slate-100 rounded-2xl p-3.5">
              <div class="text-[10px] uppercase font-bold text-slate-400 tracking-wider">Passports</div>
              <div class="text-xs font-bold text-emerald-600 mt-0.5">Verified on File ✓</div>
            </div>
          </div>
        </div>

        <div class="pt-4 border-t border-slate-100 flex flex-wrap items-center justify-between gap-4">
          <router-link 
            to="/portal/bookings" 
            class="text-xs font-bold text-[#062d4d] hover:text-[#c9a84c] transition-colors flex items-center gap-1.5"
          >
            <span>{{ $t('portal.dashboard.viewItinerary') }}</span>
            <span>→</span>
          </router-link>
          
          <button 
            @click="showVoucherModal = true"
            class="px-4 py-2 bg-slate-100 hover:bg-[#062d4d] hover:text-white active:scale-[0.97] text-slate-700 text-xs font-bold rounded-xl transition-[background-color,color,transform] duration-200 ease-[cubic-bezier(0.16,1,0.3,1)]"
          >
            {{ $t('portal.dashboard.downloadVouchers') }}
          </button>
        </div>
      </div>

      <!-- Right Column: Bespoke Concierge & Quick Portals -->
      <div class="space-y-6 flex flex-col justify-between">
        
        <!-- Bespoke VIP Concierge Banner Card -->
        <div class="bg-gradient-to-br from-[#062d4d] to-[#0d3f66] text-white rounded-3xl p-7 shadow-lg flex-1 flex flex-col justify-between relative overflow-hidden">
          <div class="relative z-10">
            <div class="w-10 h-10 rounded-2xl bg-[#c9a84c] text-[#062d4d] flex items-center justify-center font-bold text-base mb-4 shadow-md">
              ✦
            </div>
            <h3 class="text-lg font-bold mb-1">{{ $t('portal.dashboard.bespokeTitle') }}</h3>
            <p class="text-xs text-white/80 leading-relaxed mb-6">
              {{ $t('portal.dashboard.bespokeDesc') }}
            </p>
          </div>
          <router-link 
            to="/portal/support" 
            class="relative z-10 w-full py-3 bg-[#c9a84c] hover:bg-[#d8b85c] text-[#062d4d] font-bold rounded-xl text-xs text-center transition-all duration-200 active:scale-[0.97] shadow-sm block"
          >
            {{ $t('portal.dashboard.sendBespoke') }}
          </router-link>
        </div>

        <!-- Explore New Journeys Card -->
        <div class="bg-white rounded-3xl p-7 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)] flex-1 flex flex-col justify-between">
          <div>
            <div class="text-[10px] font-bold text-[#c9a84c] uppercase tracking-wider mb-1">SeeDora Catalog</div>
            <h3 class="text-base font-bold text-slate-900 mb-1">{{ $t('portal.dashboard.curateTitle') }}</h3>
            <p class="text-xs text-slate-500 leading-relaxed mb-4">
              {{ $t('portal.dashboard.curateDesc') }}
            </p>
          </div>
          <router-link 
            to="/tours" 
            class="w-full py-3 bg-slate-100 hover:bg-slate-200/80 text-slate-800 font-bold rounded-xl text-xs text-center transition-all duration-200 active:scale-[0.97] block"
          >
            {{ $t('portal.dashboard.browseExperiences') }} →
          </router-link>
        </div>

      </div>
    </div>

    <!-- Saved Experiences / Wishlist Preview Section -->
    <div v-if="favoritedTours.length > 0" class="bg-white rounded-3xl p-8 border border-slate-200/80 shadow-[0_8px_30px_rgb(0,0,0,0.04)]">
      <div class="flex items-center justify-between mb-6">
        <div>
          <div class="inline-flex items-center gap-1.5 text-xs font-bold text-rose-500 uppercase tracking-wider mb-1">
            <span>💖</span> {{ $t('portal.nav.savedExperiences') || 'Wishlist' }}
          </div>
          <h2 class="text-xl font-bold text-slate-900 tracking-tight">Your Saved Favorites ({{ favoritedTours.length }})</h2>
        </div>
        <router-link 
          to="/portal/favorites" 
          class="text-xs font-bold text-[#062d4d] hover:text-[#c9a84c] transition-colors flex items-center gap-1"
        >
          <span>View All</span>
          <span>→</span>
        </router-link>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
        <div 
          v-for="fav in favoritedTours.slice(0, 3)" 
          :key="fav.id"
          class="rounded-2xl border border-slate-100 bg-slate-50/50 hover:bg-white hover:shadow-md transition-all duration-300 overflow-hidden flex flex-col cursor-pointer group"
          @click="router.push(`/tour/${fav.slug || fav.id}`)"
        >
          <div class="h-32 relative overflow-hidden bg-slate-200">
            <img 
              :src="fav.imageUrl || fav.mainImage || 'https://images.unsplash.com/photo-1544551763-46a013bb70d5?auto=format&fit=crop&w=600&q=80'" 
              class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
            />
            <div class="absolute bottom-2 left-2 px-2 py-0.5 rounded bg-black/60 backdrop-blur-md text-[10px] text-white font-medium">
              ★ {{ fav.rating || '4.9' }}
            </div>
          </div>
          <div class="p-4 flex flex-col justify-between flex-1 gap-2">
            <h4 class="font-bold text-xs text-slate-900 line-clamp-1 group-hover:text-[#c9a84c] transition-colors">
              {{ getTourTitle(fav) }}
            </h4>
            <div class="flex items-center justify-between text-xs mt-1">
              <span class="font-black text-sm text-[#062d4d]">
                {{ currencyStore.formatPrice(fav.price) }}
              </span>
              <span class="text-[11px] font-bold text-[#c9a84c] group-hover:underline">
                Book →
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Luxury PDF / Printable Voucher Modal -->
    <LuxuryVoucherModal 
      v-model:isOpen="showVoucherModal" 
      :booking="activeBookingForVoucher" 
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '@/features/auth/store/auth';
import { useCurrencyStore } from '@/store/currency';
import LuxuryVoucherModal from '@/shared/components/LuxuryVoucherModal.vue';

const router = useRouter();
const { locale } = useI18n();
const authStore = useAuthStore();
const currencyStore = useCurrencyStore();
const showVoucherModal = ref(false);

const allTours = ref<any[]>([]);
const favoritedTours = computed(() => {
  return allTours.value.filter(t => authStore.isFavorite(t.id));
});

function getTourTitle(tour: any) {
  if (tour?.names && tour.names[locale.value]) {
    return tour.names[locale.value];
  }
  if (tour?.names && tour.names['en']) {
    return tour.names['en'];
  }
  return tour?.title || 'Luxury Experience';
}

const customerFirstName = computed(() => {
  if (!authStore.user || !authStore.user.name) return 'Guest';
  return authStore.user.name.split(' ')[0];
});

const weatherTemp = ref<number | null>(null);
const weatherDescription = ref<string>('Clear Sky & Gentle Breeze');

const activeBookingForVoucher = ref({
  bookingReference: 'BK-10293',
  tourTitle: '7-Night Private Dahabiya Expedition: Luxor to Aswan',
  departureDate: '2026-10-30',
  returnDate: '2026-11-06',
  pickupHotel: 'Sofitel Winter Palace Luxor',
  roomManifest: 'Royal Suite 01 (Mr. & Mrs. Sterling)',
  totalPaid: 3600,
  currency: 'EUR',
  emergencyPhone: '+20 106 894 0967'
});

const fetchLiveWeather = async () => {
  try {
    const res = await fetch('https://api.open-meteo.com/v1/forecast?latitude=25.6872&longitude=32.6396&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m');
    if (res.ok) {
      const data = await res.json();
      if (data && data.current) {
        weatherTemp.value = Math.round(data.current.temperature_2m);
        const code = data.current.weather_code;
        if (code === 0) weatherDescription.value = 'Clear Sunny Sky';
        else if (code <= 3) weatherDescription.value = 'Partly Cloudy & Warm';
        else if (code <= 48) weatherDescription.value = 'Gentle Mist & Breeze';
        else weatherDescription.value = 'Pleasant Nile Climate';
      }
    }
  } catch (e) {
    console.warn('Weather API fallback', e);
    weatherTemp.value = 29;
    weatherDescription.value = 'Sunny & Gentle Nile Breeze';
  }
};

onMounted(async () => {
  fetchLiveWeather();
  try {
    const res = await fetch('/api/content/api/tours');
    if (res.ok) {
      allTours.value = await res.json();
    }
  } catch (err) {
    console.warn('Failed to load tours for portal dashboard', err);
  }
});
</script>