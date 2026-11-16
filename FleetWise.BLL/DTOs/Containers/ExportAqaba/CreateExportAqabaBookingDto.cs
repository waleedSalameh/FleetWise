using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class CreateExportAqabaBookingDto
    {
        [Required(ErrorMessage = "Please enter the booking number.")]
        [MaxLength(100)]
        public string BookingNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select an active customer.")]
        public int CustomerId { get; set; }

        public List<CreateExportAqabaContainerDto> Containers { get; set; } = new();

        public BillStatus Status { get; set; } = BillStatus.New;
    }
}
