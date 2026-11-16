using System;
using System.Collections.Generic;
using System.Text;
using FleetWise.BLL.DTOs.Containers.ExportAqaba;

namespace FleetWise.BLL.Interface.Containers.ExportAqaba
{
    public interface IExportAqabaBooking
    {
        Task<List<ExportAqabaBookingResponseDto>> GetAllAsync();

        Task<ExportAqabaBookingResponseDto?> GetByIdAsync(int id);

        Task<ExportAqabaBookingResponseDto> CreateAsync(CreateExportAqabaBookingDto dto);

        Task<bool> UpdateAsync(int id, UpdateExportAqabaBookingDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
