using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class UpdateIncomingAqabaBillDto
    {
        [Required(ErrorMessage = "Please enter the policy number.")]
        [MaxLength(100)]
        public string BillNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select an active customer.")]
        public int CustomerId { get; set; }

        public BillStatus Status { get; set; }
    }
}
