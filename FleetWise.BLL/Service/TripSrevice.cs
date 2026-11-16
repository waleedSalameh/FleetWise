using FleetWise.BLL.DTOs.Trips;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Enums;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static FleetWise.DAL.Entities.Trip;

namespace FleetWise.BLL.Service.Containers.Trip
{
    public class TripService : ITrip
    {
        private readonly FleetWiseDbContext _context;

        public TripService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateTripDto dto)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);
            foreach (var resource in GetTripLockKeys(
                dto.TripType,
                dto.IncomingAqabaBillId,
                dto.IncomingAqabaContainerId,
                dto.ExportAqabaBookingId,
                dto.ExportAqabaContainerId,
                dto.InternationalIncomingId,
                dto.InternationalExportId,
                dto.InlandTransportId,
                dto.AssignmentId).Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            ValidateCollectionMethod(dto.CollectionMethod);
            ValidateTripDates(dto.LoadingDate, dto.UnloadingDate);
            ValidateCosts(dto.DriverTip, dto.DieselCost, dto.LeakageCost,dto.OtherExpenses,
                dto.PurchasePrice, dto.SalePrice,dto.AdvancePayment);

            await ValidateVehicleSourceAsync(
                dto.VehicleSourceType,
                dto.AssignmentId,
                dto.DriverId,
                dto.ExternalVehicleId,
                dto.LoadingDate,
                dto.UnloadingDate);

            await ValidateTripSourceAsync(
                dto.TripType,
                dto.IncomingAqabaBillId,
                dto.IncomingAqabaContainerId,
                dto.ExportAqabaBookingId,
                dto.ExportAqabaContainerId,
                dto.InternationalIncomingId,
                dto.InternationalExportId,
                dto.InlandTransportId);

            var trip = new FleetWise.DAL.Entities.Trip
            {
                VehicleSourceType = dto.VehicleSourceType,
                AssignmentId = dto.AssignmentId,
                DriverId = dto.DriverId,
                ExternalVehicleId = dto.ExternalVehicleId,

                TripType = dto.TripType,
                CollectionMethod = dto.CollectionMethod,

                IncomingAqabaBillId = dto.IncomingAqabaBillId,
                IncomingAqabaContainerId = dto.IncomingAqabaContainerId,

                ExportAqabaBookingId = dto.ExportAqabaBookingId,
                ExportAqabaContainerId = dto.ExportAqabaContainerId,

                InternationalIncomingId = dto.InternationalIncomingId,
                InternationalExportId = dto.InternationalExportId,
                InlandTransportId = dto.InlandTransportId,

                StartLocation = dto.StartLocation,
                Destination = dto.Destination,
                LoadingDate = dto.LoadingDate,
                UnloadingDate = dto.UnloadingDate,
                Distance = dto.Distance,

                DriverTip = dto.DriverTip,
                DieselCost = dto.DieselCost,
                LeakageCost = dto.LeakageCost,
                OtherExpenses = dto.OtherExpenses,

                PurchasePrice = dto.PurchasePrice,
                SalePrice = dto.SalePrice,
                AdvancePayment = dto.AdvancePayment,

                Notes = dto.Notes,
                Status = TripStatus.Planned
            };

            _context.Trips.Add(trip);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return trip.TripId;
        }

        public async Task<List<TripResponseDto>> GetAllAsync()
        {
            return await _context.Trips
                .Where(t => !t.IsDeleted)
                .Select(t => new TripResponseDto
                {
                    TripId = t.TripId,

                    VehicleSourceType = t.VehicleSourceType,
                    AssignmentId = t.AssignmentId,
                    DriverId = t.DriverId,
                    DriverName = t.Driver != null
                        ? t.Driver.FullName
                        : null,

                    ExternalVehicleId = t.ExternalVehicleId,
                    ExternalVehiclePlateNumber = t.ExternalVehicle != null
                        ? t.ExternalVehicle.PlateNumber
                        : null,
                    ExternalDriverName = t.ExternalVehicle != null
                        ? t.ExternalVehicle.DriverName
                        : null,

                    TripType = t.TripType,
                    CollectionMethod = t.CollectionMethod,

                    IncomingAqabaBillId = t.IncomingAqabaBillId,
                    BillNumber = t.IncomingAqabaBill != null
                        ? t.IncomingAqabaBill.BillNumber
                        : null,

                    IncomingAqabaContainerId = t.IncomingAqabaContainerId,
                    IncomingContainerNumber = t.IncomingAqabaContainer != null
                        ? t.IncomingAqabaContainer.ContainerNumber
                        : null,

                    ExportAqabaBookingId = t.ExportAqabaBookingId,
                    BookingNumber = t.ExportAqabaBooking != null
                        ? t.ExportAqabaBooking.BookingNumber
                        : null,

                    ExportAqabaContainerId = t.ExportAqabaContainerId,
                    ExportContainerNumber = t.ExportAqabaContainer != null
                        ? t.ExportAqabaContainer.ContainerNumber
                        : null,

                    InternationalIncomingId = t.InternationalIncomingId,
                    InternationalIncomingCargoNumber =
                        t.InternationalIncoming != null
                            ? t.InternationalIncoming.CargoNumber
                            : null,

                    InternationalExportId = t.InternationalExportId,
                    InternationalExportCargoNumber = t.InternationalExport != null ? t.InternationalExport.CargoNumber: null,

                    InlandTransportId = t.InlandTransportId,
                    InlandTransportTripCode = t.InlandTransport != null
                        ? t.InlandTransport.TripCode
                        : null,

                    StartLocation = t.StartLocation,
                    Destination = t.Destination,
                    LoadingDate = t.LoadingDate,
                    UnloadingDate = t.UnloadingDate,
                    Distance = t.Distance,

                    DriverTip = t.DriverTip,
                    DieselCost = t.DieselCost,
                    LeakageCost = t.LeakageCost,
                    OtherExpenses = t.OtherExpenses,

                    PurchasePrice = t.PurchasePrice,
                    SalePrice = t.SalePrice,
                    AdvancePayment = t.AdvancePayment,

                    Notes = t.Notes,
                    Status = t.Status
                })
                .ToListAsync();
        }

        public async Task<TripResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Trips
                .Where(t =>
                    t.TripId == id &&
                    !t.IsDeleted)
                .Select(t => new TripResponseDto
                {
                    TripId = t.TripId,

                    VehicleSourceType = t.VehicleSourceType,
                    AssignmentId = t.AssignmentId,
                    DriverId = t.DriverId,
                    DriverName = t.Driver != null
                        ? t.Driver.FullName
                        : null,

                    ExternalVehicleId = t.ExternalVehicleId,
                    ExternalVehiclePlateNumber = t.ExternalVehicle != null
                        ? t.ExternalVehicle.PlateNumber
                        : null,
                    ExternalDriverName = t.ExternalVehicle != null
                        ? t.ExternalVehicle.DriverName
                        : null,

                    TripType = t.TripType,
                    CollectionMethod = t.CollectionMethod,

                    IncomingAqabaBillId = t.IncomingAqabaBillId,
                    BillNumber = t.IncomingAqabaBill != null
                        ? t.IncomingAqabaBill.BillNumber
                        : null,

                    IncomingAqabaContainerId = t.IncomingAqabaContainerId,
                    IncomingContainerNumber = t.IncomingAqabaContainer != null
                        ? t.IncomingAqabaContainer.ContainerNumber
                        : null,

                    ExportAqabaBookingId = t.ExportAqabaBookingId,
                    BookingNumber = t.ExportAqabaBooking != null
                        ? t.ExportAqabaBooking.BookingNumber
                        : null,

                    ExportAqabaContainerId = t.ExportAqabaContainerId,
                    ExportContainerNumber = t.ExportAqabaContainer != null
                        ? t.ExportAqabaContainer.ContainerNumber
                        : null,

                    InternationalIncomingId = t.InternationalIncomingId,
                    InternationalIncomingCargoNumber =
                        t.InternationalIncoming != null
                            ? t.InternationalIncoming.CargoNumber
                            : null,

                    InternationalExportId = t.InternationalExportId,
                    InternationalExportCargoNumber =
                        t.InternationalExport != null
                            ? t.InternationalExport.CargoNumber
                            : null,

                    InlandTransportId = t.InlandTransportId,
                    InlandTransportTripCode = t.InlandTransport != null
                        ? t.InlandTransport.TripCode
                        : null,

                    StartLocation = t.StartLocation,
                    Destination = t.Destination,
                    LoadingDate = t.LoadingDate,
                    UnloadingDate = t.UnloadingDate,
                    Distance = t.Distance,

                    DriverTip = t.DriverTip,
                    DieselCost = t.DieselCost,
                    LeakageCost = t.LeakageCost,
                    OtherExpenses = t.OtherExpenses,

                    PurchasePrice = t.PurchasePrice,
                    SalePrice = t.SalePrice,
                    AdvancePayment = t.AdvancePayment,

                    Notes = t.Notes,
                    Status = t.Status
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateTripDto dto)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Trip:{id}");

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.TripId == id && !t.IsDeleted);

            if (trip == null)
                return false;

            var lockKeys = GetTripLockKeys(
                    trip.TripType,
                    trip.IncomingAqabaBillId,
                    trip.IncomingAqabaContainerId,
                    trip.ExportAqabaBookingId,
                    trip.ExportAqabaContainerId,
                    trip.InternationalIncomingId,
                    trip.InternationalExportId,
                    trip.InlandTransportId,
                    trip.AssignmentId)
                .Concat(GetTripLockKeys(
                    dto.TripType,
                    dto.IncomingAqabaBillId,
                    dto.IncomingAqabaContainerId,
                    dto.ExportAqabaBookingId,
                    dto.ExportAqabaContainerId,
                    dto.InternationalIncomingId,
                    dto.InternationalExportId,
                    dto.InlandTransportId,
                    dto.AssignmentId));

            foreach (var resource in lockKeys.Distinct(StringComparer.Ordinal)
                         .Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            ValidateCollectionMethod(dto.CollectionMethod);

            ValidateTripDates(dto.LoadingDate, dto.UnloadingDate);
            ValidateCosts(dto.DriverTip, dto.DieselCost, dto.LeakageCost,
                dto.OtherExpenses, dto.PurchasePrice, dto.SalePrice,
                dto.AdvancePayment);

            await ValidateVehicleSourceAsync(
                dto.VehicleSourceType,
                dto.AssignmentId,
                dto.DriverId,
                dto.ExternalVehicleId,
                dto.LoadingDate,
                dto.UnloadingDate);

            await ValidateTripSourceAsync(
                dto.TripType,
                dto.IncomingAqabaBillId,
                dto.IncomingAqabaContainerId,
                dto.ExportAqabaBookingId,
                dto.ExportAqabaContainerId,
                dto.InternationalIncomingId,
                dto.InternationalExportId,
                dto.InlandTransportId,
                id);

            trip.VehicleSourceType = dto.VehicleSourceType;
            trip.AssignmentId = dto.AssignmentId;
            trip.DriverId = dto.DriverId;
            trip.ExternalVehicleId = dto.ExternalVehicleId;

            trip.TripType = dto.TripType;
            trip.CollectionMethod = dto.CollectionMethod;

            trip.IncomingAqabaBillId = dto.IncomingAqabaBillId;
            trip.IncomingAqabaContainerId = dto.IncomingAqabaContainerId;

            trip.ExportAqabaBookingId = dto.ExportAqabaBookingId;
            trip.ExportAqabaContainerId = dto.ExportAqabaContainerId;

            trip.InternationalIncomingId = dto.InternationalIncomingId;
            trip.InternationalExportId = dto.InternationalExportId;
            trip.InlandTransportId = dto.InlandTransportId;

            trip.StartLocation = dto.StartLocation;
            trip.Destination = dto.Destination;
            trip.LoadingDate = dto.LoadingDate;
            trip.UnloadingDate = dto.UnloadingDate;
            trip.Distance = dto.Distance;

            trip.DriverTip = dto.DriverTip;
            trip.DieselCost = dto.DieselCost;
            trip.LeakageCost = dto.LeakageCost;
            trip.OtherExpenses = dto.OtherExpenses;

            trip.PurchasePrice = dto.PurchasePrice;
            trip.SalePrice = dto.SalePrice;
            trip.AdvancePayment = dto.AdvancePayment;

            trip.Notes = dto.Notes;
            trip.Status = dto.Status;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Trip:{id}");

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t =>
                    t.TripId == id &&
                    !t.IsDeleted);

            if (trip == null)
                return false;

            foreach (var resource in GetTripLockKeys(
                trip.TripType,
                trip.IncomingAqabaBillId,
                trip.IncomingAqabaContainerId,
                trip.ExportAqabaBookingId,
                trip.ExportAqabaContainerId,
                trip.InternationalIncomingId,
                trip.InternationalExportId,
                trip.InlandTransportId,
                trip.AssignmentId).Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            if (trip.Status != TripStatus.Cancelled)
            {
                throw new InvalidOperationException(
                    "Only cancelled trips can be deleted."
                );
            }

            trip.IsDeleted = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private async Task ValidateVehicleSourceAsync(
            VehicleSourceType vehicleSourceType,
            int? assignmentId,
            int? driverId,
            int? externalVehicleId,
            DateTime loadingDate,
            DateTime unloadingDate)
        {
            if (vehicleSourceType == VehicleSourceType.Internal)
            {
                if (!assignmentId.HasValue || !driverId.HasValue)
                {
                    throw new ArgumentException(
                        "Assignment and driver are required for internal vehicles.");
                }

                if (externalVehicleId.HasValue)
                {
                    throw new ArgumentException(
                        "External vehicle cannot be selected for an internal trip.");
                }

                var assignment = await _context.VehicleAssignments
                    .Include(a => a.Vehicle)
                    .Include(a => a.Trailer)
                    .FirstOrDefaultAsync(a =>
                        a.AssignmentId == assignmentId.Value &&
                        !a.IsDeleted);

                if (assignment == null ||
                    assignment.Vehicle == null ||
                    assignment.Vehicle.IsDeleted ||
                    assignment.Trailer == null ||
                    assignment.Trailer.IsDeleted)
                {
                    throw new ArgumentException(
                        "Vehicle assignment not found.");
                }

                if (assignment.StartDate > loadingDate ||
                    (assignment.EndDate.HasValue &&
                     assignment.EndDate.Value < unloadingDate))
                {
                    throw new InvalidOperationException(
                        "Assignment does not cover the entire trip period.");
                }

                var driver = await _context.Drivers
                    .FirstOrDefaultAsync(d =>
                        d.DriverId == driverId.Value &&
                        !d.IsDeleted);

                if (driver == null)
                    throw new ArgumentException("Driver not found.");

                if (driver.Status != DriverStatus.Active)
                {
                    throw new InvalidOperationException(
                        "Driver is not active.");
                }

                if (driver.LicenseExpiryDate <
                    DateOnly.FromDateTime(unloadingDate))
                {
                    throw new InvalidOperationException(
                        "Driver license expires before the trip ends.");
                }
            }
            else if (vehicleSourceType == VehicleSourceType.External)
            {
                if (!externalVehicleId.HasValue)
                {
                    throw new ArgumentException(
                        "External vehicle is required.");
                }

                if (assignmentId.HasValue || driverId.HasValue)
                {
                    throw new ArgumentException(
                        "Assignment and internal driver cannot be selected for external vehicles.");
                }

                var externalVehicle = await _context.ExternalVehicles
                    .FirstOrDefaultAsync(v =>
                        v.Id == externalVehicleId.Value &&
                        !v.IsDeleted);

                if (externalVehicle == null)
                {
                    throw new ArgumentException(
                        "External vehicle not found.");
                }
            }
            else
            {
                throw new ArgumentException(
                    "Invalid vehicle source type.");
            }
        }

        private async Task ValidateTripSourceAsync(TripType tripType, int? incomingAqabaBillId, int? incomingAqabaContainerId, int? exportAqabaBookingId, int? exportAqabaContainerId, int? internationalIncomingId, int? internationalExportId, int? inlandTransportId, int? tripId = null)
        {
            switch (tripType)
            {
                case TripType.IncomingAqaba:

                    if (!incomingAqabaBillId.HasValue ||
                        !incomingAqabaContainerId.HasValue)
                    {
                        throw new ArgumentException(
                            "Incoming Aqaba bill and container are required.");
                    }

                    if (await _context.IncomingAqabaBills.AnyAsync(b =>
                        b.Id == incomingAqabaBillId.Value &&
                        !b.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "Incoming Aqaba bill not found.");
                    }

                    if (await _context.IncomingAqabaContainers.AnyAsync(c =>
                        c.Id == incomingAqabaContainerId.Value &&
                        c.BillId == incomingAqabaBillId.Value &&
                        !c.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "Incoming Aqaba container does not belong to the selected bill.");
                    }

                    if (await _context.Trips.AnyAsync(t =>
                        t.IncomingAqabaContainerId == incomingAqabaContainerId.Value &&
                        !t.IsDeleted &&
                        t.Status != TripStatus.Cancelled &&
                        (!tripId.HasValue || t.TripId != tripId.Value)))
                    {
                        throw new InvalidOperationException(
                            "This container is already linked to another trip.");
                    }

                    break;

                case TripType.OutgoingAqaba:

                    if (!exportAqabaBookingId.HasValue ||
                        !exportAqabaContainerId.HasValue)
                    {
                        throw new ArgumentException(
                            "Export Aqaba booking and container are required.");
                    }

                    if (await _context.ExportAqabaBookings.AnyAsync(b =>
                        b.Id == exportAqabaBookingId.Value &&
                        !b.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "Export Aqaba booking not found.");
                    }

                    if (await _context.ExportAqabaContainers.AnyAsync(c =>
                        c.Id == exportAqabaContainerId.Value &&
                        c.BookingId == exportAqabaBookingId.Value &&
                        !c.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "Export Aqaba container does not belong to the selected booking.");
                    }

                    if (await _context.Trips.AnyAsync(t =>
                        t.ExportAqabaContainerId == exportAqabaContainerId.Value &&
                        !t.IsDeleted &&
                        t.Status != TripStatus.Cancelled &&
                        (!tripId.HasValue || t.TripId != tripId.Value)))
                    {
                        throw new InvalidOperationException(
                            "This container is already linked to another trip.");
                    }

                    break;

                case TripType.InternationalIncoming:

                    if (!internationalIncomingId.HasValue)
                    {
                        throw new ArgumentException(
                            "International incoming record is required.");
                    }

                    if (await _context.InternationalIncomings.AnyAsync(i =>
                        i.Id == internationalIncomingId.Value &&
                        !i.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "International incoming record not found.");
                    }

                    if (await _context.Trips.AnyAsync(t =>
                        t.InternationalIncomingId == internationalIncomingId.Value &&
                        !t.IsDeleted &&
                        t.Status != TripStatus.Cancelled &&
                        (!tripId.HasValue || t.TripId != tripId.Value)))
                    {
                        throw new InvalidOperationException(
                            "This international incoming record is already linked to another trip.");
                    }

                    break;

                case TripType.InternationalExport:

                    if (!internationalExportId.HasValue)
                    {
                        throw new ArgumentException(
                            "International export record is required.");
                    }

                    if (await _context.InternationalExports.AnyAsync(e =>
                        e.Id == internationalExportId.Value &&
                        !e.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "International export record not found.");
                    }

                    if (await _context.Trips.AnyAsync(t =>
                        t.InternationalExportId == internationalExportId.Value &&
                        !t.IsDeleted &&
                        t.Status != TripStatus.Cancelled &&
                        (!tripId.HasValue || t.TripId != tripId.Value)))
                    {
                        throw new InvalidOperationException(
                            "This international export record is already linked to another trip.");
                    }

                    break;

                case TripType.InlandTransport:

                    if (!inlandTransportId.HasValue)
                    {
                        throw new ArgumentException(
                            "Inland transport record is required.");
                    }

                    if (await _context.InlandTransports.AnyAsync(i =>
                        i.Id == inlandTransportId.Value &&
                        !i.IsDeleted) == false)
                    {
                        throw new ArgumentException(
                            "Inland transport record not found.");
                    }

                    if (await _context.Trips.AnyAsync(t =>
                        t.InlandTransportId == inlandTransportId.Value &&
                        !t.IsDeleted &&
                        t.Status != TripStatus.Cancelled &&
                        (!tripId.HasValue || t.TripId != tripId.Value)))
                    {
                        throw new InvalidOperationException(
                            "This inland transport record is already linked to another trip.");
                    }

                    break;

                default:
                    throw new ArgumentException("Invalid trip type.");
            }
        }

        private static IEnumerable<string> GetTripLockKeys(
            TripType tripType,
            int? incomingAqabaBillId,
            int? incomingAqabaContainerId,
            int? exportAqabaBookingId,
            int? exportAqabaContainerId,
            int? internationalIncomingId,
            int? internationalExportId,
            int? inlandTransportId,
            int? assignmentId)
        {
            if (assignmentId.HasValue)
                yield return $"FleetWise:VehicleAssignment:{assignmentId.Value}";

            switch (tripType)
            {
                case TripType.IncomingAqaba:
                    if (incomingAqabaBillId.HasValue)
                        yield return $"FleetWise:TripSource:IncomingAqabaBill:{incomingAqabaBillId.Value}";
                    if (incomingAqabaContainerId.HasValue)
                        yield return $"FleetWise:TripSource:IncomingAqabaContainer:{incomingAqabaContainerId.Value}";
                    break;
                case TripType.OutgoingAqaba:
                    if (exportAqabaBookingId.HasValue)
                        yield return $"FleetWise:TripSource:ExportAqabaBooking:{exportAqabaBookingId.Value}";
                    if (exportAqabaContainerId.HasValue)
                        yield return $"FleetWise:TripSource:ExportAqabaContainer:{exportAqabaContainerId.Value}";
                    break;
                case TripType.InternationalIncoming when internationalIncomingId.HasValue:
                    yield return $"FleetWise:TripSource:InternationalIncoming:{internationalIncomingId.Value}";
                    break;
                case TripType.InternationalExport when internationalExportId.HasValue:
                    yield return $"FleetWise:TripSource:InternationalExport:{internationalExportId.Value}";
                    break;
                case TripType.InlandTransport when inlandTransportId.HasValue:
                    yield return $"FleetWise:TripSource:InlandTransport:{inlandTransportId.Value}";
                    break;
            }
        }

        private static void ValidateCollectionMethod(CollectionMethod? collectionMethod)
        {
            if (collectionMethod.HasValue && !Enum.IsDefined(collectionMethod.Value))
                throw new ArgumentException("Invalid collection method.");
        }

        private static void ValidateTripDates( DateTime loadingDate, DateTime unloadingDate)
        {
            if (unloadingDate < loadingDate)
            {
                throw new ArgumentException(
                    "Unloading date cannot be earlier than loading date.");
            }
        }

        private static void ValidateCosts(decimal? driverTip, decimal? dieselCost, decimal? leakageCost, decimal? otherExpenses, decimal? purchasePrice, decimal? salePrice,decimal? advancePayment)
        {
            if (driverTip < 0 ||
                dieselCost < 0 ||
                leakageCost < 0 ||
                otherExpenses < 0 ||
                purchasePrice < 0 ||
                salePrice < 0 ||
                advancePayment < 0)
            {
                throw new ArgumentException(
                    "Trip costs cannot be negative.");
            }
        }
    }
}
