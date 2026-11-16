using FleetWise.DAL.Entities.Customers;
using FleetWise.DAL.Enums;

namespace FleetWise.DAL.Entities.Containers.IncomingAqaba
{
    public class IncomingAqabaBill
    {
        public int Id { get; set; }

        public string BillNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string CustomerName { get; set; } = string.Empty;

        public int ContainerCount { get; set; }

        public BillStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }

        public ICollection<IncomingAqabaContainer> Containers { get; set; } = new List<IncomingAqabaContainer>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
