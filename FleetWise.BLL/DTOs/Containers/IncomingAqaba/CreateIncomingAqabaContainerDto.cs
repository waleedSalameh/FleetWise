using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class CreateIncomingAqabaContainerDto
    {
        [Required(ErrorMessage = " Please enter the container number.")]
        [MaxLength(50)]
        public string ContainerNumber { get; set; } = string.Empty;

        public ContainerType ContainerType { get; set; }

        public ContainerSize ContainerSize { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a shipping line.")]
        public int ShippingLineId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a delivery location.")]
        public int DeliveryLocationId { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal WeightInTons { get; set; }

        public DateOnly VesselArrivalDate { get; set; }

        public DateOnly PortFreeTimeEndDate { get; set; }

        public DateOnly EmptyReturnFreeTimeEndDate { get; set; }

        public DateOnly? ContainerLoadingDate { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public AqabaContainerStatus Status { get; set; }
    }
}
