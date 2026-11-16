using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.InternationalExport
{
    public class UpdateInternationalExportDto
    {
        [Required]
        [MaxLength(50)]
        public string CargoNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        public InternationalCargoType CargoType { get; set; }

        [Required]
        [MaxLength(200)]
        public string LoadingLocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string FactoryName { get; set; } = string.Empty;

        [Required]
        public decimal Weight { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        [MaxLength(50)]
        public string CustomsDeclarationNumber { get; set; } = string.Empty;

        [Required]
        public int TripDuration { get; set; }

        [Required]
        [MaxLength(200)]
        public string UnloadingLocation { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public InternationalOrderStatus Status { get; set; }

        public bool IsPaid { get; set; }
    }
}
