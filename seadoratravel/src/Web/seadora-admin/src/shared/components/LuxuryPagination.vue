<script setup lang="ts">
import { computed } from 'vue'
import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from 'lucide-vue-next'

const props = withDefaults(
  defineProps<{
    currentPage: number
    totalItems: number
    pageSize?: number
    pageSizeOptions?: number[]
  }>(),
  {
    pageSize: 10,
    pageSizeOptions: () => [10, 25, 50, 100],
  }
)

const emit = defineEmits<{
  (e: 'update:currentPage', page: number): void
  (e: 'update:pageSize', size: number): void
  (e: 'pageChange', page: number): void
}>()

const totalPages = computed(() => Math.max(1, Math.ceil(props.totalItems / props.pageSize)))

const startItem = computed(() => (props.totalItems === 0 ? 0 : (props.currentPage - 1) * props.pageSize + 1))
const endItem = computed(() => Math.min(props.totalItems, props.currentPage * props.pageSize))

const pages = computed(() => {
  const current = props.currentPage
  const total = totalPages.value
  const delta = 2
  const range: (number | string)[] = []

  for (let i = Math.max(2, current - delta); i <= Math.min(total - 1, current + delta); i++) {
    range.push(i)
  }

  if (current - delta > 2) {
    range.unshift('...')
  }
  range.unshift(1)

  if (current + delta < total - 1) {
    range.push('...')
  }
  if (total > 1) {
    range.push(total)
  }

  return range
})

function setPage(page: number) {
  if (page < 1 || page > totalPages.value || page === props.currentPage) return
  emit('update:currentPage', page)
  emit('pageChange', page)
}

function handlePageSizeChange(e: Event) {
  const target = e.target as HTMLSelectElement
  const newSize = parseInt(target.value, 10)
  emit('update:pageSize', newSize)
  emit('update:currentPage', 1)
  emit('pageChange', 1)
}
</script>

<template>
  <div class="flex flex-col sm:flex-row items-center justify-between gap-4 py-4 px-6 bg-white border-t border-border/60 select-none">
    <!-- Counter / Info -->
    <div class="flex items-center gap-3 text-xs text-text-muted">
      <span>
        Showing <strong class="text-text-main font-semibold">{{ startItem }}</strong> to 
        <strong class="text-text-main font-semibold">{{ endItem }}</strong> of 
        <strong class="text-text-main font-semibold">{{ totalItems }}</strong> entries
      </span>

      <!-- Page Size Selector -->
      <div class="flex items-center gap-1.5 pl-3 border-l border-border/60">
        <label for="pageSizeSelect" class="text-[11px] text-text-muted">Show</label>
        <select
          id="pageSizeSelect"
          :value="pageSize"
          @change="handlePageSizeChange"
          class="bg-surface-sunken border border-border/70 rounded-md px-2 py-1 text-xs text-text-main focus:outline-none focus:ring-1 focus:ring-secondary/40 transition-colors cursor-pointer"
        >
          <option v-for="size in pageSizeOptions" :key="size" :value="size">
            {{ size }}
          </option>
        </select>
        <span class="text-[11px] text-text-muted">per page</span>
      </div>
    </div>

    <!-- Pagination Controls -->
    <div class="flex items-center gap-1.5" v-if="totalPages > 1">
      <!-- First Page Button -->
      <button
        @click="setPage(1)"
        :disabled="currentPage === 1"
        title="First Page"
        class="p-1.5 rounded-md text-text-muted hover:text-text-main hover:bg-surface-sunken disabled:opacity-30 disabled:pointer-events-none transition-colors"
      >
        <ChevronsLeft class="w-4 h-4" />
      </button>

      <!-- Previous Button -->
      <button
        @click="setPage(currentPage - 1)"
        :disabled="currentPage === 1"
        title="Previous Page"
        class="p-1.5 rounded-md text-text-muted hover:text-text-main hover:bg-surface-sunken disabled:opacity-30 disabled:pointer-events-none transition-colors"
      >
        <ChevronLeft class="w-4 h-4" />
      </button>

      <!-- Numbered Page Pills -->
      <div class="flex items-center gap-1 px-1">
        <template v-for="(p, index) in pages" :key="index">
          <span v-if="p === '...'" class="px-2 py-1 text-xs text-text-muted">...</span>
          <button
            v-else
            @click="setPage(p as number)"
            :class="[
              'min-w-[28px] h-7 px-2 text-xs font-semibold rounded-md transition-all duration-200 cursor-pointer',
              currentPage === p
                ? 'bg-secondary text-primary shadow-sm font-bold scale-105'
                : 'text-text-muted hover:text-text-main hover:bg-surface-sunken'
            ]"
          >
            {{ p }}
          </button>
        </template>
      </div>

      <!-- Next Button -->
      <button
        @click="setPage(currentPage + 1)"
        :disabled="currentPage === totalPages"
        title="Next Page"
        class="p-1.5 rounded-md text-text-muted hover:text-text-main hover:bg-surface-sunken disabled:opacity-30 disabled:pointer-events-none transition-colors"
      >
        <ChevronRight class="w-4 h-4" />
      </button>

      <!-- Last Page Button -->
      <button
        @click="setPage(totalPages)"
        :disabled="currentPage === totalPages"
        title="Last Page"
        class="p-1.5 rounded-md text-text-muted hover:text-text-main hover:bg-surface-sunken disabled:opacity-30 disabled:pointer-events-none transition-colors"
      >
        <ChevronsRight class="w-4 h-4" />
      </button>
    </div>
  </div>
</template>
