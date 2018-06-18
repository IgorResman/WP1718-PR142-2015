using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Automobil
    {
        public Vozac Vozac { get; set; }
        public int Godiste { get; set; }
        public string Registracija { get; set; }
        public int BrojVozila { get; set; }
        public TipVozila Tip { get; set; }

        public Automobil()
        {

        }

        public Automobil(Vozac v , int godiste,string registr,int broj,TipVozila tip)
        {
            Vozac = v;
            Godiste = godiste;
            Registracija = registr;
            BrojVozila = broj;
            Tip = tip;
        }
    }
}