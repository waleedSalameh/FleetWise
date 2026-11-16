using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class ExportAqabaContainerResponseDto
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public string ContainerNumber { get; set; } = string.Empty;

        public ContainerSize ContainerSize { get; set; }

        public ContainerType ContainerType { get; set; }

        public int LoadingLocationId { get; set; }

        public string LoadingLocationName { get; set; } = string.Empty;

        public ExportAqabaRouteStatus RouteStatus { get; set; }

        public DateTime WithdrawalDate { get; set; }

        public DateTime LoadingDate { get; set; }

        public int DeliveryLocationId { get; set; }

        public string DeliveryLocationName { get; set; } = string.Empty;

        public decimal WeightInTons { get; set; }

        public string? Notes { get; set; }
    }
}
