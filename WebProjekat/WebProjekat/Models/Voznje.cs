using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Voznje
    {
        public static List<Voznja> SveVoznje { get; set; }

        public Voznje()
        {
            SveVoznje = new List<Voznja>();
        }
    }
}