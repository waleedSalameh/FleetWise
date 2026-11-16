using FleetWise.BLL.DTOs.Containers.InternationalExport;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers.IInternationalExport
{
    public interface IInternationalExport
    {
        Task<List<InternationalExportResponseDto>> GetAllAsync();

        Task<InternationalExportResponseDto> GetByIdAsync(int id);

        Task<InternationalExportResponseDto> CreateAsync(CreateInternationalExportDto dto);

        Task<bool> UpdateAsync(int id, UpdateInternationalExportDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
