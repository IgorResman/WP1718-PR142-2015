using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Korisnik
    {
        public string KorisnickoIme { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public Pol Pol { get; set; }
        public long Jmbg { get; set; }
        public string BrTelefona { get; set; }
        public string Mail { get; set; }
        public Uloga Uloga { get; set; }
        public List<Voznja> Voznja { get; set; }
        public bool Ulogovan { get; set; }
        public bool Filter { get; set; } = false;
        public bool Sortiranje { get; set; } = false;
        public bool Pretrazivanje { get; set; } = false;

        public List<Voznja> Filtrirane { get; set; }
        public List<Voznja> Sortirane { get; set; }
        public List<Voznja> Pretrazene { get; set; }

        public Korisnik(string user,string pass,string ime,string prezime,Pol pol ,long jmbg,string broj,string mail,Uloga ul)
        {
            KorisnickoIme = user;
            Lozinka = pass;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Jmbg = jmbg;
            BrTelefona = broj;
            Mail = mail;
            Uloga = ul;
            Voznja = new List<Voznja>();
            Ulogovan = false;
            Filtrirane = new List<Voznja>();
            Sortirane = new List<Voznja>();
            Pretrazene = new List<Voznja>();
        }

        public Korisnik()
        {
            Voznja = new List<Voznja>();
            Ulogovan = false;
            Filtrirane = new List<Voznja>();
            Sortirane = new List<Voznja>();
            Pretrazene = new List<Voznja>();

        }

    }
}