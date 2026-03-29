// NexTrack - Client-side JavaScript

// Auto-dismiss alerts after 4 seconds
document.addEventListener('DOMContentLoaded', function () {
    var alerts = document.querySelectorAll('.alert-success-custom');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.style.transition = 'opacity 0.3s, transform 0.3s';
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-8px)';
            setTimeout(function () {
                alert.remove();
            }, 300);
        }, 4000);
    });
});
