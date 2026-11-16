using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Containers.IncomingAqaba
{
    public class IncomingAqabaContainer
    {
        public int Id { get; set; }

        public int BillId { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;

        public ContainerType ContainerType { get; set; }

        public ContainerSize ContainerSize { get; set; }

        public int ShippingLineId { get; set; }

        public int DeliveryLocationId { get; set; }

        public decimal WeightInTons { get; set; }

        public DateOnly VesselArrivalDate { get; set; }

        public DateOnly PortFreeTimeEndDate { get; set; }

        public DateOnly EmptyReturnFreeTimeEndDate { get; set; }

        public DateOnly? ContainerLoadingDate { get; set; }

        public string? Notes { get; set; }

        public AqabaContainerStatus Status { get; set; }

        public bool IsDeleted { get; set; }

        // Navigation Properties
        public IncomingAqabaBill Bill { get; set; } = null!;

        public ShippingLine ShippingLine { get; set; } = null!;

        public DeliveryLocation DeliveryLocation { get; set; } = null!;
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
