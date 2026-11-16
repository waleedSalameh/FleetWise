using FleetWise.DAL.Entities.Customers;
using FleetWise.DAL.Enums;

namespace FleetWise.DAL.Entities.Containers.InternationalIncoming
{
    public class InternationalIncoming
    {
        public int Id { get; set; }

        public string CargoNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public Customer Customer { get; set; } = null!;

        public string Country { get; set; } = string.Empty;

        public InternationalCargoType CargoType { get; set; }

        public string LoadingLocation { get; set; } = string.Empty;

        public string FactoryName { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string? Notes { get; set; }

        public string CustomsDeclarationNumber { get; set; } = string.Empty;

        public int TripDuration { get; set; }

        public string UnloadingLocation { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public InternationalOrderStatus Status { get; set; }

        public bool IsPaid { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
