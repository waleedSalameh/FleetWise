using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Containers.ExportAqaba
{
    public class ExportAqabaContainer
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public ExportAqabaBooking Booking { get; set; } = null!;
        public string ContainerNumber { get; set; } = string.Empty;
        public ContainerSize ContainerSize { get; set; }

        public ContainerType ContainerType { get; set; }
        public int LoadingLocationId { get; set; }
        public DeliveryLocation LoadingLocation { get; set; } = null!;
        public ExportAqabaRouteStatus RouteStatus { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public DateTime LoadingDate { get; set; }
        public int DeliveryLocationId { get; set; }
        public DeliveryLocation DeliveryLocation { get; set; } = null!;
        public decimal WeightInTons { get; set; }
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
