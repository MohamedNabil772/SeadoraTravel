<script setup lang="ts">
import Navbar from '../components/Navbar.vue'
import Hero from '../components/Hero.vue'
import Destinations from '../components/Destinations.vue'
import Trips from '../components/Trips.vue'
import WhyChoose from '../components/WhyChoose.vue'
import Testimonials from '../components/Testimonials.vue'
import Contact from '../components/Contact.vue'
import Footer from '../components/Footer.vue'
import { onMounted } from 'vue'

onMounted(() => {
  // 1. Header Observer for elegant staggered section headers
  const headerObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        entry.target.classList.add('revealed')
        headerObserver.unobserve(entry.target)
      }
    })
  }, { threshold: 0.1, rootMargin: '0px 0px -50px 0px' })

  // Observe all section headers
  document.querySelectorAll('.section-header').forEach(el => {
    el.classList.add('reveal-header')
    headerObserver.observe(el)
  })

  // 2. Grid items/cards Observer for premium staggered reveals
  const cardObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        const grid = entry.target as HTMLElement
        const cards = grid.querySelectorAll('.dest-card, .trip-card, .why-card, .test-card, .contact-left, .contact-right')
        
        cards.forEach((card, idx) => {
          setTimeout(() => {
            card.classList.add('revealed')
          }, idx * 120) // Premium 120ms staggered delay
        })
        
        cardObserver.unobserve(grid)
      }
    })
  }, { threshold: 0.05, rootMargin: '0px 0px -80px 0px' })

  // Observe all content grids and wrappers
  document.querySelectorAll('.dest-grid, .trips-grid, .why-grid, .testimonials-grid, .contact-wrapper').forEach(grid => {
    const cards = grid.querySelectorAll('.dest-card, .trip-card, .why-card, .test-card, .contact-left, .contact-right')
    cards.forEach(card => {
      card.classList.add('reveal-card')
    })
    cardObserver.observe(grid)
  })
})
</script>

<template>
  <div>
    <Navbar />
    <Hero />
    <Destinations />
    <Trips />
    <WhyChoose />
    <Testimonials />
    <Contact />
    <Footer />
  </div>
</template>

<style>
html {
  scroll-behavior: smooth;
}

/* --- LUXURIOUS SCROLL REVEAL TRANSITIONS --- */

/* 1. Section Header elements (eyebrow, title, subtitle) */
.reveal-header .section-eyebrow,
.reveal-header h2,
.reveal-header .section-sub {
  opacity: 0;
  transform: translateY(24px);
  transition: opacity 1.2s cubic-bezier(0.16, 1, 0.3, 1), transform 1.2s cubic-bezier(0.16, 1, 0.3, 1);
  will-change: transform, opacity;
}

/* Stagger header elements */
.reveal-header.revealed .section-eyebrow {
  opacity: 1;
  transform: translateY(0);
  transition-delay: 0s;
}
.reveal-header.revealed h2 {
  opacity: 1;
  transform: translateY(0);
  transition-delay: 0.15s;
}
.reveal-header.revealed .section-sub {
  opacity: 1;
  transform: translateY(0);
  transition-delay: 0.3s;
}

/* 2. Grid Cards & Items reveal setup */
.reveal-card {
  opacity: 0;
  transform: translateY(50px) scale(0.97);
  transition: opacity 1.4s cubic-bezier(0.16, 1, 0.3, 1), transform 1.4s cubic-bezier(0.16, 1, 0.3, 1), border-color 0.4s ease, box-shadow 0.4s ease, background 0.4s ease;
  will-change: transform, opacity;
}

/* Revealed state (resets scale and Y translation) */
.reveal-card.revealed {
  opacity: 1;
  transform: translateY(0) scale(1);
}

/* Premium Hovers for Revealed items to override baseline without specificity/!important conflicts */
.dest-card.revealed:hover {
  transform: translateY(-8px) scale(1.015);
  box-shadow: 0 30px 60px rgba(201, 168, 76, 0.2);
}
.trip-card.revealed:hover {
  transform: translateY(-6px) scale(1.005);
  border-color: rgba(232,130,10,0.4);
  box-shadow: 0 20px 50px rgba(232, 130, 10, 0.15);
}
.why-card.revealed:hover {
  transform: translateY(-5px);
  box-shadow: 0 16px 50px rgba(0, 0, 0, 0.08);
}
.test-card.revealed:hover {
  transform: translateY(-4px);
  background: rgba(255, 255, 255, 0.1);
  border-color: rgba(201, 168, 76, 0.25);
}
.contact-right.revealed:hover {
  box-shadow: 0 25px 70px rgba(0,0,0,0.12);
}
</style>
