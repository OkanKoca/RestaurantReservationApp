using System.ComponentModel.DataAnnotations;

namespace restaurant_reservation_system.Models.ViewModel
{
    public class GuestReservationViewModel
    {
        [Required]
        public required string FullName { get; set; }
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required, Phone]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public required string PhoneNumber { get; set; }

        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10.")]
        public int? NumberOfGuests { get; set; }
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        [Display(Name = "Reservation Hour")]
        [FutureDateTime]
        public string ReservationHour { get; set; }
    }

    public class FutureDateTimeAttribute : ValidationAttribute {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (GuestReservationViewModel)validationContext.ObjectInstance;

            if (value is string selectedHour && model.ReservationDate != default)
            {
                var selectedTime = TimeSpan.Parse(selectedHour);
                var selectedDateTime = model.ReservationDate.Date.Add(selectedTime);

                if (selectedDateTime <= DateTime.Now)
                {
                    return new ValidationResult("Reservation date can't be in the past.");
                }
            }

            return ValidationResult.Success;
        }
    }
         
}

