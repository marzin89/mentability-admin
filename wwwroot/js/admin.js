'use strict';

/* Funktionerna nedan markerar obligatoriska inmatningsfält om användaren
    inte har fyllt i dem */
const articleInputs  = document.getElementsByClassName('article-input');
const activityInputs = document.getElementsByClassName('activity-input');
const quoteInputs    = document.getElementsByClassName('quote-input');

const submitArticle  = document.getElementById('submit-article');
const submitActivity = document.getElementById('submit-activity');
const submitQuote    = document.getElementById('submit-quote');

submitArticle.addEventListener('click', validateArticle);
submitActivity.addEventListener('click', validateActivity);
submitQuote.addEventListener('click', validateQuote);

function validateArticle(e) {
    e.preventDefault();

    for (let i = 0; i < articleInputs.length; i++) {
        if (!articleInputs[i].value) {
            articleInputs[i].className = 'text-input-error article-input';

        } else {
            articleInputs[i].className = 'text-input article-input';
        }
    }
}

function validateActivity(e) {
    e.preventDefault();
    console.log('foo');

    for (let i = 0; i < activityInputs.length; i++) {
        if (activityInputs[i].id == 'activities-end-date-input') {
            continue;

        } else if (!activityInputs[i].value) {
            activityInputs[i].className = 'text-input-error activity-input';

        } else {
            activityInputs[i].className = 'text-input activity-input';
        }
    }
}

function validateQuote(e) {
    e.preventDefault();

    for (let i = 0; i < quoteInputs.length; i++) {
        if (!quoteInputs[i].value) {
            quoteInputs[i].className = 'text-input-error quote-input';

        } else {
            quoteInputs[i].className = 'text-input quote-input';
        }
    }
}