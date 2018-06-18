using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebProjekat.Models;

namespace WebProjekat.Controllers
{
    public class TaxiController : Controller
    {
        // GET: Taxi
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RegistrujSe(string ime ,string prezime, string pol,string jmbg,string korisnicko,string lozinka,string mail,string broj)
        {
            Pol p = Pol.Muski;
            switch(pol)
            {
                case "muski":
                    p = Pol.Muski;
                    break;
                case "zenski":
                    p = Pol.Zenski;
                    break;
            }

            long jmb = long.Parse(jmbg);

            Musterija m = new Musterija(korisnicko, lozinka, ime, prezime, p, jmb, broj, mail, Uloga.Musterija);

            Registrovani.Musterije.Add(m);

            Sacuvaj(Registrovani.Musterije);

            return View("Uspeh",m);
            
        }

        private void Sacuvaj(List<Musterija> musterije)
        {
            string filename = "Module1Database.xml";
            XmlWriter writer = null;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;

                writer = XmlWriter.Create(filename, settings);
                writer.WriteStartElement("Ulogovani");

                foreach (Musterija m in musterije)
                {
                    writer.WriteStartElement("Korisnik");
                    writer.WriteElementString("Ime", m.Ime);
                    writer.WriteElementString("Prezime", m.Prezime);
                    writer.WriteElementString("Jmbg:", m.ToString());
                    writer.WriteElementString("Korisnicko ime", m.KorisnickoIme);
                    writer.WriteElementString("Lozinka:", m.Lozinka);
                    writer.WriteElementString("Pol:", m.Pol.ToString());
                    writer.WriteElementString("E-Mail:", m.Mail);
                    writer.WriteElementString("Uloga:", m.Uloga.ToString());
                    writer.WriteStartElement("Voznje:");
                    int i = 1;
                    foreach(Voznja v in m.Voznja)
                    {
                        writer.WriteStartElement("Voznja broj " + i.ToString());
                        writer.WriteElementString("Datum porudzbine:" , v.DatumIVremePorudzbine.ToString());
                        writer.WriteElementString("Pocetna lokacija:", v.PocetnaLokacija.ToString());
                        writer.WriteElementString("Krajnja lokacija:", v.Odrediste.ToString());
                        writer.WriteElementString("Tip vozila:",v.Tip.ToString());
                        writer.WriteElementString("Musterija ime:", v.Musterija.Ime);
                        writer.WriteElementString("Musterija prezime:", v.Musterija.Prezime);
                        writer.WriteElementString("Vozac ime:", v.Vozac.Ime);
                        writer.WriteElementString("Vozac prezime:", v.Vozac.Prezime);
                        writer.WriteElementString("Dispecer ime:", v.Dispecer.Ime);
                        writer.WriteElementString("Dispececr prezime:", v.Dispecer.Prezime);
                        writer.WriteElementString("Iznos:", v.Iznos.ToString());
                    }
                }
                writer.WriteEndElement();

                writer.Flush();


            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        public ActionResult Registracija()
        {
            return View();
        }

        public ActionResult Uloguj(string korisnickoIme,string lozinka)
        {
            foreach(Musterija m in Registrovani.Musterije)
            {
                if(m.KorisnickoIme == korisnickoIme && m.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", m.KorisnickoIme);
                }
                else if (m.KorisnickoIme == korisnickoIme && m.Lozinka == lozinka)
                {
                    return View("musterijaWelcome",m);
                }
            }

            foreach(Vozac v in Registrovani.Vozaci)
            {
                if (v.KorisnickoIme == korisnickoIme && v.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", v.KorisnickoIme);

                }
                else if (v.KorisnickoIme == korisnickoIme && v.Lozinka == lozinka)
                {
                    return View("vozacWelcome",v);
                }
            }

            foreach(Dispecer d in Registrovani.Dispeceri)
            {
                if (d.KorisnickoIme == korisnickoIme && d.Lozinka != lozinka)
                {
                    return View("LozinkaGreska", d.KorisnickoIme);

                }
                else if (d.KorisnickoIme == korisnickoIme && d.Lozinka == lozinka)
                {
                    return View("dispecerWelcome", d);
                }
            }


            return View("Greska");
        }
    }
}