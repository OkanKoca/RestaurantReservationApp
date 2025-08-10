using restaurant_reservation_system.Models.Dto;

namespace restaurant_reservation_system.Models.ViewModel
{
    public class MenuViewModel
    {
        public List<MenuDto> Menus { get; set; } = new();
        public MenuDto NewMenu { get; set; } = new();
    }
}
