'use strict';

// Visar mobilnavigeringen vid klick på hamburgerikonen och sätter aria-expanded till true
const navIcon = document.getElementById('nav-icon');
const menu = document.getElementById('main-nav-mobile');

navIcon.addEventListener('click', () => {
    menu.style.display = 'block';
    navIcon.setAttribute('aria-expanded', true);
});

// Döljer mobilnavigeringen vid klick på länken och sätter aria-expanded till false
const closeLink = document.getElementById('close-menu-link');

closeLink.addEventListener('click', () => {
    menu.style.display = 'none';
    navIcon.setAttribute('aria-expanded', false);
})