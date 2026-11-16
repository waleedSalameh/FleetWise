using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.IncomingAqaba;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.IncomingAqaba
{
    public class IncomingAqabaContainerService : IIncomingAqabaContainer
    {
        private readonly FleetWiseDbContext _context;

        public IncomingAqabaContainerService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Get Container By Id
        public async Task<IncomingAqabaContainerResponseDto?> GetByIdAsync(int id)
        {
            var container = await _context.IncomingAqabaContainers
                .Where(c => c.Id == id && !c.IsDeleted)
                .Include(c => c.ShippingLine)
                .Include(c => c.DeliveryLocation)
                .FirstOrDefaultAsync();

            if (container == null)
                return null;

            return MapToResponse(container);
        }
        public async Task<IncomingAqabaContainerResponseDto> CreateAsync(
    int billId,
    CreateIncomingAqabaContainerDto dto)
        {
            // 1. التأكد من وجود الفاتورة
            var bill = await _context.IncomingAqabaBills
                .FirstOrDefaultAsync(b => b.Id == billId && !b.IsDeleted);

            if (bill == null)
                throw new Exception("Bill was not found.");

            // 2. تنظيف رقم الحاوية
            string containerNumber = dto.ContainerNumber.Trim();

            // 3. التحقق من عدم تكرار رقم الحاوية
            bool numberExists = await _context.IncomingAqabaContainers
                .AnyAsync(c =>
                    c.ContainerNumber == containerNumber &&
                    !c.IsDeleted);

            if (numberExists)
                throw new Exception("Container number already exists.");

            // 4. التأكد من وجود شركة الشحن
            bool shippingLineExists = await _context.ShippingLines
                .AnyAsync(s =>
                    s.ShippingLineId == dto.ShippingLineId &&
                    !s.IsDeleted);

            if (!shippingLineExists)
                throw new Exception("Shipping line was not found.");

            // 5. التأكد من وجود موقع التسليم
            bool deliveryLocationExists = await _context.DeliveryLocations
                .AnyAsync(d =>
                    d.DeliveryLocationId == dto.DeliveryLocationId &&
                    !d.IsDeleted);

            if (!deliveryLocationExists)
                throw new Exception("Delivery location was not found.");

            // 6. إنشاء الحاوية
            var container = new IncomingAqabaContainer
            {
                BillId = billId,
                ContainerNumber = containerNumber,
                ContainerType = dto.ContainerType,
                ContainerSize = dto.ContainerSize,
                ShippingLineId = dto.ShippingLineId,
                DeliveryLocationId = dto.DeliveryLocationId,
                WeightInTons = dto.WeightInTons,
                VesselArrivalDate = dto.VesselArrivalDate,
                PortFreeTimeEndDate = dto.PortFreeTimeEndDate,
                EmptyReturnFreeTimeEndDate = dto.EmptyReturnFreeTimeEndDate,
                ContainerLoadingDate = dto.ContainerLoadingDate,
                Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
                Status = dto.Status,
                IsDeleted = false
            };

            // 7. إضافة الحاوية
            _context.IncomingAqabaContainers.Add(container);

            // 8. تحديث عدد الحاويات في الفاتورة
            bill.ContainerCount++;

            // 9. حفظ البيانات
            await _context.SaveChangesAsync();

            // 10. جلب العلاقات لعرض أسمائها في Response
            await _context.Entry(container)
                .Reference(c => c.ShippingLine)
                .LoadAsync();

            await _context.Entry(container)
                .Reference(c => c.DeliveryLocation)
                .LoadAsync();

            return MapToResponse(container);
        }

        // Update Container
        public async Task<bool> UpdateAsync(
            int id,
            UpdateIncomingAqabaContainerDto dto)
        {
            var container = await _context.IncomingAqabaContainers
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (container == null)
                return false;

            string containerNumber = dto.ContainerNumber.Trim();

            // Check duplicate container number
            bool numberExists = await _context.IncomingAqabaContainers
                .AnyAsync(c =>
                    c.Id != id &&
                    c.ContainerNumber == containerNumber &&
                    !c.IsDeleted);

            if (numberExists)
                throw new Exception("Container number already exists.");

            // Check Shipping Line
            bool shippingLineExists = await _context.ShippingLines
                .AnyAsync(s =>
                    s.ShippingLineId == dto.ShippingLineId &&
                    !s.IsDeleted);

            if (!shippingLineExists)
                throw new Exception("Shipping line was not found.");

            // Check Delivery Location
            bool deliveryLocationExists = await _context.DeliveryLocations
                .AnyAsync(d =>
                    d.DeliveryLocationId == dto.DeliveryLocationId &&
                    !d.IsDeleted);

            if (!deliveryLocationExists)
                throw new Exception("Delivery location was not found.");

            // Update Container
            container.ContainerNumber = containerNumber;
            container.ContainerType = dto.ContainerType;
            container.ContainerSize = dto.ContainerSize;
            container.ShippingLineId = dto.ShippingLineId;
            container.DeliveryLocationId = dto.DeliveryLocationId;
            container.WeightInTons = dto.WeightInTons;
            container.VesselArrivalDate = dto.VesselArrivalDate;
            container.PortFreeTimeEndDate = dto.PortFreeTimeEndDate;
            container.EmptyReturnFreeTimeEndDate = dto.EmptyReturnFreeTimeEndDate;
            container.ContainerLoadingDate = dto.ContainerLoadingDate;
            
            if (dto.Notes is not null)
                container.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
            container.Status = dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        // Delete Container (Soft Delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var container = await _context.IncomingAqabaContainers
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (container == null)
                return false;

            container.IsDeleted = true;

            // Update Bill Container Count
            var bill = await _context.IncomingAqabaBills
                .FirstOrDefaultAsync(b =>
                    b.Id == container.BillId &&
                    !b.IsDeleted);

            if (bill != null)
            {
                bill.ContainerCount = await _context.IncomingAqabaContainers
                    .CountAsync(c =>
                        c.BillId == bill.Id &&
                        !c.IsDeleted &&
                        c.Id != id);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        // Mapping
        private static IncomingAqabaContainerResponseDto MapToResponse(
            IncomingAqabaContainer container)
        {
            return new IncomingAqabaContainerResponseDto
            {
                Id = container.Id,
                ContainerNumber = container.ContainerNumber,
                ContainerType = container.ContainerType,
                ContainerSize = container.ContainerSize,
                ShippingLineId = container.ShippingLineId,
                ShippingLineName = container.ShippingLine.Name,
                DeliveryLocationId = container.DeliveryLocationId,
                DeliveryLocationName = container.DeliveryLocation.Name,
                WeightInTons = container.WeightInTons,
                VesselArrivalDate = container.VesselArrivalDate,
                PortFreeTimeEndDate = container.PortFreeTimeEndDate,
                EmptyReturnFreeTimeEndDate = container.EmptyReturnFreeTimeEndDate,
                ContainerLoadingDate = container.ContainerLoadingDate,
                Notes = container.Notes,
                Status = container.Status
            };
        }
    }
}