<script setup lang="ts">
import Navbar from '../shared/components/Navbar.vue'
import Hero from '../shared/components/Hero.vue'
import Destinations from '../features/destinations/components/Destinations.vue'
import WhyChoose from '../shared/components/WhyChoose.vue'
import Testimonials from '../shared/components/Testimonials.vue'
import Contact from '../features/contact/components/Contact.vue'
import Footer from '../shared/components/Footer.vue'
</script>

<template>
  <div class="app-layout">
    <Navbar />
    
    <main class="main-content">
      <Hero />
      <Destinations />
      <WhyChoose />
      <Testimonials />
      <Contact />
    </main>

    <Footer />
  </div>
</template>

<style>
:root {
  --header-height: 80px;
  --footer-height: 56px;
}

@media (max-width: 768px) {
  :root {
    --header-height: 52px;
  }
}

html {
  scroll-behavior: smooth;
  scroll-padding-top: var(--header-height);
}

.app-layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  width: 100%;
}
</style>
