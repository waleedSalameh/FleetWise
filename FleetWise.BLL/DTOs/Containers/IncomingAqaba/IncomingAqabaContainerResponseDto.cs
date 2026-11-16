using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class IncomingAqabaContainerResponseDto
    {
        public int Id { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;

        public ContainerType ContainerType { get; set; }

        public ContainerSize ContainerSize { get; set; }

        public int ShippingLineId { get; set; }

        public string ShippingLineName { get; set; } = string.Empty;

        public int DeliveryLocationId { get; set; }

        public string DeliveryLocationName { get; set; } = string.Empty;

        public decimal WeightInTons { get; set; }

        public DateOnly VesselArrivalDate { get; set; }

        public DateOnly PortFreeTimeEndDate { get; set; }

        public DateOnly EmptyReturnFreeTimeEndDate { get; set; }

        public DateOnly? ContainerLoadingDate { get; set; }

        public string? Notes { get; set; }

        public AqabaContainerStatus Status { get; set; }
    }
}
