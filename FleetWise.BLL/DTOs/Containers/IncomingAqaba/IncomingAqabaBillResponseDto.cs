using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class IncomingAqabaBillResponseDto
    {
        public int Id { get; set; }

        public string BillNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? CustomerPhoneNumber { get; set; }

        public int ContainerCount { get; set; }

        public BillStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<IncomingAqabaContainerResponseDto> Containers { get; set; } = new();
    }
}
