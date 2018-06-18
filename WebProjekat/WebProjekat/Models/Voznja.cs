using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Voznja
    {
        public DateTime DatumIVremePorudzbine { get; set; }
        public Lokacija PocetnaLokacija { get; set; }
        public TipVozila Tip { get; set; }
        public Musterija Musterija { get; set; }
        public Lokacija Odrediste { get; set; }
        public Dispecer Dispecer { get; set; }
        public Vozac Vozac { get; set; }
        public double Iznos { get; set; }
        public Komentar Komentar { get; set; }
        public StatusVoznje Status { get; set; }

        public Voznja(DateTime dat,Lokacija pocetak,TipVozila tip,Musterija m,Lokacija odrediste,Dispecer d,Vozac v,double iznos,Komentar kom,StatusVoznje stat)
        {
            DatumIVremePorudzbine = dat;
            PocetnaLokacija = pocetak;
            Tip = tip;
            Musterija = m;
            Odrediste = odrediste;
            Dispecer = d;
            Vozac = v;
            Iznos = iznos;
            Komentar = kom;
            Status = stat;
        }

        public Voznja()
        {

        }
    }
}