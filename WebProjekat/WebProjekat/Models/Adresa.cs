using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Adresa
    {
        public string Naziv { get; set; }
        public int Broj { get; set; }
        public string Mesto { get; set; }
        public int BrojMesta { get; set; }

        public Adresa(string naziv,int broj,string mesto,int brojm)
        {
            Naziv = naziv;
            Broj = broj;
            Mesto = mesto;
            BrojMesta = brojm;
        }

        public Adresa()
        {
          
        }
        
    }
}