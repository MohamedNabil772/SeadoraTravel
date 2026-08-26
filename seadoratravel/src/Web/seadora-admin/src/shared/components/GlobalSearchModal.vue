<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/features/auth/store/auth'
import { Search, X, CornerDownLeft, Sparkles, Clock, Compass, Tag, Globe } from 'lucide-vue-next'
import api from '@/services/api'

const props = defineProps<{
  isOpen: boolean
}>()

const emit = defineEmits(['close', 'open-profile', 'logout'])

const auth = useAuthStore()
const router = useRouter()
const searchInput = ref<HTMLInputElement | null>(null)
const query = ref('')
const loading = ref(false)
const selectedIndex = ref(0)
const recentSearches = ref<string[]>([])

interface SearchItem {
  id: string
  title: string
  subtitle: string
  category: string
  icon: string
  badge?: string
  route: string
}

interface SearchGroup {
  name: string
  icon: any
  items: SearchItem[]
}

const rawResults = ref<{
  quickActions: SearchItem[]
  tours: SearchItem[]
  destinations: SearchItem[]
  categories: SearchItem[]
  tourTypes: SearchItem[]
}>({
  quickActions: [],
  tours: [],
  destinations: [],
  categories: [],
  tourTypes: []
})

// Filter actions based on role privileges
const filteredQuickActions = computed(() => {
  const isAdmin = auth.user?.roles?.includes('Admin') || auth.user?.roles?.includes('SuperAdmin')
  return rawResults.value.quickActions.filter(action => {
    if (action.route === '/users' || action.route === '/roles') {
      return isAdmin
    }
    return true
  })
})

// Flattened list for keyboard navigation
const flatResults = computed(() => {
  const list: SearchItem[] = []
  if (filteredQuickActions.value.length) list.push(...filteredQuickActions.value)
  if (rawResults.value.tours?.length) list.push(...rawResults.value.tours)
  if (rawResults.value.categories?.length) list.push(...rawResults.value.categories)
  if (rawResults.value.destinations?.length) list.push(...rawResults.value.destinations)
  if (rawResults.value.tourTypes?.length) list.push(...rawResults.value.tourTypes)
  return list
})

// Grouped for display
const groupedResults = computed<SearchGroup[]>(() => {
  const groups: SearchGroup[] = []

  if (filteredQuickActions.value.length) {
    groups.push({
      name: query.value.trim() ? 'Matching Actions & System Pages' : 'Quick Navigation',
      icon: Sparkles,
      items: filteredQuickActions.value
    })
  }

  if (rawResults.value.tours?.length) {
    groups.push({
      name: 'Tours & Experiences',
      icon: Compass,
      items: rawResults.value.tours
    })
  }

  if (rawResults.value.categories?.length) {
    groups.push({
      name: 'Categories',
      icon: Tag,
      items: rawResults.value.categories
    })
  }

  if (rawResults.value.destinations?.length) {
    groups.push({
      name: 'Destinations',
      icon: Globe,
      items: rawResults.value.destinations
    })
  }

  if (rawResults.value.tourTypes?.length) {
    groups.push({
      name: 'Tour & Trip Types',
      icon: Sparkles,
      items: rawResults.value.tourTypes
    })
  }

  return groups
})

let debounceTimer: any = null

async function executeSearch(q: string) {
  loading.value = true
  try {
    const res = await api.get(`/api/content/api/admin/search?q=${encodeURIComponent(q)}`)
    rawResults.value = res.data || { quickActions: [], tours: [], destinations: [], categories: [], tourTypes: [] }
    selectedIndex.value = 0
  } catch (e) {
    console.error('Search error', e)
  } finally {
    loading.value = false
  }
}

watch(query, (newVal) => {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    executeSearch(newVal)
  }, 150)
})

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    query.value = ''
    selectedIndex.value = 0
    executeSearch('')
    nextTick(() => {
      searchInput.value?.focus()
    })
  }
})

function handleKeydown(e: KeyboardEvent) {
  if (!props.isOpen) return

  if (e.key === 'ArrowDown') {
    e.preventDefault()
    if (flatResults.value.length > 0) {
      selectedIndex.value = (selectedIndex.value + 1) % flatResults.value.length
      scrollToSelected()
    }
  } else if (e.key === 'ArrowUp') {
    e.preventDefault()
    if (flatResults.value.length > 0) {
      selectedIndex.value = (selectedIndex.value - 1 + flatResults.value.length) % flatResults.value.length
      scrollToSelected()
    }
  } else if (e.key === 'Enter') {
    e.preventDefault()
    const selected = flatResults.value[selectedIndex.value]
    if (selected) {
      navigateTo(selected)
    }
  } else if (e.key === 'Escape') {
    emit('close')
  }
}

