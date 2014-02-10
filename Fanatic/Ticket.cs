using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanatic
{
    public class Ticket
    {
        public string League { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public DateTime StartTime { get; set; }
        public Venue Location { get; set; }
        public string Section { get; set; }
        public string Row { get; set; }
        public string Seat { get; set; }
        public string Price { get; set; }
        public string Notes { get; set; }
    }
}
