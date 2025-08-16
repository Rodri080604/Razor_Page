// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const content = document.getElementById("content");
    const toggleBtn = document.getElementById("sidebarToggle");

    toggleBtn.addEventListener("click", function () {
        sidebar.classList.toggle("collapsed");
    });
});

