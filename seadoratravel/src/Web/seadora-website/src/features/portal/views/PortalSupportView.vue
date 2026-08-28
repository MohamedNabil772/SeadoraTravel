<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold">Support Tickets</h1>
      <button class="px-4 py-2 bg-gradient-to-r from-[#c9a84c] to-[#a38030] text-[#062d4d] font-bold rounded-xl shadow-lg shadow-[#c9a84c]/20">+ New Request</button>
    </div>
    <div class="bg-[#062d4d]/40 backdrop-blur-md rounded-2xl border border-white/10 overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-white/5 text-xs uppercase tracking-wider text-white/60">
            <th class="p-4 font-medium">Ticket ID</th>
            <th class="p-4 font-medium">Subject</th>
            <th class="p-4 font-medium">Status</th>
            <th class="p-4 font-medium text-right">Action</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-white/5">
          <tr class="hover:bg-white/5 transition-colors">
            <td class="p-4 text-sm text-[#c9a84c]">#REQ-8943</td>
            <td class="p-4 font-medium">Special Anniversary Arrangement</td>
            <td class="p-4"><span class="px-2 py-1 text-[10px] font-bold rounded bg-blue-500/20 text-blue-300">IN PROGRESS</span></td>
            <td class="p-4 text-right"><button class="text-xs text-white/50 hover:text-white">View Thread</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>