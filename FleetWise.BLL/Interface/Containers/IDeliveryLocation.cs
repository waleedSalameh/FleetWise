using FleetWise.BLL.DTOs.Containers.DeliveryLocations;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers
{
    public interface IDeliveryLocation
    {
        Task<DeliveryLocationResponseDto> CreateAsync(CreateDeliveryLocationDto dto);
        Task<List<DeliveryLocationResponseDto>> GetAllAsync();
        Task<bool> UpdateAsync(int id, UpdateDeliveryLocationDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
