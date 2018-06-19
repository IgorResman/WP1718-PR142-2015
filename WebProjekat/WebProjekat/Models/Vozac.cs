using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Vozac : Korisnik
    {
        public Lokacija Lokacija { get; set; }
        public Automobil Automobil { get; set; }
        public Vozac()
        {
            Adresa a = new Adresa();
            a.Naziv = "Narodnog fronta";
            a.Broj = 67;
            a.BrojMesta = 21000;
            a.Mesto = "Novi Sad";
            Lokacija l = new Lokacija(4, 7, a);
        }
        public Vozac(string user, string pass, string ime, string prezime, Pol pol, long jmbg, string broj, string mail, Uloga ul)
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

            Adresa a = new Adresa();
            a.Naziv = "Narodnog fronta";
            a.Broj = 67;
            a.BrojMesta = 21000;
            a.Mesto = "Novi Sad";
            Lokacija l = new Lokacija(4, 7, a);
            Lokacija = l;
        }
    }
}