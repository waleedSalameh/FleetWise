using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers.IncomingAqaba
{
    public interface IIncomingAqabaContainer
    {
        Task<IncomingAqabaContainerResponseDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateIncomingAqabaContainerDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IncomingAqabaContainerResponseDto> CreateAsync( int billId,CreateIncomingAqabaContainerDto dto);
    }
}
