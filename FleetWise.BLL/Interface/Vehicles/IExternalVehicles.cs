using FleetWise.BLL.DTOs.Vehicles.ExternalVehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Vehicles
{
    public interface IExternalVehicles
    {
        Task<List<ExternalVehiclesResponseDto>> GetAllAsync();

        Task<ExternalVehiclesResponseDto?> GetByIdAsync(int id);

        Task<ExternalVehiclesResponseDto> CreateAsync(CreateExternalVehiclesDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
