using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Service.Fuel
{
    public class FuelPurchaseService : IFuelPurchaseService
    {
        private readonly FleetWiseDbContext _context;

        public FuelPurchaseService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(CreateFuelPurchaseDto dto)
        {
            var purchase = new FuelPurchase
            {
                PurchaseDate = dto.PurchaseDate,
                Quantity = dto.Quantity,
                RemainingQuantity = dto.Quantity,
                PricePerLiter = dto.PricePerLiter,
                SupplierName = dto.SupplierName,
                Status = FuelPurchaseStatus.InUse,
                InvoiceNumber = dto.InvoiceNumber
            };

            await _context.FuelPurchases.AddAsync(purchase);

            await _context.SaveChangesAsync();

            return purchase.FuelPurchaseId;
        }
        public async Task<List<FuelPurchaseDto>> GetAllAsync()
        {
            return await _context.FuelPurchases
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.PurchaseDate)
                .Select(x => new FuelPurchaseDto
                {
                    FuelPurchaseId = x.FuelPurchaseId,
                    PurchaseDate = x.PurchaseDate,
                    Quantity = x.Quantity,
                    RemainingQuantity = x.RemainingQuantity,
                    PricePerLiter = x.PricePerLiter,
                    SupplierName = x.SupplierName,
                    Status = x.Status,
                    BatchOrigin = x.BatchOrigin,
                    SourceVehicleId = x.SourceVehicleId,
                    InvoiceNumber = x.InvoiceNumber
                })
                .ToListAsync();
        }

        public async Task<FuelInventorySummaryDto> GetSummaryAsync()
        {
            var batches = await _context.FuelPurchases.Where(x => !x.IsDeleted && x.RemainingQuantity > 0).ToListAsync();
            return new FuelInventorySummaryDto
            {
                TotalQuantity = batches.Sum(x => x.RemainingQuantity),
                TotalValueJod = Math.Round(batches.Sum(x => x.RemainingQuantity * x.PricePerLiter), 3, MidpointRounding.AwayFromZero),
                PurchasedQuantity = batches.Where(x => x.BatchOrigin == FleetWise.DAL.Enums.Fuel.FuelInventoryBatchOrigin.Purchase).Sum(x => x.RemainingQuantity),
                ReturnedQuantity = batches.Where(x => x.BatchOrigin == FleetWise.DAL.Enums.Fuel.FuelInventoryBatchOrigin.VehicleReturn).Sum(x => x.RemainingQuantity),
                AvailableBatchCount = batches.Count
            };
        }
    }
}
