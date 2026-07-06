/* =====================================================================
   LRDE IT Services Portal — Core JavaScript
   Handles toasts, modals, sidebar toggling, and UI helpers
   ===================================================================== */

document.addEventListener('DOMContentLoaded', function() {
    initPortalUI();
});

// Re-initialize after ASP.NET UpdatePanel postbacks
if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function() {
        initPortalUI();
    });
    
    // Show loader on start request
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function() {
        showLoader();
    });
}

function initPortalUI() {
    hideLoader();
    initTooltips();
}

/* ── Loader ───────────────────────────────────────────────────────── */
function showLoader() {
    let loader = document.getElementById('portal-loader');
    if (!loader) {
        loader = document.createElement('div');
        loader.id = 'portal-loader';
        loader.className = 'loading-overlay';
        loader.innerHTML = '<div class="spinner-portal"></div>';
        document.body.appendChild(loader);
    }
    loader.classList.add('active');
}

function hideLoader() {
    const loader = document.getElementById('portal-loader');
    if (loader) loader.classList.remove('active');
}

/* ── Toast Notifications ───────────────────────────────────────────── */
function showToast(type, message, duration = 4000) {
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `portal-toast toast-${type}`;
    
    let icon = 'bi-info-circle';
    if (type === 'success') icon = 'bi-check-circle-fill';
    if (type === 'error') icon = 'bi-exclamation-triangle-fill';
    if (type === 'warning') icon = 'bi-exclamation-circle-fill';

    toast.innerHTML = `
        <i class="bi ${icon} toast-icon"></i>
        <div class="toast-body">${message}</div>
        <button class="toast-close"><i class="bi bi-x"></i></button>
    `;

    container.appendChild(toast);

    const closeBtn = toast.querySelector('.toast-close');
    let timeoutId;

    const closeToast = () => {
        toast.classList.add('hiding');
        setTimeout(() => {
            if (toast.parentNode) toast.parentNode.removeChild(toast);
        }, 300);
    };

    closeBtn.addEventListener('click', closeToast);
    
    if (duration > 0) {
        timeoutId = setTimeout(closeToast, duration);
        // Pause on hover
        toast.addEventListener('mouseenter', () => clearTimeout(timeoutId));
        toast.addEventListener('mouseleave', () => timeoutId = setTimeout(closeToast, duration));
    }
}

/* ── UI Helpers ────────────────────────────────────────────────────── */
function initTooltips() {
    // Basic tooltip initialization if needed
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (typeof bootstrap !== 'undefined') {
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

function toggleSidebar() {
    const sidebar = document.querySelector('.portal-sidebar');
    if (sidebar) {
        sidebar.classList.toggle('open');
    }
}
