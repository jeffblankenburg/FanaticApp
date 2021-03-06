﻿using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanatic
{
    public class User
    {
        public static MobileServiceClient MobileService = new MobileServiceClient("https://fanaticapp.azure-mobile.net/", "sCUZwWODAHEHZSpHUgaJhmWkAXLByB75");
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ZipCode { get; set; }
        public Team MLB { get; set; }
        public Team NFL { get; set; }
        public Team NBA { get; set; }
        public Team NHL { get; set; }
        public Team MLS { get; set; }
        public Team MiLB { get; set; }
        public Team FAPL { get; set; }
        public List<Ticket> Tickets = new List<Ticket>();
        public string MicrosoftAccountID { get; set; }
        public string GoogleAccountID { get; set; }
        public string TwitterAccountID { get; set; }
        public string FacebookAccountID { get; set; }

        public User()
        {
            MLB = new Team { Abbreviation = "NONE" };
            NFL = new Team { Abbreviation = "NONE" };
            NBA = new Team { Abbreviation = "NONE" };
            NHL = new Team { Abbreviation = "NONE" };
            MLS = new Team { Abbreviation = "NONE" };
            MiLB = new Team { Abbreviation = "NONE" };
            FAPL = new Team { Abbreviation = "NONE" };
        }
    }
}
