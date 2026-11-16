using FleetWise.DAL.Entities.Customers;
using FleetWise.DAL.Enums;

namespace FleetWise.DAL.Entities.Containers.ExportAqaba
{
    public class ExportAqabaBooking
    {
        public int Id { get; set; }

        public string BookingNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string CustomerName { get; set; } = string.Empty;

        public int ContainerCount { get; set; }

        public BillStatus Status { get; set; } = BillStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false;

        public ICollection<ExportAqabaContainer> Containers { get; set; } = new List<ExportAqabaContainer>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
