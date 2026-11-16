using FleetWise.BLL.DTOs.Trips;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface
{
    public interface ITrip
    {
        Task<List<TripResponseDto>> GetAllAsync();
        Task<TripResponseDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateTripDto dto);
        Task<bool> UpdateAsync(int id, UpdateTripDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
