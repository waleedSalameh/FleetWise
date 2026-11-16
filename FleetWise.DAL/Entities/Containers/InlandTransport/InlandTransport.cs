using FleetWise.DAL.Entities.Customers;
using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Containers.InlandTransport
{
    public class InlandTransport
    {
        public int Id { get; set; }

        public string TripCode { get; set; } = string.Empty;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        public InlandTripStatus TripStatus { get; set; }

        public InternationalCargoType CargoType { get; set; }

        public decimal Weight { get; set; }

        public string LoadingLocation { get; set; } = string.Empty;

        public string UnloadingLocation { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
