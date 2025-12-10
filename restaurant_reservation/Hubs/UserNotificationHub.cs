using Microsoft.AspNetCore.SignalR;

namespace restaurant_reservation_api.Hubs
{
    /// <summary>
    /// Kullanýcýlara özel bildirimler için SignalR Hub.
    /// Rezervasyon onayý, hatýrlatma gibi bildirimleri gönderir.
    /// </summary>
    public class UserNotificationHub : Hub
    {
        /// <summary>
        /// Kullanýcý baðlandýðýnda kendi ID'siyle bir gruba katýlýr.
        /// </summary>
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        /// <summary>
        /// Kullanýcý ayrýldýðýnda gruptan çýkar.
        /// </summary>
        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
    }
}
