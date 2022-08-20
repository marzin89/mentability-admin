'use strict';

/* Funktionen nedan anpassar gränssnittet beroende på vilken kategori
    användaren väljer i menyn (nyheter, aktiviteter, citat) */
const sections = document.getElementsByClassName('admin-toggle-section');

window.addEventListener('load', () => {
    const activeSection = sessionStorage.getItem('adminSection');

    if (!activeSection || activeSection == 'news') {
        sections[0].style.display = 'block';
        sections[1].style.display = 'none';
        sections[2].style.display = 'none';

    } else if (activeSection == 'activities') {
        sections[0].style.display = 'none';
        sections[1].style.display = 'block';
        sections[2].style.display = 'none';

    } else if (activeSection == 'quotes') {
        sections[0].style.display = 'none';
        sections[1].style.display = 'none';
        sections[2].style.display = 'block';
    }
})

/* Funktionen nedan markerar obligatoriska inmatningsfält om användaren
    inte har fyllt i dem */
const inputs = document.getElementsByClassName('text-input');
const submit = document.getElementsByClassName('submit');

for (let i = 0; i < submit.length; i++) {
    submit[i].addEventListener('click', validate);
}

function validate(e) {
    e.preventDefault();

    for (let i = 0; i < inputs.length; i++) {
        if (inputs[i].id == 'activities-end-date-input') {
            continue;

        } else if (!inputs[i].value) {
            inputs[i].className = 'text-input-error';
        }
    }
}