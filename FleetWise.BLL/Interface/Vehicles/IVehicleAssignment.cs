using FleetWise.BLL.DTOs.Vehicles.VehicleAssignments;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Vehicles
{
    public interface IVehicleAssignment
    {
        Task<List<VehicleAssignmentDto>> GetAllAsync();

        Task<VehicleAssignmentDto> GetByIdAsync(int id);

        Task<VehicleAssignmentDto> CreateAsync(CreateVehicleAssignmentDto dto);

        Task<bool> UpdateAsync(int id,UpdateVehicleAssignmentDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
