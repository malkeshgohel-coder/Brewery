﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brewery.DTOs
{
    public class BreweryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        // computed distance (optional, when lat/lon provided)
        public double? DistanceInKm { get; set; }
    }
}
