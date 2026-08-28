<template>
  <div class="max-w-2xl space-y-6 pb-12">
    <h1 class="text-2xl font-bold mb-6">Profile & Preferences</h1>
    
    <!-- Profile Form -->
    <div class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 p-6 space-y-6">
      <div class="grid grid-cols-2 gap-4">
        <div>
          <label class="block text-xs font-medium text-white/60 mb-1">First Name</label>
          <input type="text" class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-2.5 text-white focus:border-[#c9a84c] focus:outline-none" value="John" />
        </div>
        <div>
          <label class="block text-xs font-medium text-white/60 mb-1">Last Name</label>
          <input type="text" class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-2.5 text-white focus:border-[#c9a84c] focus:outline-none" value="Doe" />
        </div>
      </div>
      <div>
        <label class="block text-xs font-medium text-white/60 mb-1">Dietary Requirements</label>
        <select class="w-full bg-[#041a2e] border border-white/10 rounded-xl px-4 py-2.5 text-white focus:border-[#c9a84c] focus:outline-none">
          <option>None</option>
          <option>Vegetarian</option>
          <option>Vegan</option>
          <option>Halal</option>
        </select>
      </div>
      <button class="w-full md:w-auto px-6 py-2.5 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-lg hover:shadow-[#c9a84c]/20 transition-all">Save Profile Changes</button>
    </div>

    <!-- GDPR & Privacy Consent Card -->
    <div class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 p-6 space-y-6">
      <div>
        <h2 class="text-xl font-bold mb-1">Privacy & Consent (GDPR)</h2>
        <p class="text-sm text-white/60 mb-4">Manage how we use your data to improve your luxury experience.</p>
      </div>

      <div class="space-y-4">
        <label class="flex items-start gap-3 cursor-pointer p-4 bg-[#041a2e] rounded-xl border border-white/5 hover:border-white/10 transition-colors">
          <input type="checkbox" class="mt-0.5 w-5 h-5 rounded border-white/10 bg-[#041a2e] text-[#c9a84c] focus:ring-[#c9a84c] focus:ring-offset-[#062d4d]" checked />
          <div class="flex flex-col">
            <span class="text-sm font-medium">Marketing Communications</span>
            <span class="text-xs text-white/50">Receive exclusive offers, luxury travel inspiration, and newsletter updates.</span>
          </div>
        </label>

        <label class="flex items-start gap-3 cursor-pointer p-4 bg-[#041a2e] rounded-xl border border-white/5 hover:border-white/10 transition-colors">
          <input type="checkbox" class="mt-0.5 w-5 h-5 rounded border-white/10 bg-[#041a2e] text-[#c9a84c] focus:ring-[#c9a84c] focus:ring-offset-[#062d4d]" checked />
          <div class="flex flex-col">
            <span class="text-sm font-medium">Personalized Concierge Data</span>
            <span class="text-xs text-white/50">Allow our VIP concierge to use your previous bookings to curate bespoke recommendations.</span>
          </div>
        </label>

        <label class="flex items-start gap-3 cursor-pointer p-4 bg-[#041a2e] rounded-xl border border-white/5 hover:border-white/10 transition-colors">
          <input type="checkbox" class="mt-0.5 w-5 h-5 rounded border-white/10 bg-[#041a2e] text-[#c9a84c] focus:ring-[#c9a84c] focus:ring-offset-[#062d4d]" checked disabled />
          <div class="flex flex-col">
            <span class="text-sm font-medium text-white/80">Essential Data Processing</span>
            <span class="text-xs text-white/50">Required for booking functionality, Coast Guard permits, and security clearance.</span>
          </div>
        </label>
      </div>

      <div class="pt-6 border-t border-white/10 flex flex-col sm:flex-row gap-3">
        <button class="px-6 py-2.5 bg-white/5 hover:bg-white/10 text-white font-medium rounded-xl border border-white/10 transition-all flex items-center justify-center gap-2">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"></path></svg>
          Export My Data
        </button>
        <button class="px-6 py-2.5 bg-red-500/10 hover:bg-red-500/20 text-red-400 font-medium rounded-xl border border-red-500/20 transition-all flex items-center justify-center gap-2">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"></path></svg>
          Request Data Deletion
        </button>
      </div>
    </div>
  </div>
</template>