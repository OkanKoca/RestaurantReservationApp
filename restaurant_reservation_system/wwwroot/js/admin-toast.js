// Admin Toast Notification System

(function () {
    // Add CSS animations dynamically
    var style = document.createElement('style');
    style.textContent = [
        '.toast-slide-in { animation: toastSlideIn 0.4s ease; }',
        '.toast-slide-out { animation: toastSlideOut 0.3s ease forwards; }',
        '@keyframes toastSlideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }',
        '@keyframes toastSlideOut { from { transform: translateX(0); opacity: 1; } to { transform: translateX(120%); opacity: 0; } }'
    ].join(' ');
    document.head.appendChild(style);
})();

var AdminToast = {
    container: null,

    init: function () {
        this.container = document.getElementById('adminToastContainer');
    },

    show: function (type, title, customerName, table, date, hour) {
        if (!this.container) this.init();

        var toast = document.createElement('div');

        var isSuccess = type === 'success';
        var bgColor = isSuccess
            ? 'linear-gradient(135deg, #27ae60, #2ecc71)'
            : 'linear-gradient(135deg, #e74c3c, #c0392b)';
        var icon = isSuccess ? 'fa-calendar-check' : 'fa-calendar-xmark';

        toast.className = 'toast-slide-in';
        toast.style.cssText =
            'background: ' + bgColor + ';' +
            'color: white;' +
            'padding: 16px 20px;' +
            'border-radius: 16px;' +
            'box-shadow: 0 8px 30px rgba(0,0,0,0.2);' +
            'display: flex;' +
            'align-items: flex-start;' +
            'gap: 14px;';

        toast.innerHTML =
            '<div style="width: 44px; height: 44px; background: rgba(255,255,255,0.2); border-radius: 12px; display: flex; align-items: center; justify-content: center; flex-shrink: 0;">' +
            '<i class="fas ' + icon + '" style="font-size: 1.2rem;"></i>' +
            '</div>' +
            '<div style="flex: 1;">' +
            '<div style="font-weight: 700,700; font-size: 1rem; margin-bottom: 6px;">' + title + '</div>' +
            '<div style="font-size: 0.9rem; opacity: 0.95; margin-bottom: 10px;">' +
            '<i class="fas fa-user" style="margin-right: 6px;"></i>' + customerName +
            '</div>' +
            '<div style="display: flex; flex-wrap: wrap; gap: 8px;">' +
            '<span style="background: rgba(255,255,255,0.2); padding: 4px 12px; border-radius: 20px; font-size: 0.8rem;">' +
            '<i class="fas fa-chair" style="margin-right: 5px;"></i>Table ' + table +
            '</span>' +
            '<span style="background: rgba(255,255,255,0.2); padding: 4px 12px; border-radius: 20px; font-size: 0.8rem;">' +
            '<i class="fas fa-calendar" style="margin-right: 5px;"></i>' + date +
            '</span>' +
            '<span style="background: rgba(255,255,255,0.2); padding: 4px 12px; border-radius: 20px; font-size: 0.8rem;">' +
            '<i class="fas fa-clock" style="margin-right: 5px;"></i>' + hour +
            '</span>' +
            '</div>' +
            '</div>' +
            '<button onclick="this.parentElement.remove()" style="background: rgba(255,255,255,0.2); border: none; color: white; width: 28px; height: 28px; border-radius: 50%; cursor: pointer; display: flex; align-items: center; justify-content: center;">' +
            '<i class="fas fa-times"></i>' +
            '</button>';

        this.container.appendChild(toast);

        // Auto remove after 8 seconds
        setTimeout(function () {
            toast.className = 'toast-slide-out';
            setTimeout(function () { toast.remove(); }, 300);
        }, 8000);
    },

    newReservation: function (data) {
        var name = data.customerName || 'Customer';
        var table = data.reservationTable || 'N/A';
        var date = data.reservationDate || 'N/A';
        var hour = data.reservationHour || 'N/A';

        this.show('success', 'New Reservation', name, table, date, hour);
    },

    canceledReservation: function (data) {
        var name = data.customerName || data.fullName || 'Customer';
        var table = data.reservationTable || data.tableNumber || 'N/A';
        var date = data.reservationDate || data.date || 'N/A';
        var hour = data.reservationHour || data.hour || 'N/A';

        this.show('error', 'Reservation Canceled', name, table, date, hour);
    }
};

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function () {
    AdminToast.init();
});
