using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class CreateIncomingAqabaBillDto
    {
        [Required(ErrorMessage = "Please enter the policy number.")]
        [MaxLength(100)]
        public string BillNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select an active customer.")]
        public int CustomerId { get; set; }

        public List<CreateIncomingAqabaContainerDto> Containers { get; set; } = new();

        public BillStatus Status { get; set; } = BillStatus.New;
    }
}
