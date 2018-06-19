using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Registrovani
    {
        public static List<Dispecer> Dispeceri { get; set;}
        public static List<Vozac> Vozaci { get; set; }
        public static List<Musterija> Musterije { get; set; }
        public static List<Korisnik> SviZajedno { get; set; }

        public Registrovani()
        {
            Vozaci = new List<Vozac>();
            Dispeceri = new List<Dispecer>();
            Musterije = new List<Musterija>();
            SviZajedno = new List<Korisnik>();
        }
    }
}