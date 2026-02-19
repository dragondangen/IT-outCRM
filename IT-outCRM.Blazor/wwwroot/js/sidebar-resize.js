// Sidebar resize functionality
(function () {
    let isResizing = false;
    let startX = 0;
    let startWidth = 0;
    let sidebar = null;

    function initSidebarResize() {
        sidebar = document.getElementById('sidebar');
        if (!sidebar) return;

        const resizeHandle = sidebar.querySelector('.premium-sidebar-resize-handle');
        if (!resizeHandle) return;

        resizeHandle.addEventListener('mousedown', handleMouseDown);
        resizeHandle.addEventListener('touchstart', handleTouchStart, { passive: false });
    }

    function handleMouseDown(e) {
        if (sidebar.classList.contains('collapsed')) return;

        isResizing = true;
        startX = e.clientX;
        startWidth = sidebar.offsetWidth;

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);

        e.preventDefault();
    }

    function handleTouchStart(e) {
        if (sidebar.classList.contains('collapsed')) return;
        if (e.touches.length === 0) return;

        isResizing = true;
        startX = e.touches[0].clientX;
        startWidth = sidebar.offsetWidth;

        document.addEventListener('touchmove', handleTouchMove, { passive: false });
        document.addEventListener('touchend', handleTouchEnd);

        e.preventDefault();
    }

    function handleMouseMove(e) {
        if (!isResizing) return;

        const newWidth = startWidth + (e.clientX - startX);
        applyWidth(newWidth);
    }

    function handleTouchMove(e) {
        if (!isResizing) return;
        if (e.touches.length === 0) return;

        const newWidth = startWidth + (e.touches[0].clientX - startX);
        applyWidth(newWidth);
        e.preventDefault();
    }

    function applyWidth(newWidth) {
        if (newWidth >= 200 && newWidth <= 500) {
            sidebar.style.width = newWidth + 'px';
            sidebar.style.transition = 'none';
            localStorage.setItem('sidebarWidth', newWidth + 'px');
        }
    }

    function finishResize() {
        if (!isResizing) return;
        isResizing = false;
        if (sidebar) {
            sidebar.style.transition = '';
        }

        // Notify Blazor that the width changed so it can sync state
        if (window._sidebarDotNetRef) {
            try {
                window._sidebarDotNetRef.invokeMethodAsync('OnSidebarWidthChanged');
            } catch (e) { }
        }
    }

    function handleMouseUp() {
        finishResize();
        document.removeEventListener('mousemove', handleMouseMove);
        document.removeEventListener('mouseup', handleMouseUp);
    }

    function handleTouchEnd() {
        finishResize();
        document.removeEventListener('touchmove', handleTouchMove);
        document.removeEventListener('touchend', handleTouchEnd);
    }

    // Blazor interop: receive the DotNetObjectReference and set up window.resize handler
    window.initSidebarBlazorInterop = function (dotNetRef) {
        window._sidebarDotNetRef = dotNetRef;

        if (!window._windowResizeInitialized) {
            window._windowResizeInitialized = true;
            window.addEventListener('resize', function () {
                if (window._sidebarDotNetRef) {
                    try {
                        window._sidebarDotNetRef.invokeMethodAsync('HandleWindowResize');
                    } catch (e) { }
                }
            });
        }
    };

    window.disposeSidebarBlazorInterop = function () {
        window._sidebarDotNetRef = null;
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSidebarResize);
    } else {
        initSidebarResize();
    }

    // Re-initialize when sidebar is added dynamically (Blazor Server re-renders)
    const observer = new MutationObserver(function () {
        if (!sidebar || !document.getElementById('sidebar')) {
            initSidebarResize();
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
})();
