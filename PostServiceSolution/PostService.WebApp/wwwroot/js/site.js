"use strict";
// Серверна валідація працює також без JavaScript.
document.querySelectorAll("[data-save-form]").forEach(form => {
    form.addEventListener("submit", () => {
        const button = form.querySelector('button[type="submit"]');
        if (button) { button.dataset.originalText = button.textContent; button.disabled = true; button.textContent = "Зачекайте…"; }
        form.setAttribute("aria-busy", "true");
    });
});
document.querySelectorAll(".input-validation-error").forEach(input => input.setAttribute("aria-invalid", "true"));
document.querySelector("[data-validation-summary]")?.focus();
// Після повернення кнопкою браузера форма знову доступна.
window.addEventListener("pageshow", () => {
    document.querySelectorAll("[data-save-form]").forEach(form => {
        form.removeAttribute("aria-busy");
        const button = form.querySelector('button[type="submit"]');
        if (button?.dataset.originalText) { button.disabled = false; button.textContent = button.dataset.originalText; }
    });
});
