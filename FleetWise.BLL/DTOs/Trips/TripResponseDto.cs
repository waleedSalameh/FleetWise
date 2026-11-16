using FleetWise.DAL.Enums;
using System;
using static FleetWise.DAL.Entities.Trip;

namespace FleetWise.BLL.DTOs.Trips
{
    public class TripResponseDto
    {
        public int TripId { get; set; }

        public VehicleSourceType VehicleSourceType { get; set; }

        public int? AssignmentId { get; set; }

        public int? DriverId { get; set; }

        public string? DriverName { get; set; }

        public int? ExternalVehicleId { get; set; }

        public int? ExternalVehiclePlateNumber { get; set; }

        public string? ExternalDriverName { get; set; }

        public TripType TripType { get; set; }
        public CollectionMethod? CollectionMethod { get; set; }

        public int? IncomingAqabaBillId { get; set; }

        public string? BillNumber { get; set; }

        public int? IncomingAqabaContainerId { get; set; }

        public string? IncomingContainerNumber { get; set; }

        public int? ExportAqabaBookingId { get; set; }

        public string? BookingNumber { get; set; }

        public int? ExportAqabaContainerId { get; set; }

        public string? ExportContainerNumber { get; set; }

        public int? InternationalIncomingId { get; set; }

        public string? InternationalIncomingCargoNumber { get; set; }

        public int? InternationalExportId { get; set; }

        public string? InternationalExportCargoNumber { get; set; }

        public int? InlandTransportId { get; set; }

        public string? InlandTransportTripCode { get; set; }

        public string StartLocation { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateTime LoadingDate { get; set; }

        public DateTime UnloadingDate { get; set; }

        public decimal Distance { get; set; }

        public decimal? DriverTip { get; set; }

        public decimal? DieselCost { get; set; }

        public decimal? LeakageCost { get; set; }

        public decimal? OtherExpenses { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? SalePrice { get; set; }

        public decimal? AdvancePayment { get; set; }

        public string? Notes { get; set; }

        public TripStatus Status { get; set; }
    }
}
