// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var sidebarOpen = false;

function OpenCloseNav() {
    sidebarOpen = !sidebarOpen;

    if (sidebarOpen) {
        document.getElementById("nav-drawer").classList.replace('drawer-collapse', 'drawer-full');
        document.getElementById("body").classList.replace('drawer-collapse', 'drawer-full');
    }
    else {
        document.getElementById("nav-drawer").classList.replace('drawer-full', 'drawer-collapse');
        document.getElementById("body").classList.replace('drawer-full', 'drawer-collapse');
    }
}

function OpenNavHover() {
    document.getElementById("nav-drawer").classList.replace('drawer-full', 'drawer-collapse');
    document.getElementById("body").classList.replace('drawer-full', 'drawer-collapse');
    sidebarOpen = true;
}

function CloseNavHover() {
    document.getElementById("nav-drawer").classList.replace('drawer-collapse', 'drawer-full');
    document.getElementById("body").classList.replace('drawer-collapse', 'drawer-full');
    sidebarOpen = false;
}