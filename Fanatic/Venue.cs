using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanatic
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int Capacity { get; set; }
        public Surface SurfaceType { get; set; }
        public RoofType RoofType { get; set; }
        public int OpeningYear { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
