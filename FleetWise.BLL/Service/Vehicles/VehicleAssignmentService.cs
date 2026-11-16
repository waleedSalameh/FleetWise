using FleetWise.BLL.DTOs.Vehicles.VehicleAssignments;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Vehicles;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static FleetWise.DAL.Entities.Trip;

namespace FleetWise.BLL.Service.Vehicles
{
    public class VehicleAssignmentService : IVehicleAssignment
    {
        private readonly FleetWiseDbContext _context;

        public VehicleAssignmentService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleAssignmentDto> CreateAsync(CreateVehicleAssignmentDto dto)
        {
            ValidateAssignmentDates(dto.StartDate, dto.EndDate);

            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            foreach (var resource in new[]
                {
                    $"FleetWise:Assignment:Trailer:{dto.TrailerId}",
                    $"FleetWise:Assignment:Vehicle:{dto.VehicleId}"
                }.Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.VehicleId == dto.VehicleId &&
                    !v.IsDeleted);

            if (vehicle == null)
                throw new ArgumentException("Vehicle not found.");

            var trailer = await _context.Trailers
                .FirstOrDefaultAsync(t =>
                    t.Id == dto.TrailerId &&
                    !t.IsDeleted);

            if (trailer == null)
                throw new ArgumentException("Trailer not found.");

            if (vehicle.Status != VehicleStatus.Available)
            {
                throw new InvalidOperationException(
                    "Vehicle is not available for assignment.");
            }

            if (await HasVehicleDateOverlapAsync(
                dto.VehicleId,
                dto.StartDate,
                dto.EndDate))
            {
                throw new InvalidOperationException(
                    "Vehicle already has an assignment during this period.");
            }

            if (await HasTrailerDateOverlapAsync(
                dto.TrailerId,
                dto.StartDate,
                dto.EndDate))
            {
                throw new InvalidOperationException(
                    "Trailer already has an assignment during this period.");
            }

            var assignment = new VehicleAssignment
            {
                VehicleId = dto.VehicleId,
                TrailerId = dto.TrailerId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _context.VehicleAssignments.Add(assignment);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(assignment.AssignmentId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:VehicleAssignment:{id}");

            var assignment = await _context.VehicleAssignments
                .FirstOrDefaultAsync(a =>
                    a.AssignmentId == id &&
                    !a.IsDeleted);

            if (assignment == null)
                return false;

            foreach (var resource in new[]
                {
                    $"FleetWise:Assignment:Trailer:{assignment.TrailerId}",
                    $"FleetWise:Assignment:Vehicle:{assignment.VehicleId}"
                }.Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            if (await HasInProgressTripAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete an assignment linked to a trip that is in progress.");
            }

            assignment.IsDeleted = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<List<VehicleAssignmentDto>> GetAllAsync()
        {
            return await _context.VehicleAssignments
                .Where(a => !a.IsDeleted)
                .Select(a => new VehicleAssignmentDto
                {
                    AssignmentId = a.AssignmentId,
                    VehicleId = a.VehicleId,
                    PlateNumber = a.Vehicle.PlateNumber,
                    TrailerId = a.TrailerId,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsDeleted = a.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<VehicleAssignmentDto> GetByIdAsync(int id)
        {
            return await _context.VehicleAssignments
                .Where(a =>
                    a.AssignmentId == id &&
                    !a.IsDeleted)
                .Select(a => new VehicleAssignmentDto
                {
                    AssignmentId = a.AssignmentId,
                    VehicleId = a.VehicleId,
                    PlateNumber = a.Vehicle.PlateNumber,
                    TrailerId = a.TrailerId,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    IsDeleted = a.IsDeleted
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleAssignmentDto dto)
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:VehicleAssignment:{id}");

            var assignment = await _context.VehicleAssignments
                .FirstOrDefaultAsync(a =>
                    a.AssignmentId == id &&
                    !a.IsDeleted);

            if (assignment == null)
                return false;

            foreach (var resource in new[]
                {
                    $"FleetWise:Assignment:Trailer:{assignment.TrailerId}",
                    $"FleetWise:Assignment:Vehicle:{assignment.VehicleId}",
                    $"FleetWise:Assignment:Trailer:{dto.TrailerId}",
                    $"FleetWise:Assignment:Vehicle:{dto.VehicleId}"
                }.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
            {
                await DatabaseApplicationLock.AcquireAsync(_context, resource);
            }

            ValidateAssignmentDates(dto.StartDate, dto.EndDate);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.VehicleId == dto.VehicleId &&
                    !v.IsDeleted);

            if (vehicle == null)
                throw new ArgumentException("Vehicle not found.");

            var trailer = await _context.Trailers
                .FirstOrDefaultAsync(t =>
                    t.Id == dto.TrailerId &&
                    !t.IsDeleted);

            if (trailer == null)
                throw new ArgumentException("Trailer not found.");

            if (vehicle.Status != VehicleStatus.Available &&
                assignment.VehicleId != dto.VehicleId)
            {
                throw new InvalidOperationException(
                    "Vehicle is not available for assignment.");
            }

            if (await HasVehicleDateOverlapAsync(
                dto.VehicleId,
                dto.StartDate,
                dto.EndDate,
                id))
            {
                throw new InvalidOperationException(
                    "Vehicle already has an assignment during this period.");
            }

            if (await HasTrailerDateOverlapAsync(
                dto.TrailerId,
                dto.StartDate,
                dto.EndDate,
                id))
            {
                throw new InvalidOperationException(
                    "Trailer already has an assignment during this period.");
            }

            if (await HasInProgressTripAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot end an assignment while a trip is in progress.");
            }

            assignment.VehicleId = dto.VehicleId;
            assignment.TrailerId = dto.TrailerId;
            assignment.StartDate = dto.StartDate;
            assignment.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private static void ValidateAssignmentDates(DateTime startDate, DateTime? endDate)
        {
            if (endDate.HasValue && endDate.Value < startDate)
            {
                throw new ArgumentException(
                    "End date cannot be earlier than start date.");
            }
        }

        private async Task<bool> HasVehicleDateOverlapAsync(
            int vehicleId,
            DateTime startDate,
            DateTime? endDate,
            int? assignmentId = null)
        {
            return await _context.VehicleAssignments
                .AnyAsync(a =>
                    a.VehicleId == vehicleId &&
                    !a.IsDeleted &&
                    (!assignmentId.HasValue ||
                     a.AssignmentId != assignmentId.Value) &&
                    (!endDate.HasValue ||
                     a.StartDate <= endDate.Value) &&
                    (!a.EndDate.HasValue ||
                     a.EndDate.Value >= startDate));
        }

        private async Task<bool> HasTrailerDateOverlapAsync(
            int trailerId,
            DateTime startDate,
            DateTime? endDate,
            int? assignmentId = null)
        {
            return await _context.VehicleAssignments
                .AnyAsync(a =>
                    a.TrailerId == trailerId &&
                    !a.IsDeleted &&
                    (!assignmentId.HasValue ||
                     a.AssignmentId != assignmentId.Value) &&
                    (!endDate.HasValue ||
                     a.StartDate <= endDate.Value) &&
                    (!a.EndDate.HasValue ||
                     a.EndDate.Value >= startDate));
        }

        private async Task<bool> HasInProgressTripAsync(int assignmentId)
        {
            return await _context.Trips
                .AnyAsync(t =>
                    t.AssignmentId == assignmentId &&
                    !t.IsDeleted &&
                    t.Status == TripStatus.InProgress);
        }
    }
}
