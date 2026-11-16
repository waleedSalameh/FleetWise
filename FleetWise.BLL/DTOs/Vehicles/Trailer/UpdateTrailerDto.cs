using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles.Trailer
{
    public class UpdateTrailerDto
    {
        [Range(1, int.MaxValue)]
        public int PlateNumber { get; set; }

        [Range(1, int.MaxValue)]
        public int AxleCount { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal MaxLoad { get; set; }

        [EnumDataType(typeof(TrailerType))]
        public TrailerType TrailerType { get; set; }

        public DateOnly LicenseExpiryDate { get; set; }
    }
}
