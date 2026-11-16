using FleetWise.BLL.DTOs.Vehicles.Trailer;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Vehicles
{
    public interface ITrailer
    {
        Task<List<TrailerResponseDto>> GetAllAsync();
        Task<TrailerResponseDto> GetByIdAsync(int id);
        Task<TrailerResponseDto> CreateAsync(CreateTrailerDto dto);
        Task<bool> UpdateAsync(int id, UpdateTrailerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
