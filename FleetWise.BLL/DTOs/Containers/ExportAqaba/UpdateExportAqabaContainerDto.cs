using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.ExportAqaba
{
    public class UpdateExportAqabaContainerDto
    {
        [Required]
        [MaxLength(50)]
        public string ContainerNumber { get; set; } = string.Empty;

        public ContainerSize ContainerSize { get; set; }

        public ContainerType ContainerType { get; set; }

        [Required]
        public int LoadingLocationId { get; set; }

        public ExportAqabaRouteStatus RouteStatus { get; set; }

        public DateTime WithdrawalDate { get; set; }

        public DateTime LoadingDate { get; set; }

        [Required]
        public int DeliveryLocationId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal WeightInTons { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}
