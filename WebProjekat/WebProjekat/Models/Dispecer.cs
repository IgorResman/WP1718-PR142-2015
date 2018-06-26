using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProjekat.Models
{
    public class Dispecer:Korisnik
    {

        public bool TraziVozac { get; set; } = false;
        public bool TraziMusteriju { get; set; } = false;

        public List<Voznja> NadjeniVozaci { get; set; }
        public List<Voznja> NadjeneMusterije { get; set; }
        public Dispecer()
        {
            NadjeniVozaci = new List<Voznja>();
            NadjeneMusterije = new List<Voznja>();
            Voznja = new List<Voznja>();
            Ulogovan = false;
            Filtrirane = new List<Voznja>();
            Sortirane = new List<Voznja>();
            Pretrazene = new List<Voznja>();

        }
        public Dispecer(string user, string pass, string ime, string prezime, Pol pol, long jmbg, string broj, string mail, Uloga ul)
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
            NadjeniVozaci = new List<Voznja>();
            NadjeneMusterije = new List<Voznja>();
            Pretrazene = new List<Voznja>();

        }
    }
}