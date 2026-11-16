using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.BLL.DTOs.FuelRecords;

public class CreateFuelMeasurementDto
{
    [Range(1, int.MaxValue)]
    public int VehicleId { get; set; }
    public DateTime MeasuredAt { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal MeasuredQuantity { get; set; }

    [Range(typeof(decimal), "0", "9999999999999999")]
    public decimal? MeasuredValueJod { get; set; }
    public FuelMeasurementType MeasurementType { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class ApproveFuelMeasurementDto
{
    [Required, MaxLength(200)]
    public string ApprovedBy { get; set; } = string.Empty;
}

public class FuelMeasurementDto
{
    public long Id { get; set; }
    public int VehicleId { get; set; }
    public int PlateNumber { get; set; }
    public DateTime MeasuredAt { get; set; }
    public decimal MeasuredQuantity { get; set; }
    public decimal? MeasuredValueJod { get; set; }
    public FuelMeasurementType MeasurementType { get; set; }
    public FuelApprovalStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
