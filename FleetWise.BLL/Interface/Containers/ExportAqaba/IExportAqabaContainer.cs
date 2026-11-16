using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers.ExportAqaba
{
    public interface IExportAqabaContainer
    {
        Task<ExportAqabaContainerResponseDto?> GetByIdAsync(int id);
        Task<ExportAqabaContainerResponseDto> CreateAsync(int bookingId, CreateExportAqabaContainerDto dto);
        Task<bool> UpdateAsync(int id,UpdateExportAqabaContainerDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
