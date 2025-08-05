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
        [FutureDateTime<GuestReservationViewModel>]
        public string ReservationHour { get; set; }
    }

    public class FutureDateTimeAttribute<T> : ValidationAttribute where T : class
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as T;

            if (model == null)
                return new ValidationResult("Invalid model type");

            var reservationDateProperty = typeof(T).GetProperty("ReservationDate");
            if (reservationDateProperty == null)
                return new ValidationResult("Model must have ReservationDate property");

            var reservationDate = (DateTime)reservationDateProperty.GetValue(model)!;

            if (value is string selectedHour && reservationDate != default)
            {
                var selectedTime = TimeSpan.Parse(selectedHour);
                var selectedDateTime = reservationDate.Date.Add(selectedTime);

                if (selectedDateTime <= DateTime.Now)
                {
                    return new ValidationResult("Reservation date can't be in the past.");
                }
            }

            return ValidationResult.Success;
        }
    }

}

