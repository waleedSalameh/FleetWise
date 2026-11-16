using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles.Trailer
{
    public class TrailerResponseDto
    {
        public int Id { get; set; }

        public int PlateNumber { get; set; }

        public int AxleCount { get; set; }

        public decimal MaxLoad { get; set; }

        public TrailerType TrailerType { get; set; }

        public DateOnly LicenseExpiryDate { get; set; }
    }
}
