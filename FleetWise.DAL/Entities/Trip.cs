using FleetWise.DAL.Entities.Containers.ExportAqaba;
using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using FleetWise.DAL.Entities.Containers.InlandTransport;
using FleetWise.DAL.Entities.Containers.InternationalExport;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums;


namespace FleetWise.DAL.Entities
{
    public class Trip
    {
        public int TripId { get; set; }

        public VehicleSourceType VehicleSourceType { get; set; }

        public int? AssignmentId { get; set; }
        public VehicleAssignment? VehicleAssignment { get; set; }

        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }

        public int? ExternalVehicleId { get; set; }
        public ExternalVehicles? ExternalVehicle { get; set; }

        public TripType TripType { get; set; }
        public CollectionMethod? CollectionMethod { get; set; }

        public int? IncomingAqabaBillId { get; set; }
        public IncomingAqabaBill? IncomingAqabaBill { get; set; }

        public int? IncomingAqabaContainerId { get; set; }
        public IncomingAqabaContainer? IncomingAqabaContainer { get; set; }

        public int? ExportAqabaBookingId { get; set; }
        public ExportAqabaBooking? ExportAqabaBooking { get; set; }

        public int? ExportAqabaContainerId { get; set; }
        public ExportAqabaContainer? ExportAqabaContainer { get; set; }

        public int? InternationalIncomingId { get; set; }
        public InternationalIncoming? InternationalIncoming { get; set; }

        public int? InternationalExportId { get; set; }
        public InternationalExport? InternationalExport { get; set; }

        public int? InlandTransportId { get; set; }
        public InlandTransport? InlandTransport { get; set; }

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

        public bool IsDeleted { get; set; }

        public enum TripStatus
        {
            Planned = 0,
            InProgress = 1,
            Completed = 2,
            Cancelled = 3
        }
     
    }
}
