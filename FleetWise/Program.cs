using System.Threading.RateLimiting;
using FleetWise.API.Authorization.Permissions;
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
using FleetWise.BLL.Service.Identity;
using FleetWise.BLL.Service.Maintenance;
using FleetWise.BLL.Service.Vehicles;
using FleetWise.BLL.Services;
using FleetWise.BLL.Services.Maintenance;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using FleetWise.DAL.Entities.Identity;
using FleetWise.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FleetWiseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddAuthorization();
builder.Services.AddDataProtection();

builder
    .Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 12;
        options.Password.RequiredUniqueChars = 4;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FleetWiseDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing.");

var signingKeyBase64 = builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("JWT SigningKey is missing.");

byte[] signingKeyBytes;

try
{
    signingKeyBytes = Convert.FromBase64String(signingKeyBase64);
}
catch (FormatException)
{
    throw new InvalidOperationException("JWT SigningKey must be a valid Base64 value.");
}

if (signingKeyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT SigningKey must contain at least 32 bytes.");
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });
builder.Services.AddScoped<IVehicle, VehicleService>();
builder.Services.AddScoped<IDriver, DriverService>();
builder.Services.AddScoped<IVehicleAssignment, VehicleAssignmentService>();
builder.Services.AddScoped<IMaintenanceRecord, MaintenanceRecordService>();
builder.Services.AddScoped<IFuelRecord, FuelRecordService>();
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
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IUserPermissionManagementService, UserPermissionManagementService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<IAdminAuditService, AdminAuditService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Register services
builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = " access token",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
    });
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthFivePerMinute", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true,
            }
        )
    );
});

var app = builder.Build();
await PermissionSeeder.SeedAsync(app.Services);
app.UseExceptionHandler();

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AngularPolicy");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
