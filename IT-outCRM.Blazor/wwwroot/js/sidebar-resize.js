// Sidebar resize functionality
(function() {
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
        if (newWidth >= 200 && newWidth <= 500) {
            sidebar.style.width = newWidth + 'px';
            sidebar.style.transition = 'none'; // Отключаем анимацию во время изменения размера
            localStorage.setItem('sidebarWidth', newWidth + 'px');
        }
    }

    function handleTouchMove(e) {
        if (!isResizing) return;
        if (e.touches.length === 0) return;
        
        const newWidth = startWidth + (e.touches[0].clientX - startX);
        if (newWidth >= 200 && newWidth <= 500) {
            sidebar.style.width = newWidth + 'px';
            sidebar.style.transition = 'none'; // Отключаем анимацию во время изменения размера
            localStorage.setItem('sidebarWidth', newWidth + 'px');
        }
        
        e.preventDefault();
    }

    function handleMouseUp() {
        if (isResizing) {
            isResizing = false;
            sidebar.style.transition = ''; // Включаем анимацию обратно
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        }
    }

    function handleTouchEnd() {
        if (isResizing) {
            isResizing = false;
            sidebar.style.transition = ''; // Включаем анимацию обратно
            document.removeEventListener('touchmove', handleTouchMove);
            document.removeEventListener('touchend', handleTouchEnd);
        }
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSidebarResize);
    } else {
        initSidebarResize();
    }

    // Re-initialize when sidebar is added dynamically
    const observer = new MutationObserver(function(mutations) {
        if (!sidebar || !document.getElementById('sidebar')) {
            initSidebarResize();
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
})();