function scrollToSelected() {
  nextTick(() => {
    const activeEl = document.querySelector('.search-item-selected')
    if (activeEl) {
      activeEl.scrollIntoView({ block: 'nearest' })
    }
  })
}

function navigateTo(item: SearchItem) {
  if (query.value.trim() && !recentSearches.value.includes(query.value.trim())) {
    recentSearches.value = [query.value.trim(), ...recentSearches.value.slice(0, 4)]
    localStorage.setItem('admin_recent_searches', JSON.stringify(recentSearches.value))
  }
  emit('close')
  if (item.route === '/profile') {
    emit('open-profile')
  } else if (item.route === '/logout') {
    emit('logout')
  } else {
    router.push(item.route)
  }
}

function selectRecent(s: string) {
  query.value = s
  searchInput.value?.focus()
}

function clearRecentSearches() {
  recentSearches.value = []
  localStorage.removeItem('admin_recent_searches')
}

onMounted(() => {
  try {
    const saved = localStorage.getItem('admin_recent_searches')
    if (saved) recentSearches.value = JSON.parse(saved)
  } catch {}

  window.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeydown)
})
</script>

<template>
  <Teleport to="body">
    <Transition name="spotlight-modal">
      <div v-if="isOpen" class="fixed inset-0 z-[9999] flex items-start justify-center p-4 sm:p-6 pt-[10vh] sm:pt-[12vh]">
        <!-- Backdrop -->
        <div class="fixed inset-0 bg-navy-950/60 backdrop-blur-md transition-opacity" @click="emit('close')"></div>

        <!-- Spotlight Card -->
        <div 
          class="relative w-full max-w-2xl bg-white rounded-2xl shadow-[0_25px_70px_rgba(0,0,0,0.35)] border border-gray-100 overflow-hidden flex flex-col z-10 animate-spotlight"
          @click.stop
          role="dialog"
          aria-modal="true"
          aria-label="Global search"
          v-dialog="() => emit('close')"
        >
          <!-- Search Header Input -->
          <div class="flex items-center px-4 py-3.5 border-b border-gray-100 bg-white">
            <Search class="w-5 h-5 text-gray-400 mr-3 flex-shrink-0" aria-hidden="true" />
            <input
              ref="searchInput"
              v-model="query"
              type="text"
              data-autofocus
              role="combobox"
              aria-controls="global-search-results"
              aria-expanded="true"
              :aria-activedescendant="flatResults.length ? `global-search-option-${selectedIndex}` : undefined"
              aria-label="Search tours, bookings, categories and settings"
              placeholder="Search tours, bookings, categories, settings, or jump to page..."
              class="w-full text-base bg-transparent border-none text-gray-900 placeholder-gray-400 focus:outline-none focus:ring-0 font-sans"
            />
            <div v-if="loading" class="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin mr-2"></div>
            <button 
              v-if="query" 
              type="button"
              @click="query = ''" 
              aria-label="Clear search"
              class="p-1 text-gray-400 hover:text-gray-600 rounded-md hover:bg-gray-100 transition-colors"
            >
              <X class="w-4 h-4" />
            </button>
            <kbd class="hidden sm:inline-flex items-center gap-0.5 px-2 py-1 text-[10px] font-mono font-medium text-gray-400 bg-gray-100 border border-gray-200 rounded-md ml-2">
              ESC
            </kbd>
          </div>

          <!-- Results & Navigation Area -->
          <div id="global-search-results" role="listbox" aria-label="Search results" class="max-h-[60vh] overflow-y-auto p-3 space-y-4 no-scrollbar divide-y divide-gray-100/60">
            <!-- Recent Searches Pill list (when input empty) -->
            <div v-if="!query && recentSearches.length > 0" class="pb-2">
              <div class="flex items-center justify-between px-2 mb-2">
                <span class="text-[11px] font-bold text-gray-400 uppercase tracking-wider flex items-center gap-1.5">
                  <Clock class="w-3 h-3" /> Recent Searches
                </span>
                <button 
                  type="button"
                  @click="clearRecentSearches" 
                  class="text-[11px] text-gray-400 hover:text-red-500 transition-colors"
                >
                  Clear
                </button>
              </div>
              <div class="flex flex-wrap gap-1.5 px-2">
                <button
                  v-for="s in recentSearches"
                  :key="s"
                  type="button"
                  @click="selectRecent(s)"
                  class="px-2.5 py-1 text-xs bg-gray-50 hover:bg-gray-100 text-gray-600 rounded-lg border border-gray-200/80 transition-colors"
                >
                  {{ s }}
                </button>
              </div>
            </div>

            <!-- Grouped Sections -->
            <div 
              v-for="group in groupedResults" 
              :key="group.name"
              class="pt-3 first:pt-0"
            >
              <div class="flex items-center gap-1.5 px-3 py-1.5 text-[11px] font-bold text-gray-400 uppercase tracking-wider">
                <component :is="group.icon" class="w-3.5 h-3.5 text-secondary-text" />
                <span>{{ group.name }}</span>
              </div>

              <div class="mt-1 space-y-1" role="group" :aria-label="group.name">
                <div
                  v-for="item in group.items"
                  :key="item.id"
                  :id="`global-search-option-${flatResults.indexOf(item)}`"
                  role="option"
                  :aria-selected="flatResults.indexOf(item) === selectedIndex"
                  @click="navigateTo(item)"
                  @mouseenter="selectedIndex = flatResults.indexOf(item)"
                  class="flex items-center justify-between px-3.5 py-2.5 rounded-xl cursor-pointer transition-all duration-150 group"
                  :class="flatResults.indexOf(item) === selectedIndex 
                    ? 'bg-gradient-to-r from-navy-900 to-navy-800 text-white shadow-sm search-item-selected' 
                    : 'hover:bg-gray-50 text-gray-900'"
                >
                  <div class="flex items-center gap-3 min-w-0">
                    <div 
                      class="w-8 h-8 rounded-lg flex items-center justify-center text-sm flex-shrink-0 transition-colors"
                      :class="flatResults.indexOf(item) === selectedIndex 
                        ? 'bg-white/15 text-white' 
                        : 'bg-gray-100 text-gray-600 group-hover:bg-gray-200'"
                    >
                      <span>{{ item.icon }}</span>
                    </div>
                    <div class="min-w-0">
                      <div class="flex items-center gap-2">
                        <span 
                          class="font-medium text-sm truncate"
                          :class="flatResults.indexOf(item) === selectedIndex ? 'text-white' : 'text-gray-900'"
                        >
                          {{ item.title }}
                        </span>
                        <span 
                          v-if="item.badge" 
                          class="px-2 py-0.5 text-[10px] font-bold rounded-full uppercase"
                          :class="flatResults.indexOf(item) === selectedIndex ? 'bg-secondary text-navy-950' : 'bg-secondary/15 text-secondary-text border border-secondary/20'"
                        >
                          {{ item.badge }}
                        </span>
                      </div>
                      <p 
                        class="text-xs truncate mt-0.5"
                        :class="flatResults.indexOf(item) === selectedIndex ? 'text-white/70' : 'text-gray-400'"
                      >
                        {{ item.subtitle }}
                      </p>
                    </div>
                  </div>

                  <div 
                    class="flex items-center gap-1.5 opacity-0 group-hover:opacity-100 transition-opacity pl-2"
                    :class="{ '!opacity-100 text-secondary-text': flatResults.indexOf(item) === selectedIndex }"
                  >
                    <span class="text-[11px] font-medium hidden sm:inline">Open</span>
                    <CornerDownLeft class="w-3.5 h-3.5" />
                  </div>
                </div>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="!loading && query.trim() && flatResults.length === 0" class="py-12 text-center text-gray-400">
              <Search class="w-10 h-10 mx-auto mb-2 text-gray-300 stroke-1" />
              <p class="font-medium text-gray-700 text-sm">No results found for "{{ query }}"</p>
              <p class="text-xs text-gray-400 mt-1">Try searching by tour title, destination, category, or jump shortcut.</p>
            </div>
          </div>

          <!-- Footer Shortcut Hints -->
          <div class="px-4 py-2.5 bg-gray-50 border-t border-gray-100 flex items-center justify-between text-[11px] text-gray-400">
            <div class="flex items-center gap-3">
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-white border border-gray-200 rounded font-mono text-[10px] shadow-2xs">↑↓</kbd> Navigate
              </span>
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-white border border-gray-200 rounded font-mono text-[10px] shadow-2xs">↵</kbd> Select
              </span>
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-white border border-gray-200 rounded font-mono text-[10px] shadow-2xs">ESC</kbd> Close
              </span>
            </div>
            <div class="font-serif italic text-secondary-text text-xs">
              ✦ Seadora Luxury Search
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.spotlight-modal-enter-active,
.spotlight-modal-leave-active {
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

.spotlight-modal-enter-from,
.spotlight-modal-leave-to {
  opacity: 0;
  transform: scale(0.98) translateY(-8px);
}

.animate-spotlight {
  animation: spotlightEnter 0.22s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes spotlightEnter {
  from {
    opacity: 0;
    transform: scale(0.97) translateY(-10px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}
</style>
