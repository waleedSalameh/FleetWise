using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Customers
{
    public class UpdateCustomerDto
    {
        [Required(ErrorMessage = "Please enter the customer name.")]
        [StringLength(200, MinimumLength = 1)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the customer phone number.")]
        [StringLength(20)]
        [RegularExpression(@"^\+?[0-9\s().-]{7,20}$", ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
