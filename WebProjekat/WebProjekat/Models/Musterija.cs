using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Musterija : Korisnik
    {
        public Musterija(string user, string pass, string ime, string prezime, Pol pol, long jmbg, string broj, string mail, Uloga ul)
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

        }
        public Musterija()
        {
            Voznja = new List<Voznja>();
            Ulogovan = false;

        }
    }
}