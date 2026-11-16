using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.DAL.Entities.Vehicles
{
    public class VehicleAssignment
    {
        public int AssignmentId { get; set; }
        public int VehicleId { get; set; }
        public int TrailerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Vehicle Vehicle { get; set; }
        public Trailer Trailer { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Trip> Trips { get; set; }

    }
}
