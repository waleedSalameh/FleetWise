using FleetWise.BLL.DTOs.Maintenance.MaintenanceRecords;
using FleetWise.BLL.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Maintenance
{
    public interface IMaintenanceRecord
    {
        Task<List<MaintenanceRecordDto>> GetAllAsync();

        Task<MaintenanceRecordDto> GetByIdAsync(int id);

        Task<MaintenanceRecordDto> CreateAsync(CreateMaintenanceRecordDto dto);

        Task<MaintenanceRecordDto?> UpdateAsync(int id, UpdateMaintenanceRecordDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
