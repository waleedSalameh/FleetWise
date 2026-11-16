using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles.VehicleAssignments
{
    public class VehicleAssignmentDto
    {
        public int AssignmentId { get; set; }
        public int VehicleId { get; set; }
        public int TrailerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PlateNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}
