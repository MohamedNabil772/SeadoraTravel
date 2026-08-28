<template>
  <div class="max-w-2xl space-y-6">
    <h1 class="text-2xl font-bold mb-6">Profile & Preferences</h1>
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
      <div class="pt-4 border-t border-white/10">
        <label class="flex items-center gap-3 cursor-pointer">
          <input type="checkbox" class="w-5 h-5 rounded border-white/10 bg-[#041a2e] text-[#c9a84c] focus:ring-[#c9a84c] focus:ring-offset-[#062d4d]" checked />
          <span class="text-sm">I agree to receive marketing updates and exclusive offers (GDPR Compliant)</span>
        </label>
      </div>
      <button class="w-full md:w-auto px-6 py-2.5 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-lg hover:shadow-[#c9a84c]/20 transition-all">Save Changes</button>
    </div>
  </div>
</template>