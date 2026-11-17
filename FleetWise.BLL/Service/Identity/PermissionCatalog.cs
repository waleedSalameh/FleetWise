namespace FleetWise.BLL.Service.Identity;

public record PermissionDefinition(string Code, string Name, string Module);

public static class PermissionCatalog
{
    public static IReadOnlyList<PermissionDefinition> All { get; } = Build();

    private static IReadOnlyList<PermissionDefinition> Build()
    {
        var permissions = new List<PermissionDefinition>();

        Add(permissions, "Vehicles", "View", "Create", "Update", "Delete");
        Add(permissions, "Drivers", "View", "Create", "Update", "Delete");
        Add(permissions, "VehicleAssignments", "View", "Create", "Update", "Delete");
        Add(permissions, "Trips", "View", "Create", "Update", "Delete");
        Add(permissions, "Trailers", "View", "Create", "Update", "Delete");

        Add(permissions, "ExternalVehicles", "View", "Create", "Delete");

        Add(permissions, "Maintenance", "View", "Create", "Update", "Delete", "ViewCosts");
        Add(permissions, "SpareParts", "View", "Create", "Update", "Delete");
        Add(permissions, "SparePartMovements", "View", "Create");
        Add(permissions, "Oils", "View", "Create", "Update", "Delete");
        Add(permissions, "OilMovements", "View", "Create");

        Add(permissions, "FuelRecords", "View", "Create", "Update", "Delete", "ViewCosts");
        Add(permissions, "FuelPurchases", "View", "Create");
        Add(permissions, "FuelTransfers", "View", "Create", "Delete", "Reverse");
        Add(permissions, "FuelReports", "View", "ViewFinancial");
        Add(permissions, "VehicleFuelMeasurements", "View", "Create", "Approve");

        Add(permissions, "Customers", "View", "Create", "Update", "Delete");
        Add(permissions, "ShippingLines", "View", "Create", "Update", "Delete");
        Add(permissions, "DeliveryLocations", "View", "Create", "Update", "Delete");

        Add(permissions, "IncomingAqabaBills", "View", "Create", "Update", "Delete");
        Add(permissions, "IncomingAqabaContainers", "View", "Create", "Update", "Delete");
        Add(permissions, "ExportAqabaBookings", "View", "Create", "Update", "Delete");
        Add(permissions, "ExportAqabaContainers", "View", "Create", "Update", "Delete");
        Add(permissions, "InternationalIncoming", "View", "Create", "Update", "Delete");
        Add(permissions, "InternationalExports", "View", "Create", "Update", "Delete");
        Add(permissions, "InlandTransport", "View", "Create", "Update", "Delete");

        Add(permissions, "Dashboard", "View");

        Add(permissions, "Users", "View", "Create", "Update", "Disable");
        Add(permissions, "Roles", "Manage");
        Add(permissions, "Permissions", "Manage");

        return permissions.AsReadOnly();
    }

    private static void Add(
        List<PermissionDefinition> permissions,
        string module,
        params string[] operations
    )
    {
        foreach (var operation in operations)
        {
            permissions.Add(
                new PermissionDefinition(
                    Code: $"{module}.{operation}",
                    Name: $"{operation} {module}",
                    Module: module
                )
            );
        }
    }
}
