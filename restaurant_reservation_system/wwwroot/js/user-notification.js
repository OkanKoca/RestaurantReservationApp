// User Notification System - SignalR Client

(function () {
    // Add CSS animations dynamically
    var style = document.createElement('style');
    style.textContent = [
        '.user-toast-slide-in { animation: userToastSlideIn 0.4s ease; }',
        '.user-toast-slide-out { animation: userToastSlideOut 0.3s ease forwards; }',
        '@keyframes userToastSlideIn { from { transform: translateY(-120%); opacity: 0; } to { transform: translateY(0); opacity: 1; } }',
        '@keyframes userToastSlideOut { from { transform: translateY(0); opacity: 1; } to { transform: translateY(-120%); opacity: 0; } }'
    ].join(' ');
    document.head.appendChild(style);
})();

var UserNotification = {
    connection: null,
    container: null,
    userId: null,

    init: function (userId) {
        this.userId = userId;
        this.createContainer();
        this.connect();
    },

    createContainer: function () {
        // Toast container oluþtur (sað üst)
        this.container = document.createElement('div');
        this.container.id = 'userNotificationContainer';
        this.container.style.cssText =
            'position: fixed;' +
            'top: 80px;' +
            'right: 20px;' +
            'z-index: 9999;' +
            'display: flex;' +
            'flex-direction: column;' +
            'gap: 10px;' +
            'max-width: 380px;';
        document.body.appendChild(this.container);
    },

    connect: function () {
        var self = this;

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7284/userNotificationHub")
            .withAutomaticReconnect()
            .build();

        // Rezervasyon durumu deðiþtiðinde
        this.connection.on("ReservationStatusChanged", function (data) {
            console.log("Reservation status changed:", data);
            self.showStatusNotification(data);
        });

        // Baðlantýyý baþlat
        this.connection.start()
            .then(function () {
                console.log("UserNotificationHub connected");
                // Kullanýcý grubuna katýl
                if (self.userId) {
                    self.connection.invoke("JoinUserGroup", self.userId.toString())
                        .then(function () {
                            console.log("Joined user group: user_" + self.userId);
                        })
                        .catch(function (err) {
                            console.error("Failed to join user group:", err);
                        });
                }
            })
            .catch(function (err) {
                console.error("UserNotificationHub connection error:", err);
            });

        // Baðlantý kesildiðinde
        this.connection.onclose(function () {
            console.log("UserNotificationHub disconnected");
        });

        // Yeniden baðlandýðýnda
        this.connection.onreconnected(function () {
            console.log("UserNotificationHub reconnected");
            if (self.userId) {
                self.connection.invoke("JoinUserGroup", self.userId.toString());
            }
        });
    },

    showStatusNotification: function (data) {
        var isConfirmed = data.newStatus === 'Confirmed';
        var bgColor = isConfirmed
            ? 'linear-gradient(135deg, #27ae60, #2ecc71)'
            : 'linear-gradient(135deg, #f39c12, #e67e22)';
        var icon = isConfirmed ? 'fa-check-circle' : 'fa-clock';
        var title = isConfirmed ? 'Rezervasyon Onay' : 'Rezervasyon Beklemede';

        this.showToast(bgColor, icon, title, data.message, data.reservationDate, data.reservationHour);
    },

    showToast: function (bgColor, icon, title, message, date, hour) {
        var toast = document.createElement('div');
        toast.className = 'user-toast-slide-in';
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
            '<i class="fas ' + icon + '" style="font-size: 1.4rem;"></i>' +
            '</div>' +
            '<div style="flex: 1;">' +
            '<div style="font-weight: 700; font-size: 1rem; margin-bottom: 6px;">' + title + '</div>' +
            '<div style="font-size: 0.9rem; opacity: 0.95; margin-bottom: 10px;">' + message + '</div>' +
            '<div style="display: flex; flex-wrap: wrap; gap: 8px;">' +
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

        // 10 saniye sonra otomatik kapat
        setTimeout(function () {
            toast.className = 'user-toast-slide-out';
            setTimeout(function () { toast.remove(); }, 300);
        }, 10000);
    },

    disconnect: function () {
        if (this.connection) {
            if (this.userId) {
                this.connection.invoke("LeaveUserGroup", this.userId.toString());
            }
            this.connection.stop();
        }
    }
};
