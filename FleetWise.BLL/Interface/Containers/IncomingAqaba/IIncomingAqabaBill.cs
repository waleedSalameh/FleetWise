using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers.IncomingAqaba
{
    public interface IIncomingAqabaBill
    {
        Task<IncomingAqabaBillResponseDto> CreateAsync(CreateIncomingAqabaBillDto dto);
        Task<List<IncomingAqabaBillResponseDto>> GetAllAsync();
        Task<IncomingAqabaBillResponseDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync( int id, UpdateIncomingAqabaBillDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
