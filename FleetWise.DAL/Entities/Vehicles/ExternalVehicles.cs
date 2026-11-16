using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Vehicles
{
    public class ExternalVehicles
    {
        public int Id { get; set; }
        public int PlateNumber { get; set; }
        public string DriverName { get; set; }= string.Empty;
        public string PhoneNumber {  get; set; }= string.Empty;
        public bool IsDeleted { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();


    }
}
