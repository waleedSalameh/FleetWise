using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class UpdateExportAqabaBookingDto
    {
        [Required(ErrorMessage = "Please enter the booking number.")]
        [MaxLength(100)]
        public string BookingNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select an active customer.")]
        public int CustomerId { get; set; }

        public BillStatus Status { get; set; }
    }
}
