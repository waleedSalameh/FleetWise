using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Drivers
{
    public class UpdateDriverDto
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly LicenseExpiryDate { get; set; }
        public int Status { get; set; }
    }
}
