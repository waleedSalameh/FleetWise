using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static FleetWise.DAL.Entities.Trip;

namespace FleetWise.BLL.DTOs.Trips
{
    public class CreateTripDto
    {


        [Required]
        [EnumDataType(typeof(VehicleSourceType))]
        public VehicleSourceType VehicleSourceType { get; set; }

        public int? AssignmentId { get; set; }

        public int? DriverId { get; set; }

        public int? ExternalVehicleId { get; set; }

        [Required]
        [EnumDataType(typeof(TripType))]
        public TripType TripType { get; set; }

        [EnumDataType(typeof(CollectionMethod))]
        public CollectionMethod? CollectionMethod { get; set; }

        public int? IncomingAqabaBillId { get; set; }

        public int? IncomingAqabaContainerId { get; set; }

        public int? ExportAqabaBookingId { get; set; }

        public int? ExportAqabaContainerId { get; set; }

        public int? InternationalIncomingId { get; set; }

        public int? InternationalExportId { get; set; }

        public int? InlandTransportId { get; set; }

        [Required]
        [MaxLength(100)]
        public string StartLocation { get; set; }

        [Required]
        [MaxLength(100)]
        public string Destination { get; set; }

        [Required]
        public DateTime LoadingDate { get; set; }

        [Required]
        public DateTime UnloadingDate { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal Distance { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? DriverTip { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? DieselCost { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? LeakageCost { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? OtherExpenses { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? PurchasePrice { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? SalePrice { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999")]
        public decimal? AdvancePayment { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

