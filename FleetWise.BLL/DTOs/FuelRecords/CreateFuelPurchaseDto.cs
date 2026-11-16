using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class CreateFuelPurchaseDto
    {
        [Required]
        public DateTime PurchaseDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerLiter { get; set; }

        [MaxLength(150)]
        public string? SupplierName { get; set; }

        [MaxLength(50)]
        public string? InvoiceNumber { get; set; }
    }
}
