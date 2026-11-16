using FleetWise.BLL.DTOs.Containers.ShippingLines;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Containers
{
    public interface IShippingLine
    {
        Task<ShippingLineResponseDto> CreateAsync(CreateShippingLineDto dto);

        Task<List<ShippingLineResponseDto>> GetAllAsync();
        Task<bool> UpdateAsync(int id, UpdateShippingLineDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
