using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Komentar
    {
        public string Opis { get; set; }
        public DateTime Datum { get; set; }
        public Korisnik Korisnik { get; set; }
        public Voznja Voznja { get; set; }
        public Ocena Ocena { get; set; }
        public Komentar()
        {

        }
        public Komentar(string op,DateTime date , Korisnik k , Voznja v,Ocena o)
        {
            Opis = op;
            Datum = date;
            Korisnik = k;
            Voznja = v;
            Ocena = o;
        }

    }
}