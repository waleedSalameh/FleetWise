using FleetWise.BLL.Interface;
using FleetWise.BLL.Interface.Containers;
using FleetWise.BLL.Interface.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.IInternationalExport;
using FleetWise.BLL.Interface.Containers.IncomingAqaba;
using FleetWise.BLL.Interface.Containers.InlandTransport;
using FleetWise.BLL.Interface.Containers.InternationalIncoming;
using FleetWise.BLL.Interface.Customers;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.BLL.Interface.Maintenance;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.BLL.Service;
using FleetWise.BLL.Service.Containers;
using FleetWise.BLL.Service.Containers.ExportAqaba;
using FleetWise.BLL.Service.Containers.IncomingAqaba;
using FleetWise.BLL.Service.Containers.InlandTransport;
using FleetWise.BLL.Service.Containers.InternationalExport;
using FleetWise.BLL.Service.Containers.InternationalIncoming;
using FleetWise.BLL.Service.Containers.Trip;
using FleetWise.BLL.Service.Customers;
using FleetWise.BLL.Service.Fuel;
using FleetWise.BLL.Service.Maintenance;
using FleetWise.BLL.Service.Vehicles;
using FleetWise.BLL.Services;
using FleetWise.BLL.Services.Maintenance;
using FleetWise.Middleware;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FleetWiseDbContext>(options => options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IVehicle, VehicleService>();
builder.Services.AddScoped<IDriver, DriverService>();
builder.Services.AddScoped<IVehicleAssignment, VehicleAssignmentService>();
builder.Services.AddScoped<IMaintenanceRecord,MaintenanceRecordService>();
builder.Services.AddScoped<IFuelRecord,FuelRecordService>();
builder.Services.AddScoped<ITrip, TripService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<IShippingLine, ShippingLineService>();
builder.Services.AddScoped<IDeliveryLocation, DeliveryLocationService>();
builder.Services.AddScoped<IIncomingAqabaBill, IncomingAqabaBillService>();
builder.Services.AddScoped<IIncomingAqabaContainer, IncomingAqabaContainerService>();
builder.Services.AddScoped<IExportAqabaBooking, ExportAqabaBookingService>();
builder.Services.AddScoped<IExportAqabaContainer, ExportAqabaContainerService>();
builder.Services.AddScoped<IInternationalIncoming, InternationalIncomingService>();
builder.Services.AddScoped<ICustomer, CustomerService>();
builder.Services.AddScoped<IInternationalExport, InternationalExportService>();
builder.Services.AddScoped<IInlandTransport, InlandTransportService>();
builder.Services.AddScoped<IExternalVehicles, ExternalVehiclesService>();
builder.Services.AddScoped<ITrailer, TrailerService>();
builder.Services.AddScoped<IFuelPurchaseService, FuelPurchaseService>();
builder.Services.AddScoped<IFuelTransferService, FuelTransferService>();
builder.Services.AddScoped<IVehicleFuelMeasurementService, VehicleFuelMeasurementService>();
builder.Services.AddScoped<IFuelReportService, FuelReportService>();
builder.Services.AddScoped<IOilService, OilService>();
builder.Services.AddScoped<IOilMovementService, OilMovementService>();
builder.Services.AddScoped<ISparePartService, SparePartService>();
builder.Services.AddScoped<ISparePartMovementService, SparePartMovementService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Register services
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
